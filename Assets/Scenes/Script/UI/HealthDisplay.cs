using UnityEngine;
using TMPro;


public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Format")]
    [SerializeField] private string _label = "HP";
    [SerializeField] private bool _showMaxHealth = true;

    [Header("Events (Hook Later)")]
    public UnityEngine.Events.UnityEvent<int> OnHealthChanged;
    public UnityEngine.Events.UnityEvent OnDepleted;
    public UnityEngine.Events.UnityEvent OnRevived;

    private int _previousHealth;
    private bool _wasDead;

    private void Start()
    {
        if (_health == null || _text == null) return;

        _health.onDamageTaken.AddListener(_ => Refresh());
        _health.onHealed.AddListener(_ => Refresh());
        _health.onDeath.AddListener(HandleDeath);

        Refresh();
    }

    private void Refresh()
    {
        if (_health.IsDead)
        {
            _text.text = $"{_label}: DEAD";
            if (!_wasDead)
            {
                _wasDead = true;
                OnDepleted?.Invoke();
            }
            return;
        }

        _wasDead = false;
        int current = _health.CurrentHealth;
        int max = _health.MaxHealth;
        _text.text = _showMaxHealth ? $"{_label}: {current}/{max}" : $"{_label}: {current}";

        if (current != _previousHealth)
        {
            _previousHealth = current;
            OnHealthChanged?.Invoke(current);
        }
    }

    private void HandleDeath()
    {
        Refresh();
    }

    /// <summary>
    /// Call this from PlayerBrain.Revive() to reset the display.
    /// </summary>
    public void OnPlayerRevived()
    {
        _wasDead = false;
        Refresh();
        OnRevived?.Invoke();
    }
}