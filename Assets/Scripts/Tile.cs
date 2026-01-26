using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int tileType;

    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    private Vector3 originalScale;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        originalScale = transform.localScale;
    }

    public void Initialize(int gridX, int gridY, int type, object visual)
    {
        x = gridX;
        y = gridY;
        tileType = type;
        gameObject.name = $"Tile_{x}_{y}";

        if (visual is Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.white;
        }
        else if (visual is Color color)
        {
            spriteRenderer.color = color;
        }

        originalColor = spriteRenderer.color;
    }

    public void SetType(int type, object visual)
    {
        tileType = type;

        if (visual is Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.white;
        }
        else if (visual is Color color)
        {
            spriteRenderer.color = color;
        }

        originalColor = spriteRenderer.color;
    }

    public void SetGridPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
        gameObject.name = $"Tile_{x}_{y}";
    }

    public void Select()
    {
        isSelected = true;
        transform.localScale = originalScale * 1.1f;
        spriteRenderer.color = originalColor * 1.2f;
    }

    public void Deselect()
    {
        isSelected = false;
        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;
    }

    public void Destroy()
    {
        // Add destruction effect here if desired
        // For now, simple scale down animation
        StartCoroutine(DestroyAnimation());
    }

    private System.Collections.IEnumerator DestroyAnimation()
    {
        float duration = 0.15f;
        float elapsed = 0;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (Board.Instance != null)
        {
            Board.Instance.SelectTile(this);
        }
    }

    private void OnMouseEnter()
    {
        if (!isSelected)
        {
            transform.localScale = originalScale * 1.05f;
        }
    }

    private void OnMouseExit()
    {
        if (!isSelected)
        {
            transform.localScale = originalScale;
        }
    }
}
