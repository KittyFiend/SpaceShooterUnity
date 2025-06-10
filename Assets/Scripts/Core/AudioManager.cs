using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip powerupSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip crystalPickupSound;

    [Header("Background Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private float musicFadeTime = 1.0f;

    [Header("Volume Settings")]
    [SerializeField] private float sfxVolume = 1.0f;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("Audio Quality Settings")]
    [SerializeField] private bool highQualityMode = true;

    private AudioSource sfxAudioSource;
    private AudioSource musicAudioSource;
    private AudioClip currentMusic;

    private bool soundEnabled = true;
    private bool musicEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAudioSettings();
            SetupAudioSources();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAudioSettings()
    {
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        string qualityMode = PlayerPrefs.GetString("AudioQuality", "HIGH");
        highQualityMode = qualityMode == "HIGH";
    }

    private void SetupAudioSources()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.volume = soundEnabled ? sfxVolume : 0f;

        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.volume = musicEnabled ? musicVolume : 0f;

        ApplyAudioQuality();
    }

    private void ApplyAudioQuality()
    {
        if (highQualityMode)
        {
            if (sfxAudioSource != null)
            {
                sfxAudioSource.pitch = 1.0f;
            }

            if (musicAudioSource != null)
            {
                musicAudioSource.pitch = 1.0f;
            }
        }
        else
        {
            if (sfxAudioSource != null)
            {
                sfxAudioSource.pitch = 0.95f;
            }

            if (musicAudioSource != null)
            {
                musicAudioSource.pitch = 0.95f;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            PlayMusic(menuMusic);
        }
        else if (scene.name == "GameScene" || scene.name == "Level1Scene")
        {
            PlayMusic(gameplayMusic);
        }
    }

    public void PlayShootSound()
    {
        PlaySound(shootSound);
    }

    public void PlayExplosionSound()
    {
        PlaySound(explosionSound);
    }

    public void PlayPlayerHitSound()
    {
        PlaySound(playerHitSound);
    }

    public void PlayPowerupSound()
    {
        PlaySound(powerupSound);
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);

        if (gameOverMusic != null)
        {
            PlayMusic(gameOverMusic);
        }
    }

    public void PlayCrystalPickupSound()
    {
        PlaySound(crystalPickupSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxAudioSource != null && soundEnabled)
        {
            sfxAudioSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || musicAudioSource == null || musicClip == currentMusic || !musicEnabled)
            return;

        currentMusic = musicClip;

        if (musicAudioSource.isPlaying)
        {
            StartCoroutine(FadeMusicTransition(musicClip));
        }
        else
        {
            musicAudioSource.clip = musicClip;
            musicAudioSource.volume = musicVolume;
            musicAudioSource.Play();
        }
    }

    private IEnumerator FadeMusicTransition(AudioClip newClip)
    {
        float timeElapsed = 0;
        float startVolume = musicAudioSource.volume;

        while (timeElapsed < musicFadeTime)
        {
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / musicFadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        musicAudioSource.clip = newClip;
        musicAudioSource.Play();

        timeElapsed = 0;
        while (timeElapsed < musicFadeTime)
        {
            musicAudioSource.volume = Mathf.Lerp(0, musicVolume, timeElapsed / musicFadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        musicAudioSource.volume = musicVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;

        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = soundEnabled ? volume : 0f;
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicEnabled ? volume : 0f;
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSoundEnabled(bool enabled)
    {
        soundEnabled = enabled;

        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = enabled ? sfxVolume : 0f;
        }
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;

        if (musicAudioSource != null)
        {
            if (enabled)
            {
                musicAudioSource.volume = musicVolume;
                if (!musicAudioSource.isPlaying && currentMusic != null)
                {
                    musicAudioSource.Play();
                }
            }
            else
            {
                musicAudioSource.volume = 0f;
            }
        }
    }

    public void SetAudioQuality(bool highQuality)
    {
        highQualityMode = highQuality;
        ApplyAudioQuality();

        PlayerPrefs.SetString("AudioQuality", highQuality ? "HIGH" : "LOW");
        PlayerPrefs.Save();
    }

    public bool IsSoundEnabled()
    {
        return soundEnabled;
    }

    public bool IsMusicEnabled()
    {
        return musicEnabled;
    }

    public bool IsHighQuality()
    {
        return highQualityMode;
    }

    public float GetSfxVolume()
    {
        return sfxVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}