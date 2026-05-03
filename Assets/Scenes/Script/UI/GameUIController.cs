using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;  

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance;
    [Header("Player References")]
    [SerializeField] private PlayerBrain _player1;
    [SerializeField] private PlayerBrain _player2;
    [Header("Panels")]
    [SerializeField] private GameObject _gameplayPanel;
    [SerializeField] private GameObject _levelCompletePanel;
    [SerializeField] private GameObject _gameOverPanel;

    [Header("Level Complete")]
    [SerializeField] private TextMeshProUGUI _levelCompleteTimeText;
    [SerializeField] private Button _nextLevelButton;

    [Header("Game Over")]
    [SerializeField] private TextMeshProUGUI _gameOverReasonText;

    [Header("Timer")]
    [SerializeField] private Timer _timer;

    [Header("Restart")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private bool _showRestartButton = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        ShowGameplay();
        _restartButton.gameObject.SetActive(_showRestartButton);
        _restartButton.onClick.AddListener(RestartScene);
    }

    public void ShowGameplay()
    {
        _gameplayPanel.SetActive(true);
        _levelCompletePanel.SetActive(false);
        _gameOverPanel.SetActive(false);
    }

    public void ShowLevelComplete()
    {
        _timer.StopTimer();
        _gameplayPanel.SetActive(false);
        _levelCompletePanel.SetActive(true);

        if (_levelCompleteTimeText != null)
        {
            int m = Mathf.FloorToInt(_timer.CurrentTime / 60f);
            int s = Mathf.FloorToInt(_timer.CurrentTime % 60f);
            _levelCompleteTimeText.text = $"Time: {m:00}:{s:00}";
        }
    }

    public void ShowGameOver(string reason = "You Died!")
    {
        _timer.StopTimer();
        _gameplayPanel.SetActive(false);
        _gameOverPanel.SetActive(true);

        if (_gameOverReasonText != null)
            _gameOverReasonText.text = reason;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    /// <summary>
/// Called by PlayerBrain when any player dies. If all are dead, shows game over.
/// </summary>
public void CheckAllPlayersDead()
{
    bool p1Dead = _player1 == null || _player1.GetComponent<Health>().IsDead;
    bool p2Dead = _player2 == null || _player2.GetComponent<Health>().IsDead;

    if (p1Dead && p2Dead)
    {
        ShowGameOver("Both Players Died!");
    }
}
}