using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    private const string DefaultPlayerTag = "Player";
    private static readonly Vector3[] BurstDirections = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

    [Header("Stats")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private string playerTag = DefaultPlayerTag;

    [Header("Attack - Charge")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 0.8f;
    [SerializeField] private int chargeDamage = 2;

    [Header("Attack - Shoot")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int projectilesPerBurst = 4;

    [Header("Attack - Slam")]
    [SerializeField] private float slamRadius = 3f;
    [SerializeField] private int slamDamage = 1;
    [SerializeField] private GameObject slamEffect;

    [Header("Timings")]
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float enrageHealthPercent = 0.3f;
    [SerializeField] private float enrageSpeedMultiplier = 1.5f;

    [Header("UI")]
    [SerializeField] private BossHealthUI healthUI;

    private int currentHealth;
    private float baseAttackInterval;
    private Transform target;
    private bool isDead;
    private bool isCharging;
    private bool isEnraged;
    private Rigidbody rb;
    private SoundManager soundManager;

    public event System.Action OnBossDefeated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        soundManager = SoundManager.Instance;
        currentHealth = maxHealth;
        baseAttackInterval = attackInterval;

        if (healthUI == null)
            healthUI = FindFirstObjectByType<BossHealthUI>(); // OPTIMIZED: resolve UI once and cache it.
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            target = playerObject.transform;

        soundManager?.PlayMusic(soundManager.bossMusic);
        soundManager?.PlaySFX(soundManager.bossRoar);
        healthUI?.Init(maxHealth);

        StartCoroutine(AttackLoop());
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        healthUI?.UpdateHealth(currentHealth);

        if (!isEnraged && currentHealth <= maxHealth * enrageHealthPercent)
        {
            isEnraged = true;
            attackInterval = baseAttackInterval * 0.5f;
        }

        if (currentHealth > 0)
            return;

        currentHealth = 0;
        Die();
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        SetLinearVelocity(Vector3.zero); // OPTIMIZED: keep Rigidbody motion handling consistent.

        soundManager?.PlaySFX(soundManager.enemyDeath);
        OnBossDefeated?.Invoke();
        soundManager?.PlayMusic(soundManager.dungeonMusic);
        Destroy(gameObject, 2f);
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            if (target == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            float speedMult = isEnraged ? enrageSpeedMultiplier : 1f;

            switch (Random.Range(0, 3))
            {
                case 0:
                    yield return StartCoroutine(ChargeAttack(speedMult));
                    break;
                case 1:
                    yield return StartCoroutine(ShootAttack());
                    break;
                default:
                    yield return StartCoroutine(SlamAttack());
                    break;
            }

            yield return new WaitForSeconds(attackInterval / speedMult);
        }
    }

    private IEnumerator ChargeAttack(float speedMult)
    {
        if (target == null)
            yield break;

        isCharging = true;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        float elapsed = 0f;
        Vector3 chargeVelocity = direction * chargeSpeed * speedMult;

        while (elapsed < chargeDuration)
        {
            SetLinearVelocity(chargeVelocity); // OPTIMIZED: avoid mixing transform movement and Rigidbody velocity handling.
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetLinearVelocity(Vector3.zero);
        isCharging = false;
    }

    private IEnumerator ShootAttack()
    {
        if (projectilePrefab != null&& firePoint != null)
        {
            int projectileCount = Mathf.Min(projectilesPerBurst, BurstDirections.Length);

            for (int i = 0; i < projectileCount; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

                if (enemyProjectile != null)
                    enemyProjectile.SetDirection(BurstDirections[i]);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator SlamAttack()
    {
        yield return new WaitForSeconds(0.5f);

        if (slamEffect != null)
            Instantiate(slamEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, slamRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(playerTag) || !TryGetPlayerHealth(hit, out PlayerHealth playerHealth))
                continue;

            playerHealth.TakeDamage(slamDamage);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCharging || !collision.gameObject.CompareTag(playerTag) || !TryGetPlayerHealth(collision.transform, out PlayerHealth playerHealth))
            return;

        playerHealth.TakeDamage(chargeDamage);
    }

    private void SetLinearVelocity(Vector3 velocity)
    {
        if (rb != null)
            rb.linearVelocity = velocity;
    }

    private static bool TryGetPlayerHealth(Component source, out PlayerHealth playerHealth)
    {
        playerHealth = source.GetComponent<PlayerHealth>();
        return playerHealth != null;
    }
}
