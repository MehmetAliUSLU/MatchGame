using UnityEngine;
using System.Collections.Generic;

public class MatchFinder : MonoBehaviour
{
    public static MatchFinder Instance { get; private set; }

    [Header("Match Settings")]
    public int minMatchLength = 3;

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

    public List<Tile> FindAllMatches(Tile[,] tiles, int width, int height)
    {
        HashSet<Tile> allMatches = new HashSet<Tile>();

        // Find horizontal matches
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                var match = GetHorizontalMatch(tiles, x, y, width);
                if (match.Count >= minMatchLength)
                {
                    foreach (var tile in match)
                    {
                        allMatches.Add(tile);
                    }
                }
            }
        }

        // Find vertical matches
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                var match = GetVerticalMatch(tiles, x, y, height);
                if (match.Count >= minMatchLength)
                {
                    foreach (var tile in match)
                    {
                        allMatches.Add(tile);
                    }
                }
            }
        }

        return new List<Tile>(allMatches);
    }

    private List<Tile> GetHorizontalMatch(Tile[,] tiles, int startX, int y, int width)
    {
        List<Tile> match = new List<Tile>();

        if (tiles[startX, y] == null) return match;

        int tileType = tiles[startX, y].tileType;
        match.Add(tiles[startX, y]);

        for (int x = startX + 1; x < width; x++)
        {
            if (tiles[x, y] != null && tiles[x, y].tileType == tileType)
            {
                match.Add(tiles[x, y]);
            }
            else
            {
                break;
            }
        }

        return match.Count >= minMatchLength ? match : new List<Tile>();
    }

    private List<Tile> GetVerticalMatch(Tile[,] tiles, int x, int startY, int height)
    {
        List<Tile> match = new List<Tile>();

        if (tiles[x, startY] == null) return match;

        int tileType = tiles[x, startY].tileType;
        match.Add(tiles[x, startY]);

        for (int y = startY + 1; y < height; y++)
        {
            if (tiles[x, y] != null && tiles[x, y].tileType == tileType)
            {
                match.Add(tiles[x, y]);
            }
            else
            {
                break;
            }
        }

        return match.Count >= minMatchLength ? match : new List<Tile>();
    }

    // Check if a specific tile is part of a match
    public bool IsPartOfMatch(Tile[,] tiles, int x, int y, int width, int height)
    {
        if (tiles[x, y] == null) return false;

        int tileType = tiles[x, y].tileType;

        // Check horizontal
        int horizontalCount = 1;
        
        // Check left
        for (int i = x - 1; i >= 0; i--)
        {
            if (tiles[i, y] != null && tiles[i, y].tileType == tileType)
                horizontalCount++;
            else
                break;
        }
        
        // Check right
        for (int i = x + 1; i < width; i++)
        {
            if (tiles[i, y] != null && tiles[i, y].tileType == tileType)
                horizontalCount++;
            else
                break;
        }

        if (horizontalCount >= minMatchLength) return true;

        // Check vertical
        int verticalCount = 1;
        
        // Check down
        for (int i = y - 1; i >= 0; i--)
        {
            if (tiles[x, i] != null && tiles[x, i].tileType == tileType)
                verticalCount++;
            else
                break;
        }
        
        // Check up
        for (int i = y + 1; i < height; i++)
        {
            if (tiles[x, i] != null && tiles[x, i].tileType == tileType)
                verticalCount++;
            else
                break;
        }

        return verticalCount >= minMatchLength;
    }
}
