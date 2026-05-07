using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float projectileSpawnOffset = 0.2f;
    [SerializeField] private float lastFireTime;
    [SerializeField] private float fireCooldown = 1f;

    [Header("Weapon Position")]
    [SerializeField] private Vector3 upPosition;
    [SerializeField] private Vector3 downPosition;
    [SerializeField] private Vector3 leftPosition;
    [SerializeField] private Vector3 rightPosition;

    [Header("Weapon Rotation")]
    [SerializeField] private Vector3 upRotation;
    [SerializeField] private Vector3 downRotation;
    [SerializeField] private Vector3 leftRotation;
    [SerializeField] private Vector3 rightRotation;

    private MovementController movementController;

    private void Awake()
    {
        movementController = GetComponentInParent<MovementController>();
    }

    private void Update()
    {
        UpdateWeaponTransform();
    }

    public void Use()
    {
        if (projectilePrefab == null) return;
        if (firePoint == null) return;
        if (movementController == null) return;

        if (Time.time < lastFireTime + fireCooldown) return;

        lastFireTime = Time.time;

        Vector3 direction = movementController.GetLastLookDirection();
        Vector3 spawnPosition = firePoint.position + direction * projectileSpawnOffset;

        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null) return;

        projectile.Init(direction, projectileSpeed);
    }

    private void UpdateWeaponTransform()
    {
        if (movementController == null) return;

        Vector3 direction = movementController.GetLastLookDirection();

        transform.localPosition = GetLocalPositionFromDirection(direction);
        transform.localRotation = Quaternion.Euler(GetLocalRotationFromDirection(direction));
    }

    private Vector3 GetLocalPositionFromDirection(Vector3 direction)
    {
        if (direction == Vector3.right) return rightPosition;
        if (direction == Vector3.left) return leftPosition;
        if (direction == Vector3.back) return downPosition;
        return upPosition;
    }

    private Vector3 GetLocalRotationFromDirection(Vector3 direction)
    {
        if (direction == Vector3.right) return rightRotation;
        if (direction == Vector3.left) return leftRotation;
        if (direction == Vector3.back) return downRotation;
        return upRotation;
    }
}