using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MerchantItem
{
    public string itemName;
    public int cost;
    public MerchantItemType type;
    public float bonusValue;
    public float bonusDuration;
    public Sprite icon;
}

public enum MerchantItemType
{
    Heal,
    SpeedBoost,
    DamageBoost,
    Shield,
    MaxHealthUp,
    Regeneration,
    Invincibility,
    Berserk,
    Revive
}

public class MerchantNPC : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string ShopUIPrefabPath = "MerchantShopUI";

    [SerializeField] private MerchantItem[] shopItems;
    [SerializeField] private GameObject shopUIPrefab;
    [SerializeField] private Canvas parentCanvas;

    private GameObject shopUI;
    private PlayerInventory playerInventory;
    private PlayerHealth playerHealth;
    private PlayerBonusManager playerBonusManager;
    private bool playerInRange;

    private void Start()
    {
        if (shopUIPrefab == null)
            shopUIPrefab = Resources.Load<GameObject>(ShopUIPrefabPath);

        if (shopUIPrefab == null)
        {
            Debug.LogWarning($"[MerchantNPC] Aucun shopUIPrefab assigne et aucun prefab dans Resources/{ShopUIPrefabPath}.", this);
            return;
        }

        Canvas canvas = parentCanvas != null ? parentCanvas : GetOrCreateCanvas();

        shopUI = Instantiate(shopUIPrefab, canvas.transform);
        shopUI.SetActive(false);

        MerchantShopUI shopScript = shopUI.GetComponent<MerchantShopUI>();
        if (shopScript != null)
            shopScript.SetMerchant(this);
    }

    private void OnDestroy()
    {
        if (shopUI != null)
            Destroy(shopUI);
    }

    private Canvas GetOrCreateCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas != null)
            return canvas;

        GameObject canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        // Search up the hierarchy: works whether the trigger is on root or on a child collider.
        playerInventory = other.GetComponent<PlayerInventory>() ?? other.GetComponentInParent<PlayerInventory>();
        playerHealth = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
        playerBonusManager = other.GetComponent<PlayerBonusManager>() ?? other.GetComponentInParent<PlayerBonusManager>();
        playerInRange = true;

        if (shopUI != null)
            shopUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        playerInRange = false;
        playerInventory = null;
        playerHealth = null;
        playerBonusManager = null;

        if (shopUI != null)
            shopUI.SetActive(false);
    }

    public MerchantItem[] GetShopItems()
    {
        return shopItems;
    }

    public bool TryPurchase(int itemIndex)
    {
        if (!playerInRange || playerInventory == null)
            return false;

        if (itemIndex < 0 || itemIndex >= shopItems.Length)
            return false;

        MerchantItem item = shopItems[itemIndex];

        if (!playerInventory.SpendCoins(item.cost))
            return false;

        ApplyItem(item);
        SoundManager.Instance?.PlaySFX(SoundManager.Instance.purchase);
        return true;
    }

    private void ApplyItem(MerchantItem item)
    {
        switch (item.type)
        {
            case MerchantItemType.Heal:
                playerHealth?.Heal(Mathf.RoundToInt(item.bonusValue));
                break;

            case MerchantItemType.SpeedBoost:
                playerBonusManager?.ApplySpeedBoost(item.bonusValue, item.bonusDuration);
                break;

            case MerchantItemType.DamageBoost:
                playerBonusManager?.ApplyDamageBoost(item.bonusValue, item.bonusDuration);
                break;

            case MerchantItemType.Shield:
                playerBonusManager?.ApplyShield(item.bonusDuration);
                break;

            case MerchantItemType.MaxHealthUp:
                playerHealth?.IncreaseMaxHealth(Mathf.RoundToInt(item.bonusValue));
                break;

            case MerchantItemType.Regeneration:
                playerBonusManager?.ApplyRegeneration(item.bonusValue, item.bonusDuration);
                break;

            case MerchantItemType.Invincibility:
                playerBonusManager?.ApplyInvincibility(item.bonusDuration);
                break;

            case MerchantItemType.Berserk:
                playerBonusManager?.ApplyBerserk(item.bonusValue, item.bonusDuration);
                break;

            case MerchantItemType.Revive:
                playerBonusManager?.ApplyRevive();
                break;
        }
    }
}
