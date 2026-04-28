using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        if (ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.OnScoreChanged.AddListener(UpdateDisplay);
            UpdateDisplay(ScoreSystem.Instance.TotalScore);
        }
    }

    private void UpdateDisplay(int score)
    {
        _text.text = $"Score: {score}";
    }
}