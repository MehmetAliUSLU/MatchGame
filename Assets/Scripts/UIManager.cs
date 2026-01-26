using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI levelText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button nextLevelButton;

    [Header("Settings")]
    public string scoreFormat = "Score: {0}";
    public string timeFormat = "Time: {0:F1}";
    public string targetFormat = "Target: {0}";
    public string levelFormat = "Level {0}";

    private void Start()
    {
        // Subscribe to events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnTimeChanged += UpdateTime;
            GameManager.Instance.OnGameEnd += ShowGameOver;

            // Initialize UI
            UpdateScore(0);
            UpdateTime(GameManager.Instance.gameTime);
            
            if (targetScoreText != null)
            {
                targetScoreText.text = string.Format(targetFormat, GameManager.Instance.targetScore);
            }
        }

        // Show current level
        if (levelText != null)
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            levelText.text = string.Format(levelFormat, currentLevel);
        }

        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        // Setup next level button
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            nextLevelButton.gameObject.SetActive(false);
        }

        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnTimeChanged -= UpdateTime;
            GameManager.Instance.OnGameEnd -= ShowGameOver;
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = string.Format(scoreFormat, score);
        }
    }

    private void UpdateTime(float time)
    {
        if (timeText != null)
        {
            timeText.text = string.Format(timeFormat, Mathf.Max(0, time));
            
            // Change color when low on time
            if (time <= 10f)
            {
                timeText.color = Color.red;
            }
            else
            {
                timeText.color = Color.white;
            }
        }
    }

    private void ShowGameOver(bool won)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            if (won)
            {
                int stars = LevelManager.Instance != null ? LevelManager.Instance.GetStars(GameManager.Instance.currentScore) : 0;
                string starText = new string('★', stars) + new string('☆', 3 - stars);
                gameOverText.text = "You Win!\n" + starText + "\nScore: " + GameManager.Instance.currentScore;
                gameOverText.color = Color.green;
                
                // Show next level button
                if (nextLevelButton != null)
                {
                    nextLevelButton.gameObject.SetActive(true);
                }
            }
            else
            {
                gameOverText.text = "Game Over!\nFinal Score: " + GameManager.Instance.currentScore;
                gameOverText.color = Color.red;
                
                // Hide next level button
                if (nextLevelButton != null)
                {
                    nextLevelButton.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnRestartClicked()
    {
        GameManager.Instance?.RestartGame();
    }

    private void OnNextLevelClicked()
    {
        GameManager.Instance?.NextLevel();
    }
}
