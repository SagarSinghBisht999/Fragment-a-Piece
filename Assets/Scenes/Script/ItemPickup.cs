using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private string _itemID;           // unique ID for inventory/code (e.g., "RedKey")
    [SerializeField] private string _itemName;          // display name for UI (e.g., "Red Key")
    [SerializeField] private int _amount = 1;

    [Header("Weapon Pickup (optional)")]
    [SerializeField] private bool _isWeapon = false;

    [Header("Additional Actions")]
    [SerializeField] private UnityEvent _onPickup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Inventory inv = other.GetComponent<Inventory>();
        if (inv != null)
            inv.AddItem(_itemID, _amount);

        if (_isWeapon)
        {
            Shooter pickupShooter = GetComponent<Shooter>();
            Shooter playerShooter = other.GetComponent<Shooter>();

            if (pickupShooter != null && playerShooter != null)
            {
                playerShooter.CopyFrom(pickupShooter);
                playerShooter.enabled = true;
            }
        }

        _onPickup?.Invoke();
        Destroy(gameObject);
    }
}