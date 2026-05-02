// -------------------------------------------------------
// CENTRALIZED LOGGER
// Single place to control ALL debug output in the game.
// Turn off categories here — nothing else needs to change.
// This is the architectural fix from the video.
// -------------------------------------------------------
using UnityEngine;
using System.Runtime.CompilerServices;

public class GameLogger
{
    public static GameLogger Instance { get; private set; }


    #region Category Toggles — turn off entire groups here

    public bool LogInput      = true;
    public bool LogGround     = true;
    public bool LogJump       = true;
    public bool LogGravity    = false;
    public bool LogBounce     = true;
    public bool LogMovement   = false;
    public bool LogEnemy      = true;
    public bool LogHealth     = true;
    public bool LogFromm     = true;
    #endregion

    public GameLogger()
    {
        Instance = this;
    }

    #region Log Methods — one per category
    public void LogFrom(string message, GameObject context = null,
                    [CallerFilePath] string scriptPath = "",
                    [CallerMemberName] string methodName = "")
   {
    if(!LogFromm) return; // example for movement category
    // Extract just the file name from the full path
    string scriptName = System.IO.Path.GetFileNameWithoutExtension(scriptPath);
    
    // Get the GameObject name (or "Unknown")
    string objectName = context != null ? context.name : "Unknown";
    
    // Output: [ScriptName] [GameObject] [Method] Message
    Debug.Log($"[{scriptName}] [{objectName}] [{methodName}] {message}");
  }

    public void Input(string message)
    {
        if (LogInput) Debug.Log($"[INPUT] {message}");
    }

    public void Ground(string message)
    {
        if (LogGround) Debug.Log($"[GROUND] {message}");
    }

    public void Jump(string message)
    {
        if (LogJump) Debug.Log($"[JUMP] {message}");
    }

    public void Gravity(string message)
    {
        if (LogGravity) Debug.Log($"[GRAVITY] {message}");
    }

    public void Bounce(string message)
    {
        if (LogBounce) Debug.Log($"[BOUNCE] {message}");
    }

  public void Health(string message, GameObject context = null)
  {
    if (!LogHealth) return;
    string name = context != null ? context.name : "Unknown";
    Debug.Log($"[HEALTH] [{name}] {message}");
   }
    public void Movement(string message)
    {
        if (LogMovement) Debug.Log($"[MOVEMENT] {message}");
    }

    public void Enemy(string message)
    {
        if (LogEnemy) Debug.Log($"[ENEMY] {message}");
    }

    public void Warning(string message)
    {
        Debug.LogWarning($"[WARNING] {message}");
    }

    public void Error(string message)
    {
        Debug.LogError($"[ERROR] {message}");
    }

    #endregion
}