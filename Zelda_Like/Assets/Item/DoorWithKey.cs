using UnityEngine;

public class DoorWithKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        if (!inventory.UseKey())
            return;

        gameObject.SetActive(false);
    }
}