using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Match Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";

    [Header("Board Settings")]
    public int boardWidth = 8;
    public int boardHeight = 8;
    public int numberOfTileTypes = 5;

    [Header("Win Conditions")]
    public int targetScore = 1000;
    public float timeLimit = 60f;
    public int movesLimit = 30; // 0 = unlimited moves (time-based)

    [Header("Difficulty")]
    [Range(0f, 1f)]
    public float specialTileChance = 0.05f;
    public bool shuffleOnNoMoves = true;

    [Header("Rewards")]
    public int oneStarScore;
    public int twoStarScore;
    public int threeStarScore;
}
