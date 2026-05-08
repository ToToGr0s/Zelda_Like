using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantShopUI : MonoBehaviour
{
    [SerializeField] private MerchantNPC merchant;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemButtonPrefab;

    public void SetMerchant(MerchantNPC newMerchant)
    {
        merchant = newMerchant;
    }

    private void Awake()
    {
        if (merchant == null)
            merchant = GetComponentInParent<MerchantNPC>();
    }

    private void Start()
    {
        PopulateShop();
    }

    private void OnEnable()
    {
        PopulateShop();
    }

    private void PopulateShop()
    {
        if (itemContainer == null || shopItemButtonPrefab == null)
            return;

        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        if (merchant == null)
            return;

        MerchantItem[] items = merchant.GetShopItems();

        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            GameObject buttonObj = Instantiate(shopItemButtonPrefab, itemContainer);
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(true);

            if (text != null)
                text.text = items[i].itemName + " - " + items[i].cost + " coins";

            Button button = buttonObj.GetComponent<Button>();

            if (button != null)
                button.onClick.AddListener(() => merchant.TryPurchase(index));
        }
    }
}
