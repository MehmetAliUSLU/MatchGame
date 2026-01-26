using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject levelSelectPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings")]
    public Slider sfxSlider;
    public Slider musicSlider;
    public Button backFromSettingsButton;

    [Header("Level Select")]
    public Button[] levelButtons;
    public Button backFromLevelsButton;

    [Header("Scene Names")]
    public string gameSceneName = "GameScene";

    private void Start()
    {
        // Setup main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // Setup settings
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(OnBackClicked);

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // Setup level select
        if (backFromLevelsButton != null)
            backFromLevelsButton.onClick.AddListener(OnBackClicked);

        SetupLevelButtons();

        // Show main panel
        ShowPanel(mainPanel);
    }

    private void SetupLevelButtons()
    {
        if (levelButtons == null) return;

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            Button btn = levelButtons[i];

            if (btn != null)
            {
                // Lock/unlock levels
                btn.interactable = levelIndex <= unlockedLevel;
                
                btn.onClick.AddListener(() => LoadLevel(levelIndex));

                // Update button text
                var text = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = levelIndex.ToString();
                }
            }
        }
    }

    private void OnPlayClicked()
    {
        // Go directly to game or show level select
        if (levelSelectPanel != null)
        {
            ShowPanel(levelSelectPanel);
        }
        else
        {
            LoadLevel(1);
        }
    }

    private void OnSettingsClicked()
    {
        ShowPanel(settingsPanel);
    }

    private void OnBackClicked()
    {
        ShowPanel(mainPanel);
    }

    private void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void LoadLevel(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
        SceneManager.LoadScene(gameSceneName);
    }

    private void ShowPanel(GameObject panel)
    {
        if (mainPanel != null) mainPanel.SetActive(panel == mainPanel);
        if (settingsPanel != null) settingsPanel.SetActive(panel == settingsPanel);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(panel == levelSelectPanel);
    }

    public void UnlockNextLevel()
    {
        int current = PlayerPrefs.GetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("UnlockedLevel", current + 1);
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }
}
