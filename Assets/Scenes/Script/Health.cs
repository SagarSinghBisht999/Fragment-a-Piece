using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _invincibilityDuration = 1f;


    [Header("Events")]
    public UnityEvent<int> onDamageTaken;
    public UnityEvent onDeath;
    public UnityEvent<int> onHealed;
   
    [field:SerializeField] public int CurrentHealth { get; private set; }
    public int MaxHealth => _maxHealth;
    public bool IsDead { get; private set; }
    public bool IsInvincible { get; private set; }

    private float _invincibilityTimer;

    private void Awake()
    {
        CurrentHealth = _maxHealth;
    }

    private void Update()
    {
        if (IsInvincible)
        {
            _invincibilityTimer -= Time.deltaTime;
            if (_invincibilityTimer <= 0f)
                IsInvincible = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || IsInvincible) return;

        CurrentHealth -= amount;
        onDamageTaken?.Invoke(amount);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            IsDead = true;
            onDeath?.Invoke();
        }
        else
        {
            IsInvincible = true;
            _invincibilityTimer = _invincibilityDuration;
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, _maxHealth);
        onHealed?.Invoke(amount);
    }

    public void Revive()
    {
        IsDead = false;
        CurrentHealth = _maxHealth;
    }
}