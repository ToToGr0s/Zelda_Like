using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>(); // OPTIMIZED: resolve the player inventory once when not assigned.

        if (coinText == null)
            coinText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
            playerInventory.OnCoinsChanged += UpdateCoinText;
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.OnCoinsChanged -= UpdateCoinText;
    }

    private void Start()
    {
        UpdateCoinText(playerInventory != null ? playerInventory.GetCoins() : 0);
    }

    private void UpdateCoinText(int coins)
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}
