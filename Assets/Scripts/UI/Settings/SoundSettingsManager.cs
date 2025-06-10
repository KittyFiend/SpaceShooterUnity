using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsManager : MonoBehaviour
{
    [Header("Sound Buttons")]
    [SerializeField] private Button soundButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button vibrationButton;

    [Header("Audio Quality Selector")]
    [SerializeField] private Button qualityDownButton;
    [SerializeField] private Button qualityUpButton;
    [SerializeField] private TMPro.TextMeshProUGUI qualityText;

    [Header("Button Images")]
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Image vibrationButtonImage;

    [Header("Sound Button Sprites")]
    [SerializeField] private Sprite soundEnabledSprite;
    [SerializeField] private Sprite soundDisabledSprite;

    [Header("Music Button Sprites")]
    [SerializeField] private Sprite musicEnabledSprite;
    [SerializeField] private Sprite musicDisabledSprite;

    [Header("Vibration Button Sprites")]
    [SerializeField] private Sprite vibrationEnabledSprite;
    [SerializeField] private Sprite vibrationDisabledSprite;

    [Header("Audio Quality Settings")]
    [SerializeField] private string[] qualityLevels = { "LOW", "HIGH" };

    [Header("Button States")]
    [SerializeField] private Color activeButtonColor = Color.white;
    [SerializeField] private Color disabledButtonColor = Color.gray;

    private bool soundEnabled = true;
    private bool musicEnabled = true;
    private bool vibrationEnabled = true;
    private int currentQualityIndex = 1;

    private void Start()
    {
        if (soundButton != null)
            soundButton.onClick.AddListener(ToggleSound);

        if (musicButton != null)
            musicButton.onClick.AddListener(ToggleMusic);

        if (vibrationButton != null)
            vibrationButton.onClick.AddListener(ToggleVibration);

        if (qualityDownButton != null)
            qualityDownButton.onClick.AddListener(DecreaseQuality);

        if (qualityUpButton != null)
            qualityUpButton.onClick.AddListener(IncreaseQuality);

        LoadSettings();
        ApplySettings();
        UpdateButtonVisuals();
    }

    private void LoadSettings()
    {
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        vibrationEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
        currentQualityIndex = PlayerPrefs.GetInt("AudioQualityIndex", 1);

        if (currentQualityIndex < 0) currentQualityIndex = 0;
        if (currentQualityIndex >= qualityLevels.Length) currentQualityIndex = qualityLevels.Length - 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("VibrationEnabled", vibrationEnabled ? 1 : 0);
        PlayerPrefs.SetInt("AudioQualityIndex", currentQualityIndex);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSoundEnabled(soundEnabled);
            AudioManager.Instance.SetMusicEnabled(musicEnabled);

            bool isHighQuality = currentQualityIndex == 1;
            AudioManager.Instance.SetAudioQuality(isHighQuality);
        }

#if UNITY_ANDROID || UNITY_IOS
#endif
    }

    private void ApplyAudioQuality()
    {
        bool isHighQuality = currentQualityIndex == 1;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetAudioQuality(isHighQuality);
        }
    }

    private void UpdateButtonVisuals()
    {
        UpdateSoundButtonSprite();
        UpdateMusicButtonSprite();
        UpdateVibrationButtonSprite();
        UpdateQualityText();
        UpdateQualityButtons();
    }

    private void UpdateSoundButtonSprite()
    {
        if (soundButtonImage != null)
        {
            soundButtonImage.sprite = soundEnabled ? soundEnabledSprite : soundDisabledSprite;
        }
    }

    private void UpdateMusicButtonSprite()
    {
        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = musicEnabled ? musicEnabledSprite : musicDisabledSprite;
        }
    }

    private void UpdateVibrationButtonSprite()
    {
        if (vibrationButtonImage != null)
        {
            vibrationButtonImage.sprite = vibrationEnabled ? vibrationEnabledSprite : vibrationDisabledSprite;
        }
    }

    private void UpdateQualityText()
    {
        if (qualityText != null)
        {
            qualityText.text = qualityLevels[currentQualityIndex];
        }
    }

    private void UpdateQualityButtons()
    {
        if (qualityDownButton != null)
        {
            bool canDecrease = currentQualityIndex > 0;
            qualityDownButton.interactable = canDecrease;

            Image downButtonImage = qualityDownButton.GetComponent<Image>();
            if (downButtonImage != null)
            {
                downButtonImage.color = canDecrease ? activeButtonColor : disabledButtonColor;
            }
        }

        if (qualityUpButton != null)
        {
            bool canIncrease = currentQualityIndex < qualityLevels.Length - 1;
            qualityUpButton.interactable = canIncrease;

            Image upButtonImage = qualityUpButton.GetComponent<Image>();
            if (upButtonImage != null)
            {
                upButtonImage.color = canIncrease ? activeButtonColor : disabledButtonColor;
            }
        }
    }

    private void UpdateButtonSprite(Image buttonImage, bool isEnabled)
    {
    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;

        SaveSettings();
        ApplySettings();
        UpdateButtonVisuals();
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;

        SaveSettings();
        ApplySettings();
        UpdateButtonVisuals();
    }

    public void ToggleVibration()
    {
        vibrationEnabled = !vibrationEnabled;

        SaveSettings();
        ApplySettings();
        UpdateButtonVisuals();

        if (vibrationEnabled)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }

    public void DecreaseQuality()
    {
        if (currentQualityIndex > 0)
        {
            currentQualityIndex--;

            SaveSettings();
            ApplySettings();
            UpdateButtonVisuals();
        }
    }

    public void IncreaseQuality()
    {
        if (currentQualityIndex < qualityLevels.Length - 1)
        {
            currentQualityIndex++;

            SaveSettings();
            ApplySettings();
            UpdateButtonVisuals();
        }
    }

    public bool IsSoundEnabled()
    {
        return soundEnabled;
    }

    public bool IsMusicEnabled()
    {
        return musicEnabled;
    }

    public bool IsVibrationEnabled()
    {
        return vibrationEnabled;
    }

    public string GetCurrentQuality()
    {
        return qualityLevels[currentQualityIndex];
    }

    public int GetQualityIndex()
    {
        return currentQualityIndex;
    }

    public void ResetToDefaults()
    {
        soundEnabled = true;
        musicEnabled = true;
        vibrationEnabled = true;
        currentQualityIndex = 1;

        SaveSettings();
        ApplySettings();
        UpdateButtonVisuals();
    }
}