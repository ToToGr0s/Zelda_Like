using UnityEngine;

public class EnemyKillable : MonoBehaviour
{
    private const float CoinScatterRange = 0.5f;

    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] [Range(0f, 1f)] private float dropChance = 1f;

    [Header("Coin Drop")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] [Range(0f, 1f)] private float coinDropChance = 0.5f;
    [SerializeField] private int minCoins = 1;
    [SerializeField] private int maxCoins = 3;

    private bool hasDropped;
    private bool isKilled;

    public event System.Action OnKilled;

    public void Kill()
    {
        if (isKilled)
            return;

        isKilled = true; // OPTIMIZED: prevent duplicate drops and events if Kill is triggered multiple times in one frame.
        Drop();
        DropCoins();
        OnKilled?.Invoke();
        Destroy(gameObject);
    }

    private void Drop()
    {
        if (hasDropped || dropPrefab == null || Random.value > dropChance)
            return;

        Instantiate(dropPrefab, GetSpawnPosition(), Quaternion.identity);
        hasDropped = true;
    }

    private void DropCoins()
    {
        if (coinPrefab == null || Random.value > coinDropChance)
            return;

        int coinCount = Random.Range(minCoins, maxCoins + 1);
        Vector3 spawnPosition = GetSpawnPosition();

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-CoinScatterRange, CoinScatterRange), 0f, Random.Range(-CoinScatterRange, CoinScatterRange));
            Instantiate(coinPrefab, spawnPosition + offset, Quaternion.identity);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        return dropPoint != null ? dropPoint.position : transform.position;
    }
}
