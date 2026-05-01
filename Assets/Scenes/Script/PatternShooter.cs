using System.Collections;
using UnityEngine;

public class PatternShooter : MonoBehaviour
{
    [Header("Pattern")]
    [SerializeField] private string _pattern = "001001";
    [SerializeField] private float _interval = 0.5f;
    [SerializeField] private bool _loop = true;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Vector2 _shootDirection = Vector2.down;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _projectileSpeed = 5f;
    [SerializeField] private Projectile.MovementType _movementType = Projectile.MovementType.Straight;
    [SerializeField] private float _projectileGravity = 0f;

    [Header("Timing")]
    [SerializeField] private float _startDelay = 0f;

    [Header("Trigger Zone")]
    [SerializeField] private bool _useTriggerZone = true;

    private Coroutine _activePattern;
    private GameLogger _logger;

    private void Awake()
    {
        _logger = GameLogger.Instance ?? new GameLogger();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_useTriggerZone) return;
        if (!other.CompareTag("Player")) return;

        if (_activePattern == null)
        {
            _logger?.Enemy("PatternShooter — player entered, starting pattern");
            _activePattern = StartCoroutine(RunPattern());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_useTriggerZone) return;
        if (!other.CompareTag("Player")) return;
        if (AnyPlayerInZone()) return;

        StopPattern();
        _logger?.Enemy("PatternShooter — player left, pattern stopped");
    }

    private bool AnyPlayerInZone()
    {
        Collider2D triggerCol = GetComponent<Collider2D>();
        if (triggerCol == null) return false;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Collider2D[] results = new Collider2D[8];
        int count = Physics2D.OverlapCollider(triggerCol, filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i].CompareTag("Player"))
                return true;
        }
        return false;
    }

    private IEnumerator RunPattern()
    {
        if (_startDelay > 0f)
            yield return new WaitForSeconds(_startDelay);

        while (true)
        {
            foreach (char c in _pattern)
            {
                if (c == '1')
                {
                    SpawnProjectile();
                    _logger?.Enemy($"PatternShooter — firing (direction: {_shootDirection})");
                }
                yield return new WaitForSeconds(_interval);
            }

            if (!_loop)
            {
                _logger?.Enemy("PatternShooter — pattern complete (non-looping)");
                _activePattern = null;
                yield break;
            }
        }
    }

    private void SpawnProjectile()
    {
        if (_projectilePrefab == null)
        {
            _logger?.Warning("PatternShooter — no projectile prefab assigned");
            return;
        }

        GameObject projObj = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(_shootDirection.normalized, _projectileSpeed, _projectileGravity,
                            _damage, 0, gameObject, _movementType);
        }
    }

    private void StopPattern()
    {
        if (_activePattern != null)
        {
            StopCoroutine(_activePattern);
            _activePattern = null;
        }
    }

    public void StartShooting()
    {
        if (_activePattern == null)
            _activePattern = StartCoroutine(RunPattern());
    }

    public void StopShooting()
    {
        StopPattern();
    }
}