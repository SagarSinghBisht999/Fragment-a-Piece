using UnityEngine;

public class PlayerState
{
    #region Time
    public float Time;
    #endregion

    #region Input
    public float MoveInput;
    public bool  JumpToConsume;
    public bool  JumpHeld;
    public bool  JumpReleased;
    #endregion

    #region Ground
    public bool  IsGrounded;
    public bool  CoyoteUsable;
    public bool  BufferUsable;
    public float FrameLeftGround;
    #endregion

    #region Jump
    public bool  EndedJumpEarly;
    public float TimeJumpPressed;
    #endregion

    #region Physics
    public Vector2 Velocity;
    #endregion

    #region Bounce
    public bool  Stomped;
    public float BounceForce;
    #endregion

    #region Health
    public int   MaxHealth      = 3;
    public int   CurrentHealth  = 3;
    public bool  TakingDamage;
    public int   DamageAmount;
    public bool  IsInvincible;
    public float InvincibleTimer;
    public bool  IsDead;

    
    #endregion
}