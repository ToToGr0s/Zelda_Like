using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>(); // OPTIMIZED: resolve player health once when not assigned.

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (playerHealth == null || healthSlider == null)
            return;

        UpdateSlider(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateSlider;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateSlider;
    }

    private void UpdateSlider(int current, int max)
    {
        if (healthSlider == null)
            return;

        healthSlider.maxValue = max;
        healthSlider.value = current;
    }
}
