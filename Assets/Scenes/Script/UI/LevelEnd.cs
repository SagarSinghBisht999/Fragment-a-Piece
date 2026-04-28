using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("LEVEL COMPLETE");
            // For demo: just stop the game
            // Time.timeScale = 0f;
        }
    }
}
