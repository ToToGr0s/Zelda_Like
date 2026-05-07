using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private float nextDamageTime;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (Time.time < nextDamageTime)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(damage);
        nextDamageTime = Time.time + damageCooldown;
    }
}