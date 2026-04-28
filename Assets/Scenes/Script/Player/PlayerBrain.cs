// -------------------------------------------------------
// BRAIN — The Orchestrator
// Only job: create systems, inject logger, run in order.
// No movement logic lives here — pure coordination.
// External world only ever talks to Brain.
// -------------------------------------------------------
using UnityEngine;


public class PlayerBrain : MonoBehaviour
{
    private Health _health;
    private bool _hasBeenRevived = false; // Track if the player has been revived at least once
    #region Systems

    private PlayerInput      _input;
    private PlayerLocomotion _locomotion;

    #endregion

    #region Shared State and Logger

    private PlayerState _state;
    private GameLogger  _logger;
    private Rigidbody2D _rb;

    #endregion
    [Header("Revival")]
    [SerializeField] private GameObject _ghostPrefab; // Assign a ghost prefab in Inspector

    #region Unity Messages

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.onDeath.AddListener(OnPlayerDeath);
        _rb = GetComponent<Rigidbody2D>();
        // Create logger first — everything else needs it
        _logger = new GameLogger
        {
            // Configure which categories show in console
            // Change these here — nowhere else needs to change
            LogInput    = true,
            LogGround   = true,
            LogJump     = true,
            LogGravity  = false, // every frame — keep off unless debugging gravity
            LogBounce   = true,
            LogMovement = false  // every frame — keep off unless debugging movement

        };

        // Create shared state — the whiteboard
        _state = new PlayerState();

        // Find systems
        _input      = GetComponent<PlayerInput>();
        _locomotion = GetComponent<PlayerLocomotion>();

        // Inject logger into each system — they don't create their own
        // This is dependency injection — change logger behavior in one place
        _input.Initialize(_logger);
        _locomotion.Initialize(_logger);

        _logger.Warning("PlayerBrain initialized");
    }
    public void SetPlayerCollisionEnabled(PlayerBrain otherPlayer, bool enabled)
  {
    Collider2D myCollider = GetComponent<Collider2D>();
    Collider2D otherCollider = otherPlayer.GetComponent<Collider2D>();
    Physics2D.IgnoreCollision(myCollider, otherCollider, !enabled);
  }

    private void Update()
    {
        // Input in Update — most responsive to player
        _input.Tick(_state);
    }

    private void FixedUpdate()
    {
        if (_state.IsDead) return; // stop all systems on death

        _locomotion.SyncFromRigidbody(_state);
        _locomotion.CheckGround(_state);
        _locomotion.HandleHorizontal(_state);
        _locomotion.HandleJump(_state);
        _locomotion.HandleGravity(_state);
        _locomotion.HandleBounce(_state);
        _locomotion.ApplyVelocity(_state);
    }

    #endregion
    // Add this field in PlayerBrain


// Modify OnPlayerDeath
private void OnPlayerDeath()
{
    _state.IsDead = true;

    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    Collider2D col = GetComponent<Collider2D>();

    rb.linearVelocity = Vector2.zero;
    rb.bodyType = RigidbodyType2D.Kinematic;
    rb.simulated = false;
    col.enabled = false;

    // Disable sprite on all children
    SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
    foreach (SpriteRenderer sr in srs)
        sr.enabled = false;

    _logger.Health("Player died", gameObject);

    // Spawn a revival ghost
    if (!_hasBeenRevived && _ghostPrefab != null)
   {
    GameObject ghost = Instantiate(_ghostPrefab, transform.position, Quaternion.identity);
    ghost.GetComponent<RevivalGhost>()?.SetTarget(this);
   }
}

// Add a public Revive method
public void Revive()
{
    if (!_state.IsDead) return;
    _hasBeenRevived = true;

    _state.IsDead = false;
    _health.Revive(); // reset HP

    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    Collider2D col = GetComponent<Collider2D>();

    rb.bodyType = RigidbodyType2D.Dynamic;
    rb.simulated = true;
    col.enabled = true;

    // Re-enable sprite
    SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
    foreach (SpriteRenderer sr in srs)
        sr.enabled = true;

    _logger.Health("Player revived", gameObject);
    GetComponentInChildren<HealthDisplay>()?.OnPlayerRevived();
}
//    private void OnPlayerDeath()
//   {
//     _state.IsDead = true;

//     Rigidbody2D rb = GetComponent<Rigidbody2D>();
//     Collider2D col = GetComponent<Collider2D>();

//     rb.linearVelocity = Vector2.zero;
//     rb.bodyType       = RigidbodyType2D.Kinematic;
//     rb.simulated      = false;
//     col.enabled       = false;

//     // Disable sprite on ALL children
//     SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
//     foreach (SpriteRenderer sr in spriteRenderers)
//     {
//         sr.enabled = false;
//     }

//     _logger.Health("Player died", gameObject);
//   }
//    private void OnPlayerDeath()
//   {
//     _state.IsDead = true;
//     _logger.Health("Player died", gameObject);

//     Rigidbody2D rb = GetComponent<Rigidbody2D>();
//     Collider2D col = GetComponent<Collider2D>();

//     rb.linearVelocity = Vector2.zero;
//     rb.bodyType       = RigidbodyType2D.Kinematic;
//     rb.simulated      = false;
//     col.enabled       = false;
//   }
    #region Public API — External systems only talk to Brain

    // Called by EnemyBase on stomp
    public void Bounce(float force)
    {
        if (_state.IsDead) return;
        _state.Stomped     = true;
        _state.BounceForce = force;
        _state.EndedJumpEarly    = false;
        _logger.Bounce($"Bounce received by Brain — force: {force}");
    }

//  public void TakeDamage(int amount, Vector2? knockbackDirection = null)
// {
//     if (_state.IsDead) return;

//     _health.TakeDamage(amount);

//     if (knockbackDirection.HasValue)
//     {
//         _state.Velocity = knockbackDirection.Value * 10f;
//         _logger.Health($"Took {amount} damage + knockback — HP: {_health.CurrentHealth}", gameObject);
//     }
//     else
//     {
//         _logger.Health($"Took {amount} damage — HP: {_health.CurrentHealth}", gameObject);
//     }
// }
   public void TakeDamage(int amount, Vector2? knockbackDirection = null)
 {
    if (_state.IsDead) return;

    _health.TakeDamage(amount);

    if (knockbackDirection.HasValue)
    {
        // Apply knockback directly to the Rigidbody, immediately
        _rb.linearVelocity = knockbackDirection.Value * 15f;
        _logger.Health($"Took {amount} damage + knockback", gameObject);
    }
    else
    {
        _logger.Health($"Took {amount} damage", gameObject);
    }
  }

    #endregion

#if UNITY_EDITOR
    #region Editor Validation

    private void OnValidate()
    {
        if (GetComponent<PlayerInput>() == null)
            Debug.LogWarning("PlayerInput missing from this GameObject", this);

        if (GetComponent<PlayerLocomotion>() == null)
            Debug.LogWarning("PlayerLocomotion missing from this GameObject", this);
    }

    #endregion
#endif
}