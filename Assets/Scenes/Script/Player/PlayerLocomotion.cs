using UnityEngine;


public class PlayerLocomotion : MonoBehaviour
{
    #region Settings

    [Header("Movement")]
    [SerializeField] private float maxSpeed           = 13f;
    [SerializeField] private float acceleration       = 120f;
    [SerializeField] private float groundDeceleration = 60f;
    [SerializeField] private float airDeceleration    = 30f;

    [Header("Jump")]
    [SerializeField] private float jumpPower           = 36f;
    [SerializeField] private float coyoteTime          = 0.15f;
    [SerializeField] private float jumpBuffer          = 0.15f;
    [SerializeField] private float maxFallSpeed        = 40f;
    [SerializeField] private float fallAcceleration    = 110f;
    [SerializeField] private float jumpEndEarlyGravity = 300f;
    [SerializeField] private float groundingForce      = -1.5f;
    [SerializeField] private float grounderDistance    = 0.05f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    #endregion

    #region Private

    private Rigidbody2D       _rb;
    private CapsuleCollider2D _col;
    private GameLogger        _logger;

    #endregion

    #region Injection

    public void Initialize(GameLogger logger)
    {
        _logger = logger;
    }

    #endregion

    #region Unity Messages

    private void Awake()
    {
        _rb              = GetComponent<Rigidbody2D>();
        _col             = GetComponent<CapsuleCollider2D>();
        _rb.gravityScale = 0f;
    }

    #endregion

    #region Sync

    public void SyncFromRigidbody(PlayerState state)
    {
        state.Velocity  = _rb.linearVelocity;
        state.Time     += Time.deltaTime; // precise timestamp accumulation
    }

    #endregion

    #region Ground Detection

public void CheckGround(PlayerState state)
{
    bool originalQueriesHitTriggers = Physics2D.queriesHitTriggers;
    Physics2D.queriesHitTriggers = false;

    Physics2D.queriesStartInColliders = false;

    bool wasGrounded = state.IsGrounded;

    bool groundHit = Physics2D.CapsuleCast(
        _col.bounds.center,
        _col.size,
        _col.direction,
        0f,
        Vector2.down,
        grounderDistance,
        groundLayer
    );

    bool ceilingHit = Physics2D.CapsuleCast(
        _col.bounds.center,
        _col.size,
        _col.direction,
        0f,
        Vector2.up,
        grounderDistance,
        groundLayer
    );

    if (ceilingHit) state.Velocity.y = Mathf.Min(0, state.Velocity.y);

    if (!wasGrounded && groundHit)
    {
        state.IsGrounded     = true;
        state.CoyoteUsable   = true;
        state.BufferUsable   = true;
        state.EndedJumpEarly = false;
        _logger?.Ground($"Landed — impact: {state.Velocity.y:F2}");
    }
    else if (wasGrounded && !groundHit)
    {
        state.IsGrounded      = false;
        state.FrameLeftGround = state.Time;
        _logger?.Ground("Left ground — coyote started");
    }
    else
    {
        state.IsGrounded = groundHit;
    }

    Physics2D.queriesStartInColliders = true;
    Physics2D.queriesHitTriggers = originalQueriesHitTriggers;
}

    #endregion

    #region Horizontal Movement

    public void HandleHorizontal(PlayerState state)
    {
        if (state.MoveInput == 0)
        {
            // Separate decel for ground and air — matches Tarodev
            float decel = state.IsGrounded ? groundDeceleration : airDeceleration;
            state.Velocity.x = Mathf.MoveTowards(
                state.Velocity.x, 0, decel * Time.fixedDeltaTime
            );
        }
        else
        {
            state.Velocity.x = Mathf.MoveTowards(
                state.Velocity.x,
                state.MoveInput * maxSpeed,
                acceleration * Time.fixedDeltaTime
            );
        }

        _logger?.Movement($"Velocity.x: {state.Velocity.x:F2}");
    }

    #endregion

    #region Jump

    public void HandleJump(PlayerState state)
    {
        // Detect early jump release for variable height
        if (!state.EndedJumpEarly
            && !state.IsGrounded
            && !state.JumpHeld
            && state.Velocity.y > 0)
        {
            state.EndedJumpEarly = true;
            _logger?.Jump("Jump ended early");
        }

        if (!state.JumpToConsume && !HasBufferedJump(state)) return;

        if (state.IsGrounded || CanUseCoyote(state))
            ExecuteJump(state);

        state.JumpToConsume = false;
    }

    private bool HasBufferedJump(PlayerState state) =>
        state.BufferUsable &&
        state.Time < state.TimeJumpPressed + jumpBuffer;

    private bool CanUseCoyote(PlayerState state) =>
        state.CoyoteUsable &&
        !state.IsGrounded &&
        state.Time < state.FrameLeftGround + coyoteTime;

    private void ExecuteJump(PlayerState state)
    {
        state.EndedJumpEarly  = false;
        state.TimeJumpPressed = 0;
        state.BufferUsable    = false;
        state.CoyoteUsable    = false;
        state.Velocity.y      = jumpPower;
        _logger?.Jump($"Jump executed — power: {jumpPower}");
    }

    #endregion

    #region Gravity

    public void HandleGravity(PlayerState state)
    {
        // Skip on bounce frame
        if (state.Stomped) return;

        // Grounding force — pins player to ground and slopes
        if (state.IsGrounded && state.Velocity.y <= 0f)
        {
            state.Velocity.y = groundingForce;
            return;
        }

        // Matches Tarodev — early jump end multiplies gravity
        float inAirGravity = fallAcceleration;
        if (state.EndedJumpEarly && state.Velocity.y > 0)
            inAirGravity = jumpEndEarlyGravity;

        // MoveTowards smoothly caps at maxFallSpeed
        state.Velocity.y = Mathf.MoveTowards(
            state.Velocity.y,
            -maxFallSpeed,
            inAirGravity * Time.fixedDeltaTime
        );

        _logger?.Gravity($"Gravity: {inAirGravity:F2} | Vy: {state.Velocity.y:F2}");
    }

    #endregion

    #region Bounce

    public void HandleBounce(PlayerState state)
    {
        if (!state.Stomped) return;

        state.Velocity.y = state.BounceForce;
        state.Stomped    = false;
        _logger?.Bounce($"Bounce applied — force: {state.BounceForce}");
    }

    #endregion

    #region Apply

    public void ApplyVelocity(PlayerState state)
    {
        _rb.linearVelocity = state.Velocity;
    }

    #endregion
}