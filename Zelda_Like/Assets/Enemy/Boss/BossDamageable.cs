using UnityEngine;

public class BossDamageable : MonoBehaviour
{
    [SerializeField] private BossEnemy boss;

    private void Awake()
    {
        if (boss == null)
            boss = GetComponentInParent<BossEnemy>(); // OPTIMIZED: cache fallback reference once instead of resolving it repeatedly.
    }

    public void TakeDamage(int damage)
    {
        if (boss != null)
            boss.TakeDamage(damage);
    }
}
