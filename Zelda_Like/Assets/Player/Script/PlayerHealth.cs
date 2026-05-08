using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private GameObject deathPanel;

    private int currentHealth;
    private bool isDead;
    private PlayerInput playerInput;
    private PlayerBonusManager bonusManager;

    public event System.Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        playerInput = GetComponent<PlayerInput>();
        bonusManager = GetComponent<PlayerBonusManager>(); // OPTIMIZED: cache bonus manager instead of resolving it on every hit.

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (bonusManager != null && bonusManager.IsInvincible)
            return;

        if (bonusManager != null && bonusManager.HasShield)
        {
            bonusManager.ConsumeShield();
            return;
        }

        currentHealth -= damage;
        SoundManager.Instance?.PlaySFX(SoundManager.Instance.playerHit);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        ResetState(false);
    }

    public void FullReset()
    {
        ResetState(true);
    }

    private void Die()
    {
        if (bonusManager != null && bonusManager.HasRevive)
        {
            bonusManager.ConsumeRevive();
            currentHealth = 1;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            return;
        }

        isDead = true;
        playerInput?.DeactivateInput();

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    private void ResetState(bool notify)
    {
        currentHealth = maxHealth;
        isDead = false;
        playerInput?.ActivateInput();

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (notify)
            OnHealthChanged?.Invoke(currentHealth, maxHealth); // OPTIMIZED: shared reset path removes duplicate state restoration logic.
    }
}
