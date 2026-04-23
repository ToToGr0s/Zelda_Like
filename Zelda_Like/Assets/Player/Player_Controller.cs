using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementController movement;
    public PlayerInventory inventory;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        inventory = GetComponent<PlayerInventory>();
    }

    public void ToggleModule(MonoBehaviour module, bool state)
    {
        if (module != null)
            module.enabled = state;
    }
}