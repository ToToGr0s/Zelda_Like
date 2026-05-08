using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject bossHealthPanelPrefab;
    [SerializeField] private Canvas parentCanvas;

    [Header("References (auto-resolues si prefab assigne)")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject bossHealthPanel;

    private const string BossHealthPanelPrefabPath = "BossHealthUI";

    private void Start()
    {
        if (bossHealthPanelPrefab == null)
            bossHealthPanelPrefab = Resources.Load<GameObject>(BossHealthPanelPrefabPath);

        if (bossHealthPanelPrefab == null)
            return;

        Canvas canvas = parentCanvas != null ? parentCanvas : GetOrCreateCanvas();

        bossHealthPanel = Instantiate(bossHealthPanelPrefab, canvas.transform);
        bossHealthPanel.SetActive(false);
        healthSlider = bossHealthPanel.GetComponentInChildren<Slider>(true);
    }

    private Canvas GetOrCreateCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas != null)
            return canvas;

        GameObject canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    public void Init(int maxHealth)
    {
        if (bossHealthPanel != null)
            bossHealthPanel.SetActive(true);

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0 && bossHealthPanel != null)
            bossHealthPanel.SetActive(false);
    }
}
