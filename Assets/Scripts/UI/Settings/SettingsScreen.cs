using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsScreen : MonoBehaviour
{
    [Header("Volume Controls")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private Button resetSettingsButton;

    [Header("Confirmation Dialog")]
    [SerializeField] private GameObject confirmationDialog;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private void Start()
    {
        SetupVolumeSliders();
        SetupButtons();

        if (confirmationDialog != null)
        {
            confirmationDialog.SetActive(false);
        }
    }

    private void SetupVolumeSliders()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            UpdateMusicVolumeText(musicVolumeSlider.value);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            UpdateSfxVolumeText(sfxVolumeSlider.value);
        }
    }

    private void SetupButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(OnResetProgressButtonClicked);
        }

        if (resetSettingsButton != null)
        {
            resetSettingsButton.onClick.AddListener(OnResetSettingsButtonClicked);
        }

        if (confirmYesButton != null)
        {
            confirmYesButton.onClick.AddListener(OnConfirmYes);
        }

        if (confirmNoButton != null)
        {
            confirmNoButton.onClick.AddListener(OnConfirmNo);
        }
    }

    private void OnMusicVolumeChanged(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
        UpdateMusicVolumeText(volume);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxVolume(volume);
        }
        UpdateSfxVolumeText(volume);
    }

    private void UpdateMusicVolumeText(float volume)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = $"Music: {Mathf.RoundToInt(volume * 100)}%";
        }
    }

    private void UpdateSfxVolumeText(float volume)
    {
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = $"SFX: {Mathf.RoundToInt(volume * 100)}%";
        }
    }

    private void OnBackButtonClicked()
    {
        if (ScreenManager.Instance != null)
        {
            ScreenManager.Instance.GoBack();
        }
    }

    private void OnResetProgressButtonClicked()
    {
        ShowConfirmationDialog("Are you sure you want to reset all progress? This cannot be undone.", "progress");
    }

    private void OnResetSettingsButtonClicked()
    {
        ShowConfirmationDialog("Reset all settings to default values?", "settings");
    }

    private void ShowConfirmationDialog(string message, string confirmationType)
    {
        if (confirmationDialog != null && confirmationText != null)
        {
            confirmationDialog.SetActive(true);
            confirmationText.text = message;

            PlayerPrefs.SetString("ConfirmationType", confirmationType);
        }
    }

    private void OnConfirmYes()
    {
        string confirmationType = PlayerPrefs.GetString("ConfirmationType", "");

        if (confirmationType == "progress")
        {
            ResetProgress();
        }
        else if (confirmationType == "settings")
        {
            ResetSettings();
        }

        OnConfirmNo();
    }

    private void OnConfirmNo()
    {
        if (confirmationDialog != null)
        {
            confirmationDialog.SetActive(false);
        }
        PlayerPrefs.DeleteKey("ConfirmationType");
    }

    private void ResetProgress()
    {
        for (int i = 1; i <= 20; i++)
        {
            PlayerPrefs.DeleteKey($"Level_{i}_Unlocked");
            PlayerPrefs.DeleteKey($"Level_{i}_Stars");
            PlayerPrefs.DeleteKey($"Level_{i}_BestScore");
        }

        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.Save();
    }

    private void ResetSettings()
    {
        PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
        PlayerPrefs.Save();

        SetupVolumeSliders();
    }
}