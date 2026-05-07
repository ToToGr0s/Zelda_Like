using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private GameObject deathPanel;

    private int currentHealth;
    private bool isDead;
    private PlayerInput playerInput;

    private void Awake()
    {
        currentHealth = maxHealth;
        playerInput = GetComponent<PlayerInput>();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (playerInput != null)
            playerInput.ActivateInput();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void FullReset()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (playerInput != null)
            playerInput.ActivateInput();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void Die()
    {
        isDead = true;

        if (playerInput != null)
            playerInput.DeactivateInput();

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }
}