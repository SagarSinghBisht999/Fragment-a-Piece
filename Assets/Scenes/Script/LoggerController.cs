using UnityEngine;

public class LoggerController : MonoBehaviour
{
    [Header("Log Category Toggles")]
    public bool input    = false;
    public bool ground   = false;
    public bool jump     = false;
    public bool gravity  = false;
    public bool bounce   = true;
    public bool movement = false;
    public bool enemy    = false;
    public bool health   = false;

    private void OnValidate()
    {
        if (GameLogger.Instance == null) return;

        GameLogger.Instance.LogInput    = input;
        GameLogger.Instance.LogGround   = ground;
        GameLogger.Instance.LogJump     = jump;
        GameLogger.Instance.LogGravity  = gravity;
        GameLogger.Instance.LogBounce   = bounce;
        GameLogger.Instance.LogMovement = movement;
        GameLogger.Instance.LogEnemy    = enemy;
        GameLogger.Instance.LogHealth   = health;
    }
} 