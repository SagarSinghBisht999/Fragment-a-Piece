
// // -------------------------------------------------------
// // INPUT SYSTEM — Isolated
// // Only job: read keyboard, write results to PlayerState.
// // Receives logger via injection — never creates its own.
// // Knows nothing about movement, jump, or gravity.
// // -------------------------------------------------------
// using UnityEngine;

// public class PlayerInput : MonoBehaviour
// {
//     #region Settings

//     [Header("Input Keys")]
//     [SerializeField] private KeyCode leftKey  = KeyCode.A;
//     [SerializeField] private KeyCode rightKey = KeyCode.D;
//     [SerializeField] private KeyCode jumpKey  = KeyCode.Space;

//     [Header("Interaction")]
//     [SerializeField] private KeyCode interactKey = KeyCode.E;


//     #endregion

//     #region Private

//     // Logger injected by Brain — Input doesn't create it
//     // This is dependency injection from the video
//     private GameLogger _logger;

//     #endregion

//     #region Injection — Brain calls this once on startup

//     public void Initialize(GameLogger logger)
//     {
//         _logger = logger;
//     }

//     #endregion

//     #region Tick — Brain calls this every Update

//     public void Tick(PlayerState state)
//     {
//         ReadHorizontal(state);
//         ReadJump(state);
//     }

//     #endregion

//     #region Horizontal

//     private void ReadHorizontal(PlayerState state)
//     {
//         state.MoveInput = 0f;
//         if (Input.GetKey(rightKey)) state.MoveInput =  1f;
//         if (Input.GetKey(leftKey))  state.MoveInput = -1f;

//         _logger?.Movement($"MoveInput: {state.MoveInput}");
//     }

//     #endregion

//     #region Jump Input

//     private void ReadJump(PlayerState state)
//     {
//         state.JumpReleased = false;

//         if (Input.GetKeyDown(jumpKey))
//         {
//             state.JumpToConsume   = true;
//             state.TimeJumpPressed = state.Time; // precise timestamp
//             _logger?.Input("Jump pressed — buffer started");
//         }

//         if (Input.GetKeyUp(jumpKey))
//         {
//             state.JumpReleased = true;
//             _logger?.Input("Jump released");
//         }

//         state.JumpHeld        = Input.GetKey(jumpKey);
  
//     }

//     #endregion

//     #region Interaction Input

//     /// <summary>
//     /// Returns true the exact frame the interact key was pressed.
//     /// Used by PlayerBrain to trigger Interactable objects.
//     /// </summary>
//     public bool GetInteractPressed()
//     {
//         return Input.GetKeyDown(interactKey);
//     }

//     #endregion
// }
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Settings

    [Header("Movement Keys")]
    [SerializeField] private KeyCode leftKey  = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode upKey    = KeyCode.W;
    [SerializeField] private KeyCode downKey  = KeyCode.S;
    [SerializeField] private KeyCode jumpKey  = KeyCode.Space;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Combat")]
    [SerializeField] private KeyCode fireKey = KeyCode.F;

    #endregion

    #region Private

    private GameLogger _logger;
    private Vector2 _aimDirection = Vector2.right;

    #endregion

    #region Injection

    public void Initialize(GameLogger logger)
    {
        _logger = logger;
    }

    #endregion

    #region Tick

    public void Tick(PlayerState state)
    {
        ReadHorizontal(state);
        ReadAimDirection();
        ReadJump(state);
    }

    #endregion

    #region Horizontal Movement

    private void ReadHorizontal(PlayerState state)
    {
        state.MoveInput = 0f;
        if (Input.GetKey(rightKey)) state.MoveInput =  1f;
        if (Input.GetKey(leftKey))  state.MoveInput = -1f;

        _logger?.Movement($"MoveInput: {state.MoveInput}");
    }

    #endregion

    #region Aim Direction

    private void ReadAimDirection()
    {
        Vector2 input = Vector2.zero;
        if (Input.GetKey(rightKey)) input.x += 1;
        if (Input.GetKey(leftKey))  input.x -= 1;
        if (Input.GetKey(upKey))    input.y += 1;
        if (Input.GetKey(downKey))  input.y -= 1;

        if (input != Vector2.zero)
            _aimDirection = input.normalized;
    }

    public Vector2 GetAimDirection()
    {
        return _aimDirection;
    }

    #endregion

    #region Jump Input

    private void ReadJump(PlayerState state)
    {
        state.JumpReleased = false;

        if (Input.GetKeyDown(jumpKey))
        {
            state.JumpToConsume   = true;
            state.TimeJumpPressed = state.Time;
            _logger?.Input("Jump pressed — buffer started");
        }

        if (Input.GetKeyUp(jumpKey))
        {
            state.JumpReleased = true;
            _logger?.Input("Jump released");
        }

        state.JumpHeld = Input.GetKey(jumpKey);
    }

    #endregion

    #region Interaction & Fire Input

    public bool GetInteractPressed()
    {
        return Input.GetKeyDown(interactKey);
    }

    public bool GetFireHeld()
    {
        return Input.GetKey(fireKey);
    }

    #endregion
}