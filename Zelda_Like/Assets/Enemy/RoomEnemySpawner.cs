using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnZone spawnZone;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform parentForSpawnedEnemies;
    [SerializeField] private RoomGridPathfinder roomGridPathfinder;
    [SerializeField] private Collider roomTrigger;

    [SerializeField, Range(3, 20)] private int minEnemyCount = 3;
    [SerializeField, Range(3, 20)] private int maxEnemyCount = 6;

    private void Awake()
    {
        if (spawnZone == null)
            spawnZone = GetComponent<EnemySpawnZone>(); // OPTIMIZED: cache local spawn zone fallback once.

        if (parentForSpawnedEnemies == null)
            parentForSpawnedEnemies = transform;

        if (roomGridPathfinder == null)
            roomGridPathfinder = GetComponentInParent<RoomGridPathfinder>();

        if (roomTrigger == null)
            roomTrigger = GetComponentInParent<Collider>();
    }

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (spawnZone == null || enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        int countToSpawn = Random.Range(minEnemyCount, maxEnemyCount + 1);
        int prefabCount = enemyPrefabs.Length;

        for (int i = 0; i < countToSpawn; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, prefabCount)];
            Vector3 spawnPosition = spawnZone.GetRandomPoint();
            GameObject enemyInstance = Instantiate(prefab, spawnPosition, Quaternion.identity, parentForSpawnedEnemies);
            EnemyGridMover enemyMover = enemyInstance.GetComponent<EnemyGridMover>();

            if (enemyMover != null)
                enemyMover.Init(roomGridPathfinder, roomTrigger);
        }
    }
}
