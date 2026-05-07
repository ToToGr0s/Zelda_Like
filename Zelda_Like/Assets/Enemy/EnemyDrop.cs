using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] [Range(0f, 1f)] private float dropChance = 1f;

    private bool hasDropped;

    public void Drop()
    {
        if (hasDropped)
            return;

        if (dropPrefab == null)
            return;

        if (Random.value > dropChance)
            return;

        Vector3 spawnPosition = dropPoint != null ? dropPoint.position : transform.position;

        Instantiate(dropPrefab, spawnPosition, Quaternion.identity);
        hasDropped = true;
    }
}