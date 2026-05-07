using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private BoxCollider hitbox;
    [SerializeField] private Transform hitboxTransform;

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

    [Header("Hitbox Position")]
    [SerializeField] private Vector3 hitboxUpPosition;
    [SerializeField] private Vector3 hitboxDownPosition;
    [SerializeField] private Vector3 hitboxLeftPosition;
    [SerializeField] private Vector3 hitboxRightPosition;

    [Header("Hitbox Rotation")]
    [SerializeField] private Vector3 hitboxUpRotation;
    [SerializeField] private Vector3 hitboxDownRotation;
    [SerializeField] private Vector3 hitboxLeftRotation;
    [SerializeField] private Vector3 hitboxRightRotation;

    private MovementController movementController;

    private void Awake()
    {
        movementController = GetComponentInParent<MovementController>();

        if (hitboxTransform == null && hitbox != null)
            hitboxTransform = hitbox.transform;
    }

    private void Update()
    {
        UpdateWeaponTransform();
    }

    public void Use()
    {
        KillEnemiesInsideHitbox();
    }

    private void KillEnemiesInsideHitbox()
    {
        if (hitbox == null) return;
        if (hitboxTransform == null) return;

        Vector3 center = hitboxTransform.TransformPoint(hitbox.center);
        Vector3 halfExtents = Vector3.Scale(hitbox.size * 0.5f, hitboxTransform.lossyScale);
        Quaternion rotation = hitboxTransform.rotation;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider current = hits[i];

            if (current.transform.root.CompareTag("Player"))
                continue;

            EnemyKillable enemy = current.GetComponentInParent<EnemyKillable>();

            if (enemy != null)
                enemy.Kill();
        }
    }

    private void UpdateWeaponTransform()
    {
        if (movementController == null) return;

        Vector3 direction = movementController.GetLastLookDirection();

        transform.localPosition = GetWeaponLocalPosition(direction);
        transform.localRotation = Quaternion.Euler(GetWeaponLocalRotation(direction));

        if (hitboxTransform != null)
        {
            hitboxTransform.localPosition = GetHitboxLocalPosition(direction);
            hitboxTransform.localRotation = Quaternion.Euler(GetHitboxLocalRotation(direction));
        }
    }

    private Vector3 GetWeaponLocalPosition(Vector3 direction)
    {
        if (direction == Vector3.right) return rightPosition;
        if (direction == Vector3.left) return leftPosition;
        if (direction == Vector3.back) return downPosition;
        return upPosition;
    }

    private Vector3 GetWeaponLocalRotation(Vector3 direction)
    {
        if (direction == Vector3.right) return rightRotation;
        if (direction == Vector3.left) return leftRotation;
        if (direction == Vector3.back) return downRotation;
        return upRotation;
    }

    private Vector3 GetHitboxLocalPosition(Vector3 direction)
    {
        if (direction == Vector3.right) return hitboxRightPosition;
        if (direction == Vector3.left) return hitboxLeftPosition;
        if (direction == Vector3.back) return hitboxDownPosition;
        return hitboxUpPosition;
    }

    private Vector3 GetHitboxLocalRotation(Vector3 direction)
    {
        if (direction == Vector3.right) return hitboxRightRotation;
        if (direction == Vector3.left) return hitboxLeftRotation;
        if (direction == Vector3.back) return hitboxDownRotation;
        return hitboxUpRotation;
    }
}