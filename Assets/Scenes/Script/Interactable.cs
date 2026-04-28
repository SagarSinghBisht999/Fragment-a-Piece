using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Activation Settings")]
    [SerializeField] private int requiredActivations = 1;   // how many times or players needed
    [SerializeField] private string requiredItemID = "";    // if set, player must have this item
    [SerializeField] private bool consumeItem = false;      // remove item on use
    [SerializeField] private float resetDelay = 0f;         // seconds before reset if not fully activated

    [Header("Event")]
    [SerializeField] private UnityEvent OnActivated;        // called when fully activated

    private int currentActivations;
    private HashSet<GameObject> interactedPlayers = new HashSet<GameObject>();
    private bool alreadyActivated;
    private Coroutine resetCoroutine;

    public void Interact(GameObject player)
    {
        if (alreadyActivated) return;
        if (interactedPlayers.Contains(player)) return;

        // Check item requirement
        if (!string.IsNullOrEmpty(requiredItemID))
        {
            Inventory inv = player.GetComponent<Inventory>();
            if (inv == null || !inv.HasItem(requiredItemID, 1)) return;
            if (consumeItem) inv.RemoveItem(requiredItemID, 1);
        }

        interactedPlayers.Add(player);
        currentActivations++;

        // Start reset timer if previous activation count was zero
        if (resetDelay > 0 && currentActivations == 1)
        {
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
            resetCoroutine = StartCoroutine(ResetTimer());
        }

        if (currentActivations >= requiredActivations)
        {
            alreadyActivated = true;
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
            OnActivated?.Invoke();
        }
    }

    private IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(resetDelay);
        ResetActivation();
    }

    private void ResetActivation()
    {
        currentActivations = 0;
        interactedPlayers.Clear();
        resetCoroutine = null;
    }
}