using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates UI sprites at runtime for buttons, panels, etc.
/// </summary>
public class UIAssetGenerator : MonoBehaviour
{
    public static UIAssetGenerator Instance { get; private set; }

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

    /// <summary>
    /// Creates a rounded rectangle sprite for UI elements
    /// </summary>
    public Sprite CreateRoundedRect(int width, int height, int cornerRadius, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (IsInsideRoundedRect(x, y, width, height, cornerRadius))
                {
                    pixels[index] = color;
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Use 9-slice borders
        int border = cornerRadius;
        return Sprite.Create(texture, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect,
            new Vector4(border, border, border, border));
    }

    /// <summary>
    /// Creates a gradient panel sprite
    /// </summary>
    public Sprite CreateGradientPanel(int width, int height, Color topColor, Color bottomColor, int cornerRadius = 20)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (IsInsideRoundedRect(x, y, width, height, cornerRadius))
                {
                    float t = (float)y / height;
                    pixels[index] = Color.Lerp(bottomColor, topColor, t);
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        int border = cornerRadius;
        return Sprite.Create(texture, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect,
            new Vector4(border, border, border, border));
    }

    /// <summary>
    /// Creates a button sprite with 3D effect
    /// </summary>
    public Sprite CreateButton(int width, int height, Color color, int cornerRadius = 15)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        Color lightColor = Color.Lerp(color, Color.white, 0.3f);
        Color darkColor = Color.Lerp(color, Color.black, 0.3f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (IsInsideRoundedRect(x, y, width, height, cornerRadius))
                {
                    // Create 3D button effect
                    float normalizedY = (float)y / height;
                    Color pixelColor;

                    if (normalizedY > 0.5f)
                    {
                        // Top half - lighter
                        float t = (normalizedY - 0.5f) * 2f;
                        pixelColor = Color.Lerp(color, lightColor, t * 0.5f);
                    }
                    else
                    {
                        // Bottom half - darker
                        float t = (0.5f - normalizedY) * 2f;
                        pixelColor = Color.Lerp(color, darkColor, t * 0.3f);
                    }

                    pixels[index] = pixelColor;
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        int border = cornerRadius;
        return Sprite.Create(texture, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect,
            new Vector4(border, border, border, border));
    }

    /// <summary>
    /// Creates a star sprite for ratings
    /// </summary>
    public Sprite CreateStar(int size, Color color, bool filled = true)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        float center = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                bool inside = IsInsideStar(x, y, center, 5, center * 0.9f, center * 0.4f);

                if (filled)
                {
                    pixels[index] = inside ? color : Color.clear;
                }
                else
                {
                    // Outline only
                    bool innerCheck = IsInsideStar(x, y, center, 5, center * 0.8f, center * 0.35f);
                    pixels[index] = (inside && !innerCheck) ? color : Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f), size);
    }

    private bool IsInsideRoundedRect(int x, int y, int width, int height, int radius)
    {
        // Check corners
        if (x < radius && y < radius)
            return Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) <= radius;
        if (x >= width - radius && y < radius)
            return Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)) <= radius;
        if (x < radius && y >= height - radius)
            return Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)) <= radius;
        if (x >= width - radius && y >= height - radius)
            return Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)) <= radius;

        return x >= 0 && x < width && y >= 0 && y < height;
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
        float radius = t < 1 
            ? Mathf.Lerp(outerR, innerR, t) 
            : Mathf.Lerp(innerR, outerR, t - 1);

        return dist < radius;
    }
}
