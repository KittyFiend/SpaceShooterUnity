using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettingsManager : MonoBehaviour
{
    [Header("Max FPS Controls")]
    [SerializeField] private Slider maxFpsSlider;
    [SerializeField] private TextMeshProUGUI maxFpsValueText;

    [Header("FPS Counter Controls")]
    [SerializeField] private Button fpsCounterDownButton;
    [SerializeField] private Button fpsCounterUpButton;
    [SerializeField] private TextMeshProUGUI fpsCounterText;

    [Header("Reset Button")]
    [SerializeField] private Button resetButton;

    [Header("UI State Colors")]
    [SerializeField] private Color activeButtonColor = Color.white;
    [SerializeField] private Color disabledButtonColor = Color.gray;

    public enum FpsCounterMode
    {
        Off = 0,
        On = 1
    }

    private float maxFps = 60f;
    private FpsCounterMode currentFpsCounterMode = FpsCounterMode.Off;

    private string[] fpsCounterModeNames = { "OFF", "ON" };

    private void Start()
    {
        SetupControls();
        LoadSettings();
        UpdateAllVisuals();
        ApplySettingsToGame();
    }

    private void SetupControls()
    {
        if (fpsCounterDownButton != null)
            fpsCounterDownButton.onClick.AddListener(DecreaseFpsCounter);
        if (fpsCounterUpButton != null)
            fpsCounterUpButton.onClick.AddListener(IncreaseFpsCounter);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);

        if (maxFpsSlider != null)
        {
            maxFpsSlider.minValue = 30f;
            maxFpsSlider.maxValue = 120f;
            maxFpsSlider.wholeNumbers = true;
            maxFpsSlider.onValueChanged.AddListener(OnMaxFpsChanged);
        }
    }

    private void LoadSettings()
    {
        maxFps = PlayerPrefs.GetFloat("MaxFPS", 60f);
        currentFpsCounterMode = (FpsCounterMode)PlayerPrefs.GetInt("FpsCounterMode", 0);

        if (maxFpsSlider != null)
        {
            maxFpsSlider.SetValueWithoutNotify(maxFps);
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MaxFPS", maxFps);
        PlayerPrefs.SetInt("FpsCounterMode", (int)currentFpsCounterMode);
        PlayerPrefs.Save();

        ApplySettingsToGame();
    }

    private void ApplySettingsToGame()
    {
        Application.targetFrameRate = (int)maxFps;

        FpsCounter fpsCounter = FindObjectOfType<FpsCounter>();
        if (fpsCounter != null)
        {
            bool showFpsCounter = currentFpsCounterMode == FpsCounterMode.On;
            fpsCounter.SetFpsCounterActive(showFpsCounter);
        }
    }

    private void DecreaseFpsCounter()
    {
        if (currentFpsCounterMode > 0)
        {
            currentFpsCounterMode = (FpsCounterMode)((int)currentFpsCounterMode - 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void IncreaseFpsCounter()
    {
        if (currentFpsCounterMode < (FpsCounterMode)(fpsCounterModeNames.Length - 1))
        {
            currentFpsCounterMode = (FpsCounterMode)((int)currentFpsCounterMode + 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void OnMaxFpsChanged(float value)
    {
        maxFps = value;
        SaveSettings();
        UpdateMaxFpsText();
    }

    private void UpdateAllVisuals()
    {
        UpdateFpsCounterText();
        UpdateMaxFpsText();
        UpdateNavigationButtons();
    }

    private void UpdateFpsCounterText()
    {
        if (fpsCounterText != null)
        {
            fpsCounterText.text = fpsCounterModeNames[(int)currentFpsCounterMode];
        }
    }

    private void UpdateMaxFpsText()
    {
        if (maxFpsValueText != null)
        {
            maxFpsValueText.text = maxFps.ToString("F0");
        }
    }

    private void UpdateNavigationButtons()
    {
        UpdateNavigationButton(fpsCounterDownButton, (int)currentFpsCounterMode > 0);
        UpdateNavigationButton(fpsCounterUpButton, (int)currentFpsCounterMode < fpsCounterModeNames.Length - 1);
    }

    private void UpdateNavigationButton(Button button, bool canNavigate)
    {
        if (button == null) return;

        button.interactable = canNavigate;
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = canNavigate ? activeButtonColor : disabledButtonColor;
        }
    }

    public float GetMaxFps() => maxFps;
    public FpsCounterMode GetFpsCounterMode() => currentFpsCounterMode;

    public void ResetToDefaults()
    {
        maxFps = 60f;
        currentFpsCounterMode = FpsCounterMode.Off;

        if (maxFpsSlider != null)
            maxFpsSlider.SetValueWithoutNotify(maxFps);

        SaveSettings();
        UpdateAllVisuals();
    }
}