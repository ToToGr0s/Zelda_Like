using UnityEngine;

public class WeaponHUDManager : MonoBehaviour
{
    [SerializeField] private RectTransform overlay;
    [SerializeField] private RectTransform[] weaponSlots;
    [SerializeField] private PlayerInventory inventory;

    public void SetSelectedWeapon(int index)
    {
        if (index < 0 || index >= weaponSlots.Length) return;
        overlay.position = weaponSlots[index].position;
    }
}