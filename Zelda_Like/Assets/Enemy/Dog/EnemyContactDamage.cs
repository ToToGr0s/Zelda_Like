using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    private const string DefaultPlayerTag = "Player";

    [SerializeField] private string playerTag = DefaultPlayerTag;
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private float nextDamageTime;
    private Collider cachedPlayerCollider;
    private PlayerHealth cachedPlayerHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        cachedPlayerCollider = other;
        cachedPlayerHealth = other.GetComponent<PlayerHealth>(); // OPTIMIZED: cache the player health reference once per contact.
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag) || Time.time < nextDamageTime)
            return;

        if (cachedPlayerCollider != other)
        {
            cachedPlayerCollider = other;
            cachedPlayerHealth = other.GetComponent<PlayerHealth>(); // OPTIMIZED: refresh the cache only when the collider changes.
        }

        if (cachedPlayerHealth == null)
            return;

        cachedPlayerHealth.TakeDamage(damage);
        nextDamageTime = Time.time + damageCooldown;
    }

    private void OnTriggerExit(Collider other)
    {
        if (cachedPlayerCollider != other)
            return;

        cachedPlayerCollider = null;
        cachedPlayerHealth = null;
    }
}
