using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string EnemyTag = "Enemy";
    private const string WallTag = "Wall";

    [SerializeField] private float lifeTime = 2f;

    private Vector3 moveDirection;
    private float speed;
    private int damage = 1;

    public void Init(Vector3 direction, float projectileSpeed, int projectileDamage = 1)
    {
        moveDirection = direction.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag) || other.transform.root.CompareTag(PlayerTag))
            return;

        if (other.CompareTag(EnemyTag))
        {
            EnemyKillable enemy = other.GetComponentInParent<EnemyKillable>();

            if (enemy != null)
            {
                enemy.Kill();
                SoundManager.Instance?.PlaySFX(SoundManager.Instance.enemyDeath);
            }

            BossDamageable boss = other.GetComponentInParent<BossDamageable>();

            if (boss != null)
                boss.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag(WallTag))
            Destroy(gameObject);
    }
}
