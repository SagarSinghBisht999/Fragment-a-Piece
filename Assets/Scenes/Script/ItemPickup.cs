using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string itemID;
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Inventory inv = other.GetComponent<Inventory>();
        if (inv != null)
        {
            inv.AddItem(itemID, amount);
            Destroy(gameObject);
        }
    }
}