using UnityEngine;

/// <summary>
/// Creates a background sprite with gradient and optional pattern
/// </summary>
public class BackgroundGenerator : MonoBehaviour
{
    [Header("Background Settings")]
    public int width = 1920;
    public int height = 1080;
    
    [Header("Colors")]
    public Color topColor = new Color(0.1f, 0.1f, 0.3f);
    public Color bottomColor = new Color(0.05f, 0.05f, 0.15f);
    
    [Header("Pattern")]
    public bool addPattern = true;
    public float patternOpacity = 0.05f;
    public int patternSize = 50;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        GenerateBackground();
    }

    [ContextMenu("Generate Background")]
    public void GenerateBackground()
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                float t = (float)y / height;
                
                // Base gradient
                Color baseColor = Color.Lerp(bottomColor, topColor, t);

                // Add pattern
                if (addPattern)
                {
                    bool isPattern = ((x / patternSize) + (y / patternSize)) % 2 == 0;
                    if (isPattern)
                    {
                        baseColor = Color.Lerp(baseColor, Color.white, patternOpacity);
                    }
                }

                // Add subtle vignette
                float centerX = width / 2f;
                float centerY = height / 2f;
                float maxDist = Mathf.Sqrt(centerX * centerX + centerY * centerY);
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                float vignette = dist / maxDist;
                baseColor = Color.Lerp(baseColor, Color.black, vignette * 0.3f);

                pixels[index] = baseColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100);

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = -100;
        }
    }

    /// <summary>
    /// Creates a parallax-ready layered background
    /// </summary>
    public static Sprite CreateParallaxLayer(int width, int height, Color color, float noiseScale)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                float noise = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                
                if (noise > 0.6f)
                {
                    float alpha = (noise - 0.6f) / 0.4f;
                    pixels[index] = new Color(color.r, color.g, color.b, alpha * 0.5f);
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100);
    }
}
