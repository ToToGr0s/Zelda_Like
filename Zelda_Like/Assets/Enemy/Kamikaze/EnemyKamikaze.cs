using UnityEngine;

public class EnemyKamikaze : MonoBehaviour
{
    private const string DefaultPlayerTag = "Player";

    [SerializeField] private string playerTag = DefaultPlayerTag;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float explosionRange = 1.2f;
    [SerializeField] private GameObject explosionEffect;

    private Transform target;
    private PlayerHealth targetHealth;
    private bool hasExploded;
    private float detectionRangeSqr;
    private float explosionRangeSqr;

    private void Awake()
    {
        CacheRanges();
    }

    private void OnValidate()
    {
        CacheRanges();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject == null)
            return;

        target = playerObject.transform;
        targetHealth = playerObject.GetComponent<PlayerHealth>(); // OPTIMIZED: cache the target health reference once.
    }

    private void Update()
    {
        if (hasExploded || target == null)
            return;

        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;
        float distanceSqr = directionToTarget.sqrMagnitude;

        if (distanceSqr > detectionRangeSqr)
            return;

        if (distanceSqr <= explosionRangeSqr)
        {
            Explode();
            return;
        }

        transform.position += directionToTarget.normalized * moveSpeed * Time.deltaTime;
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        if (targetHealth != null && !targetHealth.IsDead())
        {
            int currentHp = targetHealth.GetCurrentHealth();

            if (currentHp > 1)
                targetHealth.TakeDamage(currentHp - 1);
        }

        SoundManager.Instance?.PlaySFX(SoundManager.Instance.explosion);

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        EnemyKillable killable = GetComponent<EnemyKillable>();

        if (killable != null)
            killable.Kill();
        else
            Destroy(gameObject);
    }

    private void CacheRanges()
    {
        detectionRangeSqr = detectionRange * detectionRange;
        explosionRangeSqr = explosionRange * explosionRange;
    }
}
