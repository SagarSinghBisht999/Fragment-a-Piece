using UnityEngine;

public class PLayersCameraFollow : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;

    [Header("Settings")]
    [SerializeField] private float _minSize = 5f;
    [SerializeField] private float _maxSize = 10f;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _padding = 3f;
    [SerializeField] private float _verticalOffset = 1f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        // Both players dead — do nothing
        if (_player1 == null && _player2 == null) return;

        // Only one player alive — follow that one
        if (_player1 == null || _player2 == null)
        {
            Transform target = _player1 != null ? _player1 : _player2;
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _minSize, Time.deltaTime * _smoothSpeed);

            Vector3 pos = new Vector3(target.position.x, target.position.y + _verticalOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _smoothSpeed);
            return;
        }

        // Both alive — follow midpoint
        Vector2 midpoint = (_player1.position + _player2.position) / 2f;
        float distance = Vector2.Distance(_player1.position, _player2.position);

        float targetSize = Mathf.Clamp(distance + _padding, _minSize, _maxSize);
        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetSize, Time.deltaTime * _smoothSpeed);

        Vector3 targetPosition = new Vector3(midpoint.x, midpoint.y + _verticalOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _smoothSpeed);
    }
}


// using UnityEngine;

// public class CameraFollow : MonoBehaviour
// {
//     [Header("Players")]
//     [SerializeField] private Transform _player1;
//     [SerializeField] private Transform _player2;

//     [Header("Level Boundaries")]
//     [SerializeField] private float _minX = -10f;
//     [SerializeField] private float _maxX = 10f;
//     [SerializeField] private float _minY = -5f;
//     [SerializeField] private float _maxY = 5f;

//     [Header("Smoothness")]
//     [SerializeField] private float _smoothSpeed = 5f;

//     private Camera _cam;

//     private void Awake()
//     {
//         _cam = GetComponent<Camera>();
//     }

//     private void LateUpdate()
//     {
//         if (_player1 == null && _player2 == null) return;

//         Vector2 midpoint = GetMidpoint();

//         float halfHeight = _cam.orthographicSize;
//         float halfWidth = halfHeight * _cam.aspect;

//         float targetX = Mathf.Clamp(midpoint.x, _minX + halfWidth, _maxX - halfWidth);
//         float targetY = Mathf.Clamp(midpoint.y, _minY + halfHeight, _maxY - halfHeight);

//         Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);

//         transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _smoothSpeed);
//     }

//     private Vector2 GetMidpoint()
//     {
//         if (_player1 == null && _player2 == null) return transform.position;
//         if (_player1 == null) return _player2.position;
//         if (_player2 == null) return _player1.position;

//         return (_player1.position + _player2.position) / 2f;
//     }
// }