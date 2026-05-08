using UnityEngine;

public class EnemySpawnZone : MonoBehaviour
{
    [SerializeField] private Vector3 zoneSize = new Vector3(10f, 1f, 10f);
    [SerializeField] private Vector3 zoneOffset = Vector3.zero;
    [SerializeField] private float spawnY = 0.5f;
    [SerializeField] private Color gizmoFillColor = new Color(1f, 0f, 0f, 0.2f);
    [SerializeField] private Color gizmoWireColor = Color.red;

    public Vector3 GetRandomPoint()
    {
        Vector3 center = GetZoneCenter();

        float randomX = Random.Range(-zoneSize.x * 0.5f, zoneSize.x * 0.5f);
        float randomZ = Random.Range(-zoneSize.z * 0.5f, zoneSize.z * 0.5f);

        return new Vector3(
            center.x + randomX,
            transform.position.y + spawnY,
            center.z + randomZ
        );
    }

    public Vector3 GetZoneCenter()
    {
        return transform.position + transform.TransformDirection(zoneOffset);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoFillColor;
        Gizmos.DrawCube(GetZoneCenter(), zoneSize);

        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireCube(GetZoneCenter(), zoneSize);
    }
}