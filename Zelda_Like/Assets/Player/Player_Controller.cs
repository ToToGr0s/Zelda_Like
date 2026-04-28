using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementController movement;
    public PlayerInventory inventory;
    public PlayerHealth health;
    private void Awake()
    {
        movement = GetComponent<MovementController>();
        inventory = GetComponent<PlayerInventory>();
        health = GetComponent<PlayerHealth>();
    }

    public void ToggleModule(MonoBehaviour module, bool state)
    {
        if (module != null)
            module.enabled = state;
    }
}