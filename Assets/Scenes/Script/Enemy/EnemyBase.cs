using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    #region Settings

    [Header("Patrol")]
    [SerializeField] private float moveSpeed      = 2f;
    [SerializeField] private float patrolRange    = 4f;

    [Header("Detection")]
    [SerializeField] private float wallCheckDist  = 0.6f;
    [SerializeField] private float edgeCheckDist  = 1f;

    [Header("Stomp")]
    [SerializeField] private float bounceForce    = 20f;
    [SerializeField] private float stompTolerance = 0.3f;
    [Header("Knock")]
    [SerializeField] private float KnockBackForce    = 12f;

    #endregion

    #region Private

    private Rigidbody2D  _rb;
    private Collider2D   _col;
    private Health       _health;
    private GameLogger   _logger;
    private Vector2      _startPos;
    private int          _direction = 1;
    private bool         _isDead;
    private float        _lastFlipTime;
    private const float  FlipCooldown = 0.2f;

    #endregion

    #region Unity Messages

    private void Awake()
    {
        _rb                = GetComponent<Rigidbody2D>();
        _col               = GetComponent<Collider2D>();
        _health            = GetComponent<Health>();
        _logger            = GameLogger.Instance ?? new GameLogger();
        _rb.freezeRotation = true;
        _startPos          = transform.position;

        if (_health != null)
            _health.onDeath.AddListener(Die);
    }

    private void FixedUpdate()
    {
        if (_health != null && _health.IsDead) return;
        Patrol();
    }

    #endregion

    #region Patrol

    private void Patrol()
    {
        bool shouldFlip = false;

        float dist = transform.position.x - _startPos.x;
        if (Mathf.Abs(dist) >= patrolRange)
            shouldFlip = true;

        if (IsWallAhead())
            shouldFlip = true;

        if (IsEdgeAhead())
            shouldFlip = true;

        if (shouldFlip && Time.time > _lastFlipTime + FlipCooldown)
        {
            _direction   *= -1;
            _lastFlipTime = Time.time;
            Flip();
        }

        _rb.linearVelocity = new Vector2(moveSpeed * _direction, _rb.linearVelocity.y);
    }

    #endregion

    #region Detection
//     private bool IsWallAhead()
// {
//     //int mask = ~LayerMask.GetMask("Enemy", "Player");

//     RaycastHit2D hit = Physics2D.Raycast(
//         _col.bounds.center,
//         Vector2.right * _direction,
//         wallCheckDist
//         //,mask
//     );

//     // --- DIAGNOSTIC START ---
//     // Draw the ray in the Scene view so you can SEE what it hits
//     Debug.DrawRay(_col.bounds.center, Vector2.right * _direction * wallCheckDist, Color.cyan, 0.1f);

//     // Log EVERYTHING useful
//     if (hit.collider != null)
//     {
//         Debug.Log($"WALL RAY HIT: {hit.collider.gameObject.name} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} ({hit.collider.gameObject.layer}) | Tag: {hit.collider.gameObject.tag}");
//     }
//     else
//     {
//         Debug.Log("WALL RAY: Hit nothing");
//     }
//     // --- DIAGNOSTIC END ---

//     return hit.collider != null && hit.collider != _col;
// }
    private bool IsWallAhead()
    {
        
         int mask = LayerMask.GetMask("Ground","Platform");
        RaycastHit2D hit = Physics2D.Raycast(
            _col.bounds.center,
            Vector2.right * _direction,
            wallCheckDist,
               mask
        );

        return hit.collider != null && hit.collider != _col;
    }

    private bool IsEdgeAhead()
    {
        int maske = LayerMask.GetMask("Ground","Platform");
        Vector2 origin = new Vector2(
            transform.position.x + _direction * (_col.bounds.size.x * 0.5f + 0.05f),
            transform.position.y - _col.bounds.size.y * 0.5f
        );

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, edgeCheckDist,maske);
        return hit.collider == null;
    }

    private void Flip()
    {
        Vector3 s = transform.localScale;
        s.x      *= -1;
        transform.localScale = s;
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_health != null && _health.IsDead) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerBrain player = collision.gameObject.GetComponent<PlayerBrain>();
        if (player == null) return;

        if (IsStomped(collision))
        {
            _logger?.Enemy("Stomped");
            _health?.TakeDamage(1);
            player.Bounce(bounceForce);
        }
        else
        {
            _logger?.Enemy("Side hit");
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            knockbackDir.y = 0.5f;
            knockbackDir.Normalize();
            player.TakeDamage(KnockBackForce,1, knockbackDir);
        }
    }

    private bool IsStomped(Collision2D collision)
    {
        bool fallingDown = collision.rigidbody != null &&
                           collision.rigidbody.linearVelocity.y < 0.1f;

        float playerBottom = collision.collider.bounds.min.y;
        float enemyTop     = _col.bounds.max.y;
        bool  fromAbove    = playerBottom >= enemyTop - stompTolerance;

        _logger?.Enemy($"Stomp check — falling: {fallingDown}, fromAbove: {fromAbove}");
        return fallingDown && fromAbove;
    }
  private void Die()
 {
    Debug.Log($"DIE CALLED on (gameObject.name)");
    _col.enabled       = false;
    _rb.linearVelocity = Vector2.zero;
    _rb.bodyType       = RigidbodyType2D.Kinematic;
    _rb.simulated      = false;

    // Disable sprite on ALL children
    SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    foreach (SpriteRenderer sr in spriteRenderers)
    {
        sr.enabled = false;
    }

    _logger?.Health("Enemy died", gameObject);
 }
//     private void Die()
//    {
//     _isDead            = true;
//     _col.enabled       = false;
//     _rb.linearVelocity = Vector2.zero;
//     _rb.bodyType       = RigidbodyType2D.Kinematic;
//     _rb.simulated      = false;    // Rigidbody completely stops simulating

//     _logger?.Health("Enemy died", gameObject);

//     // Hide the sprite so it looks gone
//     SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
//     if (sr != null) sr.enabled = false;
//   }
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (_col == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(_startPos.x, transform.position.y),
            new Vector3(patrolRange * 2, 0.1f, 0)
        );

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_col.bounds.center, Vector2.right * _direction * wallCheckDist);

        Vector2 edgeOrigin = new Vector2(
            transform.position.x + _direction * (_col.bounds.size.x * 0.5f + 0.05f),
            transform.position.y - _col.bounds.size.y * 0.5f
        );
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(edgeOrigin, Vector2.down * edgeCheckDist);
    }

    #endregion
}