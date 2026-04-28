using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public string itemID;
        public int amount;
    }

    [SerializeField] private List<InventorySlot> items = new List<InventorySlot>();

    public event Action<string, int> OnInventoryChanged; // itemID, newTotalAmount

    public void AddItem(string itemID, int amount)
    {
        InventorySlot slot = items.Find(s => s.itemID == itemID);
        if (slot != null)
        {
            slot.amount += amount;
        }
        else
        {
            items.Add(new InventorySlot { itemID = itemID, amount = amount });
        }
        OnInventoryChanged?.Invoke(itemID, GetItemCount(itemID));
    }

    public bool RemoveItem(string itemID, int amount)
    {
        InventorySlot slot = items.Find(s => s.itemID == itemID);
        if (slot == null || slot.amount < amount) return false;

        slot.amount -= amount;
        if (slot.amount <= 0)
        {
            items.Remove(slot);
        }
        OnInventoryChanged?.Invoke(itemID, GetItemCount(itemID));
        return true;
    }

    public bool HasItem(string itemID, int minAmount = 1)
    {
        InventorySlot slot = items.Find(s => s.itemID == itemID);
        return slot != null && slot.amount >= minAmount;
    }

    public int GetItemCount(string itemID)
    {
        InventorySlot slot = items.Find(s => s.itemID == itemID);
        return slot != null ? slot.amount : 0;
    }
}