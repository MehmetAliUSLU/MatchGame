using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int targetScore = 1000;
    public float gameTime = 60f;

    [Header("Current State")]
    public int currentScore = 0;
    public float remainingTime;
    public bool isGameOver = false;
    public bool isGameWon = false;

    public event System.Action<int> OnScoreChanged;
    public event System.Action<float> OnTimeChanged;
    public event System.Action<bool> OnGameEnd; // true = won, false = lost

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        remainingTime = gameTime;
        currentScore = 0;
    }

    private void Update()
    {
        if (isGameOver) return;

        remainingTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(remainingTime);

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(false);
        }
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);

        if (currentScore >= targetScore)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool won)
    {
        isGameOver = true;
        isGameWon = won;
        
        // Save level completion
        if (won && LevelManager.Instance != null)
        {
            LevelManager.Instance.CompleteLevel(currentScore);
        }
        
        OnGameEnd?.Invoke(won);
        Debug.Log(won ? "You Won!" : "Game Over!");
    }

    public void NextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
