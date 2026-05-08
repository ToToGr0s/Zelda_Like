using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        inventory.AddCoins(coinValue);
        SoundManager.Instance?.PlaySFX(SoundManager.Instance.coinPickup);
        Destroy(gameObject);
    }
}
