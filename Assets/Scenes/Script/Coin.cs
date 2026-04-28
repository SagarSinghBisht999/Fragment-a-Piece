using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int _scoreValue = 10;

    [Header("Feedback (Optional)")]
    [SerializeField] private GameObject _collectEffect;    // optional particle/sprite burst
    [SerializeField] private AudioClip _collectSound;      // optional sound

    [Header("Events — wire these to ScoreSystem")]
    public UnityEvent<int> OnCollected = new UnityEvent<int>();
    public UnityEvent OnSoundRequested = new UnityEvent();  // optional

    private bool _collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;

        // Award points through the event
        OnCollected?.Invoke(_scoreValue);

        // Optional feedback
        if (_collectEffect != null)
            Instantiate(_collectEffect, transform.position, Quaternion.identity);

        DisableCoin();
    }

    private void DisableCoin()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
}
