using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    #region Settings

    [Header("Input Keys")]
    [SerializeField] private KeyCode leftKey  = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode jumpKey  = KeyCode.Space;

    #endregion

    #region Private

    private GameLogger _logger;

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
        ReadJump(state);
    }

    #endregion

    #region Horizontal

    private void ReadHorizontal(PlayerState state)
    {
        state.MoveInput = 0f;
        if (Input.GetKey(rightKey)) state.MoveInput =  1f;
        if (Input.GetKey(leftKey))  state.MoveInput = -1f;

        _logger?.Movement($"MoveInput: {state.MoveInput}");
    }

    #endregion

    #region Jump

    private void ReadJump(PlayerState state)
    {
        state.JumpReleased = false;

        if (Input.GetKeyDown(jumpKey))
        {
            state.JumpToConsume   = true;
            state.TimeJumpPressed = state.Time; // precise timestamp
            _logger?.Input("Jump pressed");
        }

        if (Input.GetKeyUp(jumpKey))
            state.JumpReleased = true;

        state.JumpHeld = Input.GetKey(jumpKey);
    }

    #endregion
}