using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [Header("Particle Effects")]
    public ParticleSystem matchParticlePrefab;
    public ParticleSystem comboParticlePrefab;

    [Header("Settings")]
    public int particlePoolSize = 10;

    private ParticleSystem[] matchParticlePool;
    private int currentMatchParticle = 0;

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
        // Create particle pool
        if (matchParticlePrefab != null)
        {
            matchParticlePool = new ParticleSystem[particlePoolSize];
            for (int i = 0; i < particlePoolSize; i++)
            {
                matchParticlePool[i] = Instantiate(matchParticlePrefab, transform);
                matchParticlePool[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayMatchEffect(Vector3 position, Color color)
    {
        if (matchParticlePool == null || matchParticlePool.Length == 0)
        {
            // Create simple particle effect if no prefab
            CreateSimpleParticle(position, color);
            return;
        }

        var particle = matchParticlePool[currentMatchParticle];
        currentMatchParticle = (currentMatchParticle + 1) % particlePoolSize;

        particle.transform.position = position;
        
        // Set particle color
        var main = particle.main;
        main.startColor = color;

        particle.gameObject.SetActive(true);
        particle.Play();

        // Disable after playing
        StartCoroutine(DisableAfterPlay(particle));
    }

    private System.Collections.IEnumerator DisableAfterPlay(ParticleSystem particle)
    {
        yield return new WaitForSeconds(particle.main.duration + particle.main.startLifetime.constantMax);
        particle.gameObject.SetActive(false);
    }

    private void CreateSimpleParticle(Vector3 position, Color color)
    {
        // Create a simple visual feedback without particle system
        GameObject effect = new GameObject("MatchEffect");
        effect.transform.position = position;

        var sr = effect.AddComponent<SpriteRenderer>();
        sr.color = color;
        
        // Create a simple sprite
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; i++)
        {
            float dist = Vector2.Distance(new Vector2(i % 32, i / 32), new Vector2(16, 16));
            colors[i] = dist < 12 ? Color.white : Color.clear;
        }
        texture.SetPixels(colors);
        texture.Apply();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);

        StartCoroutine(AnimateAndDestroy(effect, color));
    }

    private System.Collections.IEnumerator AnimateAndDestroy(GameObject effect, Color color)
    {
        var sr = effect.GetComponent<SpriteRenderer>();
        float duration = 0.3f;
        float elapsed = 0;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            effect.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            sr.color = new Color(color.r, color.g, color.b, 1 - t);

            yield return null;
        }

        Destroy(effect);
    }

    public void PlayComboEffect(Vector3 position, int comboLevel)
    {
        if (comboParticlePrefab != null)
        {
            var particle = Instantiate(comboParticlePrefab, position, Quaternion.identity);
            
            // Scale up for higher combos
            particle.transform.localScale = Vector3.one * (1 + comboLevel * 0.2f);
            
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
    }
}
