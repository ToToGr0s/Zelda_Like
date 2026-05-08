using UnityEngine;

public class RangedWeapon : PlayerWeapon
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
    private PlayerBonusManager bonusManager;
    private Vector3 lastAppliedDirection;
    private bool hasAppliedDirection;

    private void Awake()
    {
        movementController = GetComponentInParent<MovementController>();
        bonusManager = GetComponentInParent<PlayerBonusManager>();
    }

    private void Update()
    {
        UpdateWeaponTransform();
    }

    public override void Use()
    {
        if (projectilePrefab == null || firePoint == null || movementController == null)
            return;

        if (Time.time < lastFireTime + fireCooldown)
            return;

        lastFireTime = Time.time;

        int damage = Mathf.RoundToInt(bonusManager != null ? bonusManager.DamageMultiplier : 1f);
        Vector3 direction = movementController.GetLastLookDirection();
        Vector3 spawnPosition = firePoint.position + direction * projectileSpawnOffset;
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
            return;

        projectile.Init(direction, projectileSpeed, damage);
    }

    private void UpdateWeaponTransform()
    {
        if (movementController == null)
            return;

        Vector3 direction = movementController.GetLastLookDirection();

        if (hasAppliedDirection && direction == lastAppliedDirection)
            return;

        transform.localPosition = GetLocalPositionFromDirection(direction); // OPTIMIZED: only recalculate weapon placement when the look direction changes.
        transform.localRotation = Quaternion.Euler(GetLocalRotationFromDirection(direction));
        lastAppliedDirection = direction;
        hasAppliedDirection = true;
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
