using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum FireMode { Raycast, Projectile }

    [Header("Fire Mode")]
    [SerializeField] private FireMode _mode = FireMode.Projectile;

    [Header("Fire Rate")]
    [SerializeField] private float _fireRate = 0.5f;

    [Header("Spread")]
    [SerializeField] private float _spreadAngle = 0f;
    [SerializeField] private int _bulletCount = 1;

    [Header("Damage")]
    [SerializeField] private int _damage = 1;

    [Header("Raycast (only used if mode = Raycast)")]
    [SerializeField] private float _range = 10f;
    [SerializeField] private LayerMask _hitLayers = -1;

    [Header("Projectile (only used if mode = Projectile)")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private float _projectileGravity = 0f;
    [SerializeField] private Projectile.MovementType _projectileMovement = Projectile.MovementType.Straight;

    private float _nextFireTime;
    private GameLogger _logger;

    private void Awake()
    {
        _logger = GameLogger.Instance ?? new GameLogger();
    }
    /// <summary>
/// Copies all settings from another Shooter.
/// Used when picking up a weapon — the gun's stats overwrite the player's.
/// </summary>
  public void CopyFrom(Shooter other)
   {
    _mode = other._mode;
    _fireRate = other._fireRate;
    _spreadAngle = other._spreadAngle;
    _bulletCount = other._bulletCount;
    _damage = other._damage;
    _range = other._range;
    _hitLayers = other._hitLayers;
    _projectilePrefab = other._projectilePrefab;
    _projectileSpeed = other._projectileSpeed;
    _projectileGravity = other._projectileGravity;
    _projectileMovement = other._projectileMovement;
   }

    public void Fire(Vector2 origin, Vector2 direction, GameObject shooter)
    {
        if (Time.time < _nextFireTime) return;
        _nextFireTime = Time.time + _fireRate;

        for (int i = 0; i < _bulletCount; i++)
        {
            Vector2 dir = direction;
            if (_bulletCount > 1)
            {
                float t = (float)i / (_bulletCount - 1);
                float angleOffset = Mathf.Lerp(-_spreadAngle / 2f, _spreadAngle / 2f, t);
                dir = Quaternion.Euler(0, 0, angleOffset) * direction;
            }

            if (_mode == FireMode.Raycast)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, dir, _range, _hitLayers);
                if (hit.collider != null)
                {
                    Health target = hit.collider.GetComponent<Health>();
                    if (target != null)
                    {
                        target.TakeDamage(_damage);
                        _logger?.Enemy($"Raycast hit {hit.collider.name} for {_damage}");
                    }
                }
                Debug.DrawRay(origin, dir * _range, Color.red, 0.1f);
            }
            else if (_mode == FireMode.Projectile && _projectilePrefab != null)
            {
                GameObject projObj = Instantiate(_projectilePrefab, origin, Quaternion.identity);
                Projectile proj = projObj.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Initialize(dir, _projectileSpeed, _projectileGravity,
                                    _damage, 0, shooter, _projectileMovement);
                }
                _logger?.Enemy($"Projectile fired — dir: {dir}");
            }
        }
    }
}