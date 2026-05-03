using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _countdown = false;
    [SerializeField] private float _startTime = 60f;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI _timerText;

    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set; } = true;

    private void Start()
    {
        CurrentTime = _countdown ? _startTime : 0f;
    }

    private void Update()
    {
        if (!IsRunning) return;

        if (_countdown)
        {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                IsRunning = false;
                GameUIController.Instance?.ShowGameOver("Time's Up!");
            }
        }
        else
        {
            CurrentTime += Time.deltaTime;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_timerText == null) return;
        int minutes = Mathf.FloorToInt(CurrentTime / 60f);
        int seconds = Mathf.FloorToInt(CurrentTime % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer() => IsRunning = false;
    public void ResetTimer()
    {
        CurrentTime = _countdown ? _startTime : 0f;
        IsRunning = true;
    }
}