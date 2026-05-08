using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Key")]
    public bool hasKey;

    [Header("Coins")]
    [SerializeField] private int coins;

    public event System.Action<int> OnCoinsChanged;

    [Header("Weapons")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private int currentWeaponIndex;

    private GameObject currentWeaponInstance;

    private void Start()
    {
        SpawnCurrentWeapon();
    }

    public void EquipWeapon(int index)
    {
        if (weaponPrefabs == null || weaponPrefabs.Length == 0) return;
        if (index < 0 || index >= weaponPrefabs.Length) return;

        currentWeaponIndex = index;
        SpawnCurrentWeapon();
    }

    private void SpawnCurrentWeapon()
    {
        if (weaponHolder == null) return;
        if (weaponPrefabs == null || weaponPrefabs.Length == 0) return;

        if (currentWeaponInstance != null)
            Destroy(currentWeaponInstance);

        currentWeaponInstance = Instantiate(weaponPrefabs[currentWeaponIndex], weaponHolder);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeaponInstance;
    }

    public void AddKey()
    {
        hasKey = true;
    }

    public void RemoveKey()
    {
        hasKey = false;
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public bool UseKey()
    {
        if (!hasKey)
            return false;

        hasKey = false;
        return true;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount)
            return false;

        coins -= amount;
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    public int GetCoins()
    {
        return coins;
    }
}