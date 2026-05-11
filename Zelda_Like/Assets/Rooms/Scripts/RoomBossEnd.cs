using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach this to the RoomBossEnd prefab.
/// Spawns the boss when the player enters the trigger, then shows an end panel when the boss is defeated.
/// </summary>
public class RoomBossEnd : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI victoryText;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float endPanelDelay = 2f;

    private BossEnemy spawnedBoss;
    private bool bossSpawned;
    private bool bossDefeated;

    private void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bossSpawned || !other.CompareTag(playerTag))
            return;

        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("[RoomBossEnd] bossPrefab non assigné dans l'inspecteur!");
            return;
        }

        bossSpawned = true;

        Vector3 spawnPos = bossSpawnPoint != null
            ? bossSpawnPoint.position
            : transform.position + Vector3.forward * 3f;

        GameObject bossObj = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        spawnedBoss = bossObj.GetComponent<BossEnemy>();

        if (spawnedBoss != null)
            spawnedBoss.OnBossDefeated += HandleBossDefeated;
        else
            Debug.LogWarning("[RoomBossEnd] BossEnemy component non trouvé sur le bossPrefab!");
    }

    private void HandleBossDefeated()
    {
        if (bossDefeated)
            return;

        bossDefeated = true;

        if (spawnedBoss != null)
            spawnedBoss.OnBossDefeated -= HandleBossDefeated;

        StartCoroutine(ShowEndPanelAfterDelay(endPanelDelay));
    }

    private IEnumerator ShowEndPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Disable player input
        var playerInput = FindFirstObjectByType<UnityEngine.InputSystem.PlayerInput>();
        playerInput?.DeactivateInput();

        if (victoryText != null)
            victoryText.text = "VICTOIRE !";

        if (endPanel != null)
            endPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (spawnedBoss != null)
            spawnedBoss.OnBossDefeated -= HandleBossDefeated;
    }
}
