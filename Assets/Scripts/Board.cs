using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [Header("Board Settings")]
    public int width = 8;
    public int height = 8;
    public float tileSize = 1f;
    public float swapDuration = 0.2f;
    public float fallDuration = 0.3f;
    public float destroyDelay = 0.2f;

    [Header("Tile Prefabs")]
    public GameObject tilePrefab;
    public Sprite[] tileSprites; // Different colored tile sprites

    [Header("Tile Colors (if no sprites)")]
    public Color[] tileColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        new Color(1f, 0.5f, 0f), // Orange
        new Color(0.5f, 0f, 1f)  // Purple
    };

    private Tile[,] tiles;
    private Tile selectedTile;
    private bool isProcessing = false;
    private int activeTileTypes = 6;

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
        ApplyLevelSettings();
        InitializeBoard();
    }

    private void ApplyLevelSettings()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.currentLevelData != null)
        {
            var levelData = LevelManager.Instance.currentLevelData;
            width = levelData.boardWidth;
            height = levelData.boardHeight;
            activeTileTypes = levelData.numberOfTileTypes;
        }
    }

    private void InitializeBoard()
    {
        tiles = new Tile[width, height];

        // Center the board
        Vector3 startPos = transform.position - new Vector3((width - 1) * tileSize / 2f, (height - 1) * tileSize / 2f, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateTile(x, y, startPos);
            }
        }

        // Check for initial matches and replace them
        StartCoroutine(RemoveInitialMatches());
    }

    private void CreateTile(int x, int y, Vector3 startPos)
    {
        Vector3 pos = startPos + new Vector3(x * tileSize, y * tileSize, 0);
        GameObject tileObj;

        if (tilePrefab != null)
        {
            tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
        }
        else
        {
            tileObj = new GameObject($"Tile_{x}_{y}");
            tileObj.transform.position = pos;
            tileObj.transform.parent = transform;
            
            // Add sprite renderer
            var sr = tileObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateDefaultSprite();
            
            // Add collider for clicking
            var collider = tileObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(tileSize * 0.9f, tileSize * 0.9f);
        }

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null)
        {
            tile = tileObj.AddComponent<Tile>();
        }

        // Assign random type avoiding initial matches
        int tileType = GetRandomTypeWithoutMatch(x, y);
        tile.Initialize(x, y, tileType, GetTileVisual(tileType));
        tiles[x, y] = tile;
    }

    private int GetRandomTypeWithoutMatch(int x, int y)
    {
        List<int> possibleTypes = new List<int>();
        int typeCount = Mathf.Min(activeTileTypes, tileColors.Length);

        for (int i = 0; i < typeCount; i++)
        {
            possibleTypes.Add(i);
        }

        // Remove types that would create a horizontal match
        if (x >= 2)
        {
            if (tiles[x - 1, y] != null && tiles[x - 2, y] != null)
            {
                if (tiles[x - 1, y].tileType == tiles[x - 2, y].tileType)
                {
                    possibleTypes.Remove(tiles[x - 1, y].tileType);
                }
            }
        }

        // Remove types that would create a vertical match
        if (y >= 2)
        {
            if (tiles[x, y - 1] != null && tiles[x, y - 2] != null)
            {
                if (tiles[x, y - 1].tileType == tiles[x, y - 2].tileType)
                {
                    possibleTypes.Remove(tiles[x, y - 1].tileType);
                }
            }
        }

        if (possibleTypes.Count == 0)
        {
            return Random.Range(0, typeCount);
        }

        return possibleTypes[Random.Range(0, possibleTypes.Count)];
    }

    private object GetTileVisual(int tileType)
    {
        if (tileSprites != null && tileSprites.Length > tileType)
        {
            return tileSprites[tileType];
        }
        else if (tileColors.Length > tileType)
        {
            return tileColors[tileType];
        }
        return Color.white;
    }

    private Sprite CreateDefaultSprite()
    {
        // Create a simple square sprite
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        
        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
    }

    private IEnumerator RemoveInitialMatches()
    {
        yield return new WaitForSeconds(0.1f);
        
        var matches = MatchFinder.Instance?.FindAllMatches(tiles, width, height);
        while (matches != null && matches.Count > 0)
        {
            foreach (var tile in matches)
            {
                if (tile != null)
                {
                    int newType = GetRandomTypeWithoutMatch(tile.x, tile.y);
                    tile.SetType(newType, GetTileVisual(newType));
                }
            }
            matches = MatchFinder.Instance?.FindAllMatches(tiles, width, height);
        }
    }

    public void SelectTile(Tile tile)
    {
        if (isProcessing || GameManager.Instance.isGameOver) return;

        if (selectedTile == null)
        {
            selectedTile = tile;
            tile.Select();
        }
        else if (selectedTile == tile)
        {
            selectedTile.Deselect();
            selectedTile = null;
        }
        else if (IsAdjacent(selectedTile, tile))
        {
            StartCoroutine(TrySwapTiles(selectedTile, tile));
            selectedTile.Deselect();
            selectedTile = null;
        }
        else
        {
            selectedTile.Deselect();
            selectedTile = tile;
            tile.Select();
        }
    }

    private bool IsAdjacent(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private IEnumerator TrySwapTiles(Tile a, Tile b)
    {
        isProcessing = true;

        // Swap tiles
        yield return StartCoroutine(SwapTiles(a, b));

        // Check for matches
        var matches = MatchFinder.Instance.FindAllMatches(tiles, width, height);

        if (matches.Count > 0)
        {
            // Valid move - process matches
            yield return StartCoroutine(ProcessMatches());
        }
        else
        {
            // Invalid move - swap back
            yield return StartCoroutine(SwapTiles(a, b));
        }

        isProcessing = false;
    }

    private IEnumerator SwapTiles(Tile a, Tile b)
    {
        // Swap positions in array
        tiles[a.x, a.y] = b;
        tiles[b.x, b.y] = a;

        // Swap grid positions
        int tempX = a.x;
        int tempY = a.y;
        a.SetGridPosition(b.x, b.y);
        b.SetGridPosition(tempX, tempY);

        // Animate swap
        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        float elapsed = 0;
        while (elapsed < swapDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swapDuration;
            t = t * t * (3f - 2f * t); // Smooth step

            a.transform.position = Vector3.Lerp(posA, posB, t);
            b.transform.position = Vector3.Lerp(posB, posA, t);

            yield return null;
        }

        a.transform.position = posB;
        b.transform.position = posA;
    }

    private IEnumerator ProcessMatches()
    {
        var matches = MatchFinder.Instance.FindAllMatches(tiles, width, height);

        while (matches.Count > 0)
        {
            // Calculate score
            int score = matches.Count * 10;
            if (matches.Count >= 4) score += 20;
            if (matches.Count >= 5) score += 50;
            GameManager.Instance.AddScore(score);

            // Destroy matched tiles
            foreach (var tile in matches)
            {
                if (tile != null)
                {
                    tiles[tile.x, tile.y] = null;
                    tile.Destroy();
                }
            }

            yield return new WaitForSeconds(destroyDelay);

            // Drop tiles down
            yield return StartCoroutine(DropTiles());

            // Fill empty spaces
            yield return StartCoroutine(FillEmptySpaces());

            // Check for new matches
            matches = MatchFinder.Instance.FindAllMatches(tiles, width, height);
        }
    }

    private IEnumerator DropTiles()
    {
        bool tilesDropped = true;

        while (tilesDropped)
        {
            tilesDropped = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    if (tiles[x, y] == null)
                    {
                        // Find tile above
                        for (int aboveY = y + 1; aboveY < height; aboveY++)
                        {
                            if (tiles[x, aboveY] != null)
                            {
                                // Move tile down
                                Tile tile = tiles[x, aboveY];
                                tiles[x, y] = tile;
                                tiles[x, aboveY] = null;
                                tile.SetGridPosition(x, y);

                                // Animate fall
                                StartCoroutine(AnimateFall(tile, y));
                                tilesDropped = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (tilesDropped)
            {
                yield return new WaitForSeconds(fallDuration);
            }
        }
    }

    private IEnumerator AnimateFall(Tile tile, int targetY)
    {
        Vector3 startPos = tile.transform.position;
        Vector3 startBoardPos = transform.position - new Vector3((width - 1) * tileSize / 2f, (height - 1) * tileSize / 2f, 0);
        Vector3 endPos = startBoardPos + new Vector3(tile.x * tileSize, targetY * tileSize, 0);

        float elapsed = 0;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            t = t * t; // Ease in (gravity feel)

            tile.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        tile.transform.position = endPos;
    }

    private IEnumerator FillEmptySpaces()
    {
        Vector3 startBoardPos = transform.position - new Vector3((width - 1) * tileSize / 2f, (height - 1) * tileSize / 2f, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    // Create new tile above board
                    Vector3 spawnPos = startBoardPos + new Vector3(x * tileSize, (height + 1) * tileSize, 0);
                    Vector3 targetPos = startBoardPos + new Vector3(x * tileSize, y * tileSize, 0);

                    GameObject tileObj = new GameObject($"Tile_{x}_{y}");
                    tileObj.transform.position = spawnPos;
                    tileObj.transform.parent = transform;

                    var sr = tileObj.AddComponent<SpriteRenderer>();
                    sr.sprite = CreateDefaultSprite();

                    var collider = tileObj.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(tileSize * 0.9f, tileSize * 0.9f);

                    Tile tile = tileObj.AddComponent<Tile>();
                    int tileType = Random.Range(0, tileColors.Length);
                    tile.Initialize(x, y, tileType, GetTileVisual(tileType));
                    tiles[x, y] = tile;

                    // Animate fall
                    StartCoroutine(AnimateFallFromSpawn(tile, spawnPos, targetPos));
                }
            }
        }

        yield return new WaitForSeconds(fallDuration);
    }

    private IEnumerator AnimateFallFromSpawn(Tile tile, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            t = t * t;

            tile.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        tile.transform.position = endPos;
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y];
        }
        return null;
    }
}
