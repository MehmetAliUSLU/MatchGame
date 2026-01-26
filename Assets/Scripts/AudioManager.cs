using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Sound Effects")]
    public AudioClip selectSound;
    public AudioClip swapSound;
    public AudioClip matchSound;
    public AudioClip noMatchSound;
    public AudioClip comboSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create audio sources if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
    }

    private void Start()
    {
        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;

        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnd += OnGameEnd;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnd -= OnGameEnd;
        }
    }

    public void PlaySelect()
    {
        PlaySFX(selectSound);
    }

    public void PlaySwap()
    {
        PlaySFX(swapSound);
    }

    public void PlayMatch()
    {
        PlaySFX(matchSound);
    }

    public void PlayNoMatch()
    {
        PlaySFX(noMatchSound);
    }

    public void PlayCombo(int comboLevel)
    {
        if (comboSound != null)
        {
            // Pitch up for higher combos
            sfxSource.pitch = 1f + (comboLevel * 0.1f);
            PlaySFX(comboSound);
            sfxSource.pitch = 1f;
        }
    }

    private void OnGameEnd(bool won)
    {
        if (won)
        {
            PlaySFX(winSound);
        }
        else
        {
            PlaySFX(loseSound);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void ToggleMute()
    {
        AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
    }
}
