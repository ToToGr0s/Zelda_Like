using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        if (playerHealth.IsDead())
            return;

        if (playerHealth.GetCurrentHealth() >= playerHealth.GetMaxHealth())
            return;

        playerHealth.Heal(healAmount);
        Destroy(gameObject);
    }
}