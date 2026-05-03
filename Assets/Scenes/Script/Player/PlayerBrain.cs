using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    #region Systems

    private PlayerInput      _input;
    private PlayerLocomotion _locomotion;
    private Health           _health;
    private Shooter          _shooter;
    private bool             _hasBeenRevived = false;

    #endregion
    

    private Animator _animator;

    #region Shared State and Logger

    private PlayerState _state;
    private GameLogger  _logger;

    #endregion

    [Header("Revival")]
    [SerializeField] private GameObject _ghostPrefab;

    #region Interaction

    [Header("Interaction")]
    [SerializeField] private float     _interactRange     = 2f;
    [SerializeField] private LayerMask _interactLayerMask = -1;
    private PlayerInput _playerInput;

    #endregion

    #region Unity Messages

    private void Awake()
    {
        _logger = new GameLogger
        {
            LogInput    = true,
            LogGround   = true,
            LogJump     = true,
            LogGravity  = false,
            LogBounce   = true,
            LogMovement = false
        };

        _state = new PlayerState();

        _input      = GetComponent<PlayerInput>();
        _locomotion = GetComponent<PlayerLocomotion>();
        _health     = GetComponent<Health>();
        _shooter    = GetComponent<Shooter>();
        _animator   = GetComponentInChildren<Animator>();

        _input.Initialize(_logger);
        _locomotion.Initialize(_logger);

        _playerInput = _input;

        if (_health != null)
            _health.onDeath.AddListener(OnPlayerDeath);

        _logger.LogFrom("PlayerBrain initialized", gameObject);
    }

    private void Update()
    {
        _input.Tick(_state);

        if (_playerInput.GetInteractPressed())
            InteractWithWorld();

        if (_shooter != null && _shooter.enabled && _playerInput.GetFireHeld())
        {
            Transform firePoint = transform.Find("GunPivot/GunVisual/FirePoint");
            Vector2 spawnPos = firePoint != null ? firePoint.position : (Vector2)transform.position;
            _shooter.Fire(spawnPos, _playerInput.GetAimDirection(), gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (_state.IsDead) return;

        _locomotion.SyncFromRigidbody(_state);
        _locomotion.CheckGround(_state);
        _locomotion.HandleHorizontal(_state);
        _locomotion.HandleJump(_state);
        _locomotion.HandleGravity(_state);
        _locomotion.HandleBounce(_state);
        _locomotion.UpdateAnimation(_state);
        _locomotion.ApplyVelocity(_state);
    }

    #endregion

    #region Public API

    public void Bounce(float force)
    {
        if (_state.IsDead) return;
        _state.Stomped     = true;
        _state.BounceForce = force;
        _state.EndedJumpEarly = false;
        _logger.Bounce($"Bounce received by Brain — force: {force}");
    }

    public void TakeDamage(float knockbackForce, int amount, Vector2? knockbackDirection = null)
    {
        if (_state.IsDead) return;

        _health?.TakeDamage(amount);
        _animator?.SetTrigger("Hurt");

        if (knockbackDirection.HasValue)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = knockbackDirection.Value * knockbackForce;
            _logger.Health($"Took {amount} damage + knockback", gameObject);
        }
        else
        {
            _logger.Health($"Took {amount} damage", gameObject);
        }
    }

    public void Revive()
    {
        if (!_state.IsDead) return;

        _state.IsDead = false;
        _hasBeenRevived = true;
        _health?.Revive();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
        }
        if (col != null)
            col.enabled = true;

        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
            sr.enabled = true;

        // Notify HealthDisplay
        GetComponentInChildren<HealthDisplay>()?.OnPlayerRevived();

        _logger.Health("Player revived", gameObject);
    }

    #endregion

    #region Interaction

    private void InteractWithWorld()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _interactRange, _interactLayerMask);
        Interactable closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            Interactable inter = hit.GetComponent<Interactable>();
            if (inter != null)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = inter;
                }
            }
        }

        if (closest != null)
            closest.Interact(gameObject);
    }

    #endregion

    #region Death Handling

    private void OnPlayerDeath()
    {
        _state.IsDead = true;
        _animator?.SetTrigger("Die");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
        col.enabled = false;

        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
            sr.enabled = false;

        _logger.Health("Player died", gameObject);

        if (!_hasBeenRevived && _ghostPrefab != null)
        {
            GameObject ghost = Instantiate(_ghostPrefab, transform.position, Quaternion.identity);
            ghost.GetComponent<RevivalGhost>()?.SetTarget(this);
        }

        // Check if ALL players are dead
// Notify GameUIController so it can check if all players are dead
        
    GameUIController.Instance?.CheckAllPlayersDead();
    

}

    #endregion

    public void SetPlayerCollisionEnabled(PlayerBrain otherPlayer, bool enabled)
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D otherCollider = otherPlayer.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(myCollider, otherCollider, !enabled);
    }

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce = 78f;

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }

    #endregion
}