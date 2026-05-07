using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFight : MonoBehaviour
{
    private PlayerInventory inventory;
    private WeaponHUDManager weaponHUD;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        weaponHUD = FindFirstObjectByType<WeaponHUDManager>();
    }

    private void OnAttack(InputValue value)
    {
        UseCurrentWeapon();
    }

    private void OnWeapon1(InputValue value)
    {
        inventory.EquipWeapon(0);
        weaponHUD?.SetSelectedWeapon(0);
    }

    private void OnWeapon2(InputValue value)
    {
        inventory.EquipWeapon(1);
        weaponHUD?.SetSelectedWeapon(1);
    }

    private void UseCurrentWeapon()
    {
        if (inventory == null) return;

        GameObject currentWeapon = inventory.GetCurrentWeapon();
        if (currentWeapon == null) return;

        MeleeWeapon meleeWeapon = currentWeapon.GetComponent<MeleeWeapon>();
        if (meleeWeapon != null)
        {
            meleeWeapon.Use();
            return;
        }

        RangedWeapon rangedWeapon = currentWeapon.GetComponent<RangedWeapon>();
        if (rangedWeapon != null)
            rangedWeapon.Use();
    }
}