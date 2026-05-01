using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private string itemID = "Gun";
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Optional: add to inventory
        Inventory inv = other.GetComponent<Inventory>();
        if (inv != null)
            inv.AddItem(itemID, amount);

        // Enable the Shooter component (on the player root)
        Shooter shooter = other.GetComponent<Shooter>();
        if (shooter != null)
            shooter.enabled = true;

        // Activate the GunVisual child (direct child of player root)
        Transform gunVisual = other.transform.Find("GunPivot/GunVisual");   // correct path
        if (gunVisual != null)
            gunVisual.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}