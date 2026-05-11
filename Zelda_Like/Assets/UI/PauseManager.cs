using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the pause menu: ESC to pause/resume, volume sliders, key rebinding.
/// Place this on a persistent GameObject in the game scene (not DontDestroyOnLoad — one per scene).
/// Wire up the UI references in the Inspector.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Key Rebinding")]
    [Tooltip("PlayerInput component on the player GameObject")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private List<RebindActionUI> rebindButtons = new();

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private InputActionRebindingExtensions.RebindingOperation currentRebind;

    // PlayerPrefs keys
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey   = "SFXVolume";
    private const string BindingsKey     = "InputBindings";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        SetupSliders();
        SetupButtons();
        LoadBindings();
        InitRebindButtons();
    }

    private void SetupSliders()
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float savedSFX   = PlayerPrefs.GetFloat(SFXVolumeKey,   1f);

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = savedMusic;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = savedSFX;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        SoundManager.Instance?.SetMusicVolume(savedMusic);
        SoundManager.Instance?.SetSFXVolume(savedSFX);
    }

    private void SetupButtons()
    {
        if (resumeButton != null)    resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton != null)  mainMenuButton.onClick.AddListener(ReturnToMenu);
        if (quitButton != null)      quitButton.onClick.AddListener(QuitGame);
    }

    private void InitRebindButtons()
    {
        foreach (RebindActionUI rebind in rebindButtons)
            rebind.Init(this, playerInput);
    }

    private void Update()
    {
        // ESC only toggles pause when not actively rebinding a key.
        if (currentRebind == null && Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else          Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Disable gameplay input so the player doesn't move while the menu is open.
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        playerInput?.DeactivateInput();
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        playerInput?.ActivateInput();
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance?.SetMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance?.SetSFXVolume(value);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    public void ReturnToMenu()
    {
        Resume();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ─── Key Rebinding ───────────────────────────────────────────────────────

    /// <summary>Starts an interactive rebind for <paramref name="actionName"/>.</summary>
    public void StartRebind(string actionName, TextMeshProUGUI displayText)
    {
        if (playerInput == null)
        {
            Debug.LogWarning("[PauseManager] PlayerInput non assigné — rebind impossible.");
            return;
        }

        InputAction action = playerInput.actions.FindAction(actionName);

        if (action == null)
        {
            Debug.LogWarning($"[PauseManager] Action '{actionName}' introuvable dans le PlayerInput!");
            return;
        }

        // Cancel any ongoing rebind first.
        currentRebind?.Cancel();
        currentRebind?.Dispose();
        currentRebind = null;

        action.Disable();

        if (displayText != null)
            displayText.text = "Appuyez sur une touche...";

        currentRebind = action
            .PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                action.Enable();
                RefreshBindingDisplay(action, displayText);
                currentRebind.Dispose();
                currentRebind = null;
                SaveBindings();
            })
            .OnCancel(op =>
            {
                action.Enable();
                RefreshBindingDisplay(action, displayText);
                currentRebind.Dispose();
                currentRebind = null;
            })
            .Start();
    }

    /// <summary>Returns the human-readable name of the first binding for <paramref name="actionName"/>.</summary>
    public string GetBindingDisplay(string actionName)
    {
        if (playerInput == null) return "?";

        InputAction action = playerInput.actions.FindAction(actionName);

        if (action == null || action.bindings.Count == 0)
            return "?";

        return InputControlPath.ToHumanReadableString(
            action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private static void RefreshBindingDisplay(InputAction action, TextMeshProUGUI text)
    {
        if (text == null || action == null || action.bindings.Count == 0)
            return;

        text.text = InputControlPath.ToHumanReadableString(
            action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void SaveBindings()
    {
        if (playerInput == null) return;

        string json = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(BindingsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        if (playerInput == null) return;

        string json = PlayerPrefs.GetString(BindingsKey, string.Empty);

        if (!string.IsNullOrEmpty(json))
            playerInput.actions.LoadBindingOverridesFromJson(json);
    }

    private void OnDisable()
    {
        currentRebind?.Cancel();
        currentRebind?.Dispose();
        currentRebind = null;

        // Re-enable the action if it was left disabled by an interrupted rebind.
        // (PlayerInput will re-enable actions on its own next Enable cycle, so this is a safety net.)
    }

    private void OnDestroy()
    {
        currentRebind?.Cancel();
        currentRebind?.Dispose();
    }
}

// ─── RebindActionUI — serializable helper for each rebindable action ─────────

[System.Serializable]
public class RebindActionUI
{
    [Tooltip("Exact name of the InputAction (e.g. 'Move', 'Attack')")]
    public string actionName;

    [Tooltip("TMP label that shows the current key")]
    public TextMeshProUGUI displayText;

    [Tooltip("Button the player clicks to start rebinding")]
    public Button rebindButton;

    private PauseManager manager;

    public void Init(PauseManager pauseManager, PlayerInput input)
    {
        manager = pauseManager;

        // Show the current binding.
        if (displayText != null && input != null)
        {
            InputAction action = input.actions.FindAction(actionName);

            if (action != null && action.bindings.Count > 0)
            {
                displayText.text = InputControlPath.ToHumanReadableString(
                    action.bindings[0].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }

        if (rebindButton != null)
            rebindButton.onClick.AddListener(OnRebindButtonClicked);
    }

    private void OnRebindButtonClicked()
    {
        manager?.StartRebind(actionName, displayText);
    }
}
