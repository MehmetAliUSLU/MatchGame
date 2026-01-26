using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Data")]
    public LevelData[] allLevels;
    public LevelData currentLevelData;

    private int currentLevelIndex;

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

        // Load level data immediately in Awake so Board can read it in Start
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 1) - 1;
        if (currentLevelIndex < 0) currentLevelIndex = 0;
        
        if (allLevels != null && allLevels.Length > currentLevelIndex)
        {
            currentLevelData = allLevels[currentLevelIndex];
        }
    }

    private void Start()
    {
        ApplyLevelSettings();
    }

    private void ApplyLevelSettings()
    {
        if (currentLevelData == null) return;

        // Apply to Board
        if (Board.Instance != null)
        {
            Board.Instance.width = currentLevelData.boardWidth;
            Board.Instance.height = currentLevelData.boardHeight;
        }

        // Apply to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.targetScore = currentLevelData.targetScore;
            GameManager.Instance.gameTime = currentLevelData.timeLimit;
        }
    }

    public int GetStars(int score)
    {
        if (currentLevelData == null) return 0;

        if (score >= currentLevelData.threeStarScore) return 3;
        if (score >= currentLevelData.twoStarScore) return 2;
        if (score >= currentLevelData.oneStarScore) return 1;
        return 0;
    }

    public void CompleteLevel(int score)
    {
        int stars = GetStars(score);
        
        // Save best score
        string key = $"Level{currentLevelIndex + 1}_BestScore";
        int bestScore = PlayerPrefs.GetInt(key, 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(key, score);
        }

        // Save best stars
        string starsKey = $"Level{currentLevelIndex + 1}_Stars";
        int bestStars = PlayerPrefs.GetInt(starsKey, 0);
        if (stars > bestStars)
        {
            PlayerPrefs.SetInt(starsKey, stars);
        }

        // Unlock next level
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (currentLevelIndex + 2 > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 2);
        }

        PlayerPrefs.Save();
    }
}
