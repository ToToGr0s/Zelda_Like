using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        healthSlider.maxValue = playerHealth.GetMaxHealth();
        healthSlider.value = playerHealth.GetCurrentHealth();
    }

    private void Update()
    {
        healthSlider.value = playerHealth.GetCurrentHealth();
    }
}