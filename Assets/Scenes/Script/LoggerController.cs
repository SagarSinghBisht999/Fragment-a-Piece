using UnityEngine;

public class LoggerControl : MonoBehaviour
{
    [Header("Log Category Toggles")]
    public bool input    = true;
    public bool ground   = true;
    public bool jump     = true;
    public bool gravity  = false;
    public bool bounce   = true;
    public bool movement = false;
    public bool enemy    = true;
    public bool health   = true;

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