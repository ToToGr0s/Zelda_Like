using UnityEngine;

public class WeaponHUDManager : MonoBehaviour
{
    [SerializeField] private RectTransform overlay;
    [SerializeField] private RectTransform[] weaponSlots;
    private PlayerInventory inventory;

    private void Awake()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void SetSelectedWeapon(int index)
    {
        if (index < 0 || index >= weaponSlots.Length) return;
        overlay.position = weaponSlots[index].position;
    }
}