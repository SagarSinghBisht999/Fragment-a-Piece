using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum MovementType { Straight, Arc }

    [Header("Settings (set by Initialize)")]
    private MovementType _movementType;
    private float _speed;
    private float _gravityScale;
     private int _damage;
     private int _pierceCount;
     [SerializeField]private float _lifetime = 10f;

    private Rigidbody2D _rb;
    private GameObject _shooter;
    private GameLogger _logger;
    private Vector2 _direction;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _logger = GameLogger.Instance ?? new GameLogger();
        Debug.Log($"Projectile spawned! Lifetime: {_lifetime}, Speed: {_speed}, Damage: {_damage}");
    }

    public void Initialize(Vector2 direction, float speed, float gravityScale, int damage,
                           int pierce, GameObject shooter, MovementType type)
    {
        _direction = direction.normalized;
        _speed = speed;
        _gravityScale = gravityScale;
        _damage = damage;
        _pierceCount = pierce;
        _shooter = shooter;
        _movementType = type;

        if (type == MovementType.Straight)
        {
            _rb.gravityScale = 0f;
            _rb.linearVelocity = _direction * _speed;
        }
        else
        {
            _rb.gravityScale = _gravityScale;
            _rb.linearVelocity = _direction * _speed;
        }

        Destroy(gameObject, _lifetime);
    }

    private void FixedUpdate()
    {
        if (_movementType == MovementType.Straight)
            _rb.linearVelocity = _direction * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _shooter) return;

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(_damage);
            _logger?.Enemy($"Projectile hit {other.name} for {_damage} damage");

            _pierceCount--;
            if (_pierceCount < 0)
            {
                _logger?.Enemy("Projectile destroyed");
                Destroy(gameObject);
            }
        }
    }
}