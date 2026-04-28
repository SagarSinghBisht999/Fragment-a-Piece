using UnityEngine;

public class RevivalGhost : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _riseSpeed = 1.5f;
    [SerializeField] private float _lifetime = 8f;      // disappear after this many seconds

    private PlayerBrain _deadPlayer;
    private float _spawnTime;

    public void SetTarget(PlayerBrain player)
    {
        _deadPlayer = player;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        // Float upward
        transform.Translate(Vector2.up * _riseSpeed * Time.deltaTime);

        // Destroy if lifetime expires
        if (Time.time - _spawnTime > _lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerBrain alivePlayer = other.GetComponent<PlayerBrain>();
        if (alivePlayer == null || alivePlayer == _deadPlayer) return;

        // Revive the dead player at the ghost's current position
        _deadPlayer.Revive();
        _deadPlayer.transform.position = transform.position;

        Destroy(gameObject);
    }
}