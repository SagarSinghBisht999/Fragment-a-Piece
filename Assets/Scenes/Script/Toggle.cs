using UnityEngine;

public class Toggle : MonoBehaviour
{
    [SerializeField] private PlayerBrain _player1;
    [SerializeField] private PlayerBrain _player2;
    [SerializeField] private bool _playersCollide = true;

    private void OnValidate()
    {
        if (_player1 != null && _player2 != null)
            _player1.SetPlayerCollisionEnabled(_player2, _playersCollide);
    }
}