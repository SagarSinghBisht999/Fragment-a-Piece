using UnityEngine;
using UnityEngine.Events;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }
    
    public int TotalScore { get; private set; }

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;   // passes new total score

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        TotalScore += amount;
        OnScoreChanged?.Invoke(TotalScore);
    }
}