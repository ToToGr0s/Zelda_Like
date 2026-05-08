using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Slider roomCountSlider;
    [SerializeField] private TextMeshProUGUI roomCountText;

    [Header("Settings")]
    [SerializeField] private int minRooms = 5;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private string gameSceneName = "Game";

    public static int SelectedRoomCount { get; private set; } = 10;

    private void Start()
    {
        if (roomCountSlider != null)
        {
            roomCountSlider.minValue = minRooms;
            roomCountSlider.maxValue = maxRooms;
            roomCountSlider.value = SelectedRoomCount;
            roomCountSlider.wholeNumbers = true;
            roomCountSlider.onValueChanged.AddListener(OnRoomCountChanged);
            UpdateRoomCountText();
        }

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        SoundManager.Instance?.PlayMusic(SoundManager.Instance.mainMenuMusic);
    }

    private void OnRoomCountChanged(float value)
    {
        SelectedRoomCount = Mathf.RoundToInt(value);
        UpdateRoomCountText();
    }

    private void UpdateRoomCountText()
    {
        if (roomCountText != null)
            roomCountText.text = "Rooms : " + SelectedRoomCount;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }
}
