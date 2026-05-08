using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private const string PlayerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        inventory.AddKey();
        gameObject.SetActive(false);
    }
}
