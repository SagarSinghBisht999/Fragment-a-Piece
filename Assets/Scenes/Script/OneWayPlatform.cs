using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool _isOneWay = true;

    [Header("Who Can Use It")]
    [SerializeField] private LayerMask _affectedLayers;

    private Collider2D _platformCollider;

    private void Awake()
    {
        _platformCollider = GetComponent<Collider2D>();
        ApplyMode();
    }

    private void OnValidate()
    {
        if (_platformCollider == null)
            _platformCollider = GetComponent<Collider2D>();
        if (_platformCollider != null)
            ApplyMode();
    }

    private void ApplyMode()
    {
        _platformCollider.isTrigger = _isOneWay;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isOneWay) return;
        if (((1 << other.gameObject.layer) & _affectedLayers) == 0) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // Reliable check: object must be falling AND its centre is above the platform top.
        bool isFalling = rb.linearVelocity.y < 0f;
        float objectCenterY = other.bounds.center.y;
        float platformTop = _platformCollider.bounds.max.y;

        if (isFalling && objectCenterY > platformTop)
        {
            _platformCollider.isTrigger = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_isOneWay) return;
        if (((1 << other.gameObject.layer) & _affectedLayers) == 0) return;

        // When the character leaves the trigger area, make it pass-through again.
        _platformCollider.isTrigger = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!_isOneWay) return;
        if (((1 << collision.gameObject.layer) & _affectedLayers) == 0) return;

        // When the character leaves the trigger area, make it pass-through again.
        _platformCollider.isTrigger = true;
    }

    /// <summary>
    /// Switch mode at runtime. True = one-way, false = solid.
    /// </summary>
    public void SetOneWay(bool isOneWay)
    {
        _isOneWay = isOneWay;
        ApplyMode();
    }
}