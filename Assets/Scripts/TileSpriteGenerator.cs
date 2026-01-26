using UnityEngine;

/// <summary>
/// Creates tile sprites at runtime. Attach to an empty GameObject.
/// Run once in editor to generate sprites, or use at runtime.
/// </summary>
public class TileSpriteGenerator : MonoBehaviour
{
    [Header("Sprite Settings")]
    public int textureSize = 128;
    public float cornerRadius = 0.2f;

    [Header("Generated Sprites")]
    public Sprite[] generatedSprites;

    [Header("Tile Colors")]
    public TileColorData[] tileColors = new TileColorData[]
    {
        new TileColorData("Red", new Color(0.9f, 0.2f, 0.2f), new Color(0.7f, 0.1f, 0.1f)),
        new TileColorData("Blue", new Color(0.2f, 0.4f, 0.9f), new Color(0.1f, 0.2f, 0.7f)),
        new TileColorData("Green", new Color(0.2f, 0.8f, 0.3f), new Color(0.1f, 0.6f, 0.2f)),
        new TileColorData("Yellow", new Color(0.95f, 0.85f, 0.2f), new Color(0.8f, 0.7f, 0.1f)),
        new TileColorData("Purple", new Color(0.7f, 0.3f, 0.9f), new Color(0.5f, 0.2f, 0.7f)),
        new TileColorData("Orange", new Color(0.95f, 0.5f, 0.1f), new Color(0.8f, 0.4f, 0.05f)),
        new TileColorData("Cyan", new Color(0.2f, 0.9f, 0.9f), new Color(0.1f, 0.7f, 0.7f)),
        new TileColorData("Pink", new Color(0.95f, 0.4f, 0.7f), new Color(0.8f, 0.3f, 0.5f))
    };

    [System.Serializable]
    public class TileColorData
    {
        public string name;
        public Color mainColor;
        public Color shadowColor;

        public TileColorData(string n, Color main, Color shadow)
        {
            name = n;
            mainColor = main;
            shadowColor = shadow;
        }
    }

    [ContextMenu("Generate All Sprites")]
    public void GenerateAllSprites()
    {
        generatedSprites = new Sprite[tileColors.Length];

        for (int i = 0; i < tileColors.Length; i++)
        {
            generatedSprites[i] = GenerateTileSprite(tileColors[i]);
        }

        Debug.Log($"Generated {generatedSprites.Length} tile sprites!");
    }

    public Sprite GenerateTileSprite(TileColorData colorData)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Bilinear;

        Color[] pixels = new Color[textureSize * textureSize];
        float radius = textureSize * cornerRadius;
        float center = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int index = y * textureSize + x;
                
                // Check if inside rounded rectangle
                if (IsInsideRoundedRect(x, y, textureSize, textureSize, radius))
                {
                    // Create gradient effect
                    float gradient = (float)y / textureSize;
                    Color baseColor = Color.Lerp(colorData.shadowColor, colorData.mainColor, gradient);

                    // Add shine effect at top
                    float distFromTop = (float)y / textureSize;
                    if (distFromTop > 0.7f)
                    {
                        float shine = (distFromTop - 0.7f) / 0.3f * 0.3f;
                        baseColor = Color.Lerp(baseColor, Color.white, shine);
                    }

                    // Add inner glow
                    float distFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                    if (distFromCenter < 0.5f)
                    {
                        float glow = (0.5f - distFromCenter) / 0.5f * 0.2f;
                        baseColor = Color.Lerp(baseColor, Color.white, glow);
                    }

                    pixels[index] = baseColor;
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), 
            new Vector2(0.5f, 0.5f), textureSize);
    }

    private bool IsInsideRoundedRect(int x, int y, int width, int height, float radius)
    {
        int margin = 4; // Small margin for anti-aliasing
        int left = margin;
        int right = width - margin;
        int top = height - margin;
        int bottom = margin;

        // Check corners
        if (x < left + radius && y < bottom + radius)
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(left + radius, bottom + radius)) <= radius;
        }
        if (x > right - radius && y < bottom + radius)
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(right - radius, bottom + radius)) <= radius;
        }
        if (x < left + radius && y > top - radius)
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(left + radius, top - radius)) <= radius;
        }
        if (x > right - radius && y > top - radius)
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(right - radius, top - radius)) <= radius;
        }

        // Inside rectangle
        return x >= left && x <= right && y >= bottom && y <= top;
    }

    public Sprite GenerateIconSprite(string iconType, Color color)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        Color[] pixels = new Color[textureSize * textureSize];
        float center = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int index = y * textureSize + x;
                bool filled = false;

                switch (iconType.ToLower())
                {
                    case "circle":
                        filled = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) < center * 0.8f;
                        break;
                    case "diamond":
                        float dx = Mathf.Abs(x - center);
                        float dy = Mathf.Abs(y - center);
                        filled = (dx + dy) < center * 0.8f;
                        break;
                    case "star":
                        filled = IsInsideStar(x, y, center, 5, center * 0.8f, center * 0.4f);
                        break;
                    case "heart":
                        filled = IsInsideHeart(x, y, center, center * 0.7f);
                        break;
                    default:
                        filled = true;
                        break;
                }

                pixels[index] = filled ? color : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f), textureSize);
    }

    private bool IsInsideStar(int x, int y, float center, int points, float outerR, float innerR)
    {
        float dx = x - center;
        float dy = y - center;
        float angle = Mathf.Atan2(dy, dx);
        float dist = Mathf.Sqrt(dx * dx + dy * dy);

        float angleStep = Mathf.PI / points;
        float normalizedAngle = Mathf.Repeat(angle + Mathf.PI, angleStep * 2);
        
        float t = normalizedAngle / angleStep;
        float radius;
        if (t < 1)
            radius = Mathf.Lerp(outerR, innerR, t);
        else
            radius = Mathf.Lerp(innerR, outerR, t - 1);

        return dist < radius;
    }

    private bool IsInsideHeart(int x, int y, float center, float size)
    {
        float nx = (x - center) / size;
        float ny = (center - y) / size; // Flip Y

        // Heart equation
        float value = Mathf.Pow(nx * nx + ny * ny - 1, 3) - nx * nx * ny * ny * ny;
        return value < 0;
    }
}
