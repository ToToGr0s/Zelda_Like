using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float gizmoHeight = 0.2f;
    [SerializeField] private float gizmoThickness = 0.1f;
    [SerializeField] private Color shootFillColor = new Color(1f, 0f, 0f, 0.12f);
    [SerializeField] private Color shootWireColor = Color.red;

    private Transform target;
    private float nextShootTime;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            target = playerObject.transform;
    }

    private void Update()
    {
        if (target == null || projectilePrefab == null || firePoint == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer > shootRange)
            return;

        if (Time.time < nextShootTime)
            return;

        Shoot();
        nextShootTime = Time.time + fireRate;
    }

    private void Shoot()
    {
        Vector3 direction = GetCardinalDirection(firePoint.position, target.position);
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
            enemyProjectile.SetDirection(direction);
    }

    private Vector3 GetCardinalDirection(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        direction.y = 0f;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            return direction.x > 0f ? Vector3.right : Vector3.left;

        return direction.z > 0f ? Vector3.forward : Vector3.back;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + Vector3.up * gizmoHeight;
        Vector3 size = new Vector3(shootRange * 2f, gizmoThickness, shootRange * 2f);

        Gizmos.color = shootFillColor;
        Gizmos.DrawCube(center, size);

        Gizmos.color = shootWireColor;
        Gizmos.DrawWireCube(center, size);
    }
}