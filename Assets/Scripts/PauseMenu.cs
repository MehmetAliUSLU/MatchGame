using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pause menu that can be used during gameplay
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button settingsButton;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Button closeSettingsButton;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        // Setup buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);

        // Setup sliders
        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }

        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        // Hide panels
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        PlayerPrefs.Save();
    }

    private void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnMusicChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }
}
