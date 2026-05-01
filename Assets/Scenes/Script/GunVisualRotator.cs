// using UnityEngine;

// public class GunVisualRotator : MonoBehaviour
// {
//     [SerializeField] private PlayerInput _playerInput;

//     private void Update()
//     {
//         if (_playerInput == null) return;

//         Vector2 dir = _playerInput.GetAimDirection();
//         float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

//         transform.rotation = Quaternion.Euler(0f, 0f, angle);
//     }
// }
using UnityEngine;

public class GunVisualRotator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _gunDistance = 1.2f;

    private PlayerInput _playerInput;
    private Transform _gunVisual;

    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();

        if (transform.childCount > 0)
            _gunVisual = transform.GetChild(0);   // GunVisual

        ApplyDistance();
    }

    private void OnValidate()
    {
        if (transform.childCount > 0)
            transform.GetChild(0).localPosition = Vector2.right * _gunDistance;
    }

    private void ApplyDistance()
    {
        if (_gunVisual != null)
            _gunVisual.localPosition = Vector2.right * _gunDistance;
    }

    private void Update()
    {
        if (_playerInput == null) return;

        Vector2 dir = _playerInput.GetAimDirection();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}