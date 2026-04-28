using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private void Die()
    {
        isDead = true;

        if (playerInput != null)
            playerInput.DeactivateInput();

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}