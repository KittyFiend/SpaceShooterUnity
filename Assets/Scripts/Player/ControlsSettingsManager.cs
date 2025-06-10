using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsSettingsManager : MonoBehaviour
{
    [Header("Follow Method Controls")]
    [SerializeField] private Button followMethodDownButton;
    [SerializeField] private Button followMethodUpButton;
    [SerializeField] private TextMeshProUGUI followMethodText;

    [Header("Sensitivity Controls")]
    [SerializeField] private GameObject fingerOffsetRow;
    [SerializeField] private GameObject draggingSensitivityRow;
    [SerializeField] private GameObject vjSensitivityRow;

    [SerializeField] private Slider fingerOffsetSlider;
    [SerializeField] private Slider draggingSensitivitySlider;
    [SerializeField] private Slider vjSensitivitySlider;

    [Header("Reset Button")]
    [SerializeField] private Button resetButton;

    [Header("Firing Mode Controls")]
    [SerializeField] private Button firingModeDownButton;
    [SerializeField] private Button firingModeUpButton;
    [SerializeField] private TextMeshProUGUI firingModeText;

    [Header("Invert Control")]
    [SerializeField] private Button invertControlDownButton;
    [SerializeField] private Button invertControlUpButton;
    [SerializeField] private TextMeshProUGUI invertControlText;

    [Header("UI State Colors")]
    [SerializeField] private Color activeRowColor = Color.white;
    [SerializeField] private Color inactiveRowColor = Color.gray;
    [SerializeField] private Color activeButtonColor = Color.white;
    [SerializeField] private Color disabledButtonColor = Color.gray;

    public enum ControlMethod
    {
        Point = 0,
        Drag = 1,
        VirtualJoystick = 2
    }

    public enum FiringMode
    {
        Automatic = 0,
        OnScreenButton = 1
    }

    public enum InvertControl
    {
        No = 0,
        Yes = 1
    }

    private ControlMethod currentControlMethod = ControlMethod.VirtualJoystick;
    private FiringMode currentFiringMode = FiringMode.Automatic;
    private InvertControl currentInvertControl = InvertControl.No;

    private float fingerOffset = 50f;
    private float draggingSensitivity = 1.5f;
    private float vjSensitivity = 1.0f;

    private string[] controlMethodNames = { "POINT", "DRAG", "VIRTUAL JOYSTICK" };
    private string[] firingModeNames = { "AUTOMATIC", "ON-SCREEN BUTTON" };
    private string[] invertControlNames = { "NO", "YES" };

    private void Start()
    {
        DebugInspectorSettings();
        SetupButtons();
        LoadSettings();
        UpdateAllVisuals();
    }

    private void DebugInspectorSettings()
    {
    }

    private void SetupButtons()
    {
        if (followMethodDownButton != null)
            followMethodDownButton.onClick.AddListener(DecreaseFollowMethod);
        if (followMethodUpButton != null)
            followMethodUpButton.onClick.AddListener(IncreaseFollowMethod);

        if (firingModeDownButton != null)
            firingModeDownButton.onClick.AddListener(DecreaseFiringMode);
        if (firingModeUpButton != null)
            firingModeUpButton.onClick.AddListener(IncreaseFiringMode);

        if (invertControlDownButton != null)
            invertControlDownButton.onClick.AddListener(DecreaseInvertControl);
        if (invertControlUpButton != null)
            invertControlUpButton.onClick.AddListener(IncreaseInvertControl);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);

        if (fingerOffsetSlider != null)
        {
            fingerOffsetSlider.minValue = 10f;
            fingerOffsetSlider.maxValue = 200f;
            fingerOffsetSlider.onValueChanged.AddListener(OnFingerOffsetChanged);
        }

        if (draggingSensitivitySlider != null)
        {
            draggingSensitivitySlider.minValue = 0.5f;
            draggingSensitivitySlider.maxValue = 3.0f;
            draggingSensitivitySlider.onValueChanged.AddListener(OnDraggingSensitivityChanged);
        }

        if (vjSensitivitySlider != null)
        {
            vjSensitivitySlider.minValue = 0.5f;
            vjSensitivitySlider.maxValue = 2.0f;
            vjSensitivitySlider.onValueChanged.AddListener(OnVJSensitivityChanged);
        }
    }

    private void LoadSettings()
    {
        currentControlMethod = (ControlMethod)PlayerPrefs.GetInt("ControlMethod", 2);
        currentFiringMode = (FiringMode)PlayerPrefs.GetInt("FiringMode", 0);
        currentInvertControl = (InvertControl)PlayerPrefs.GetInt("InvertControl", 0);

        fingerOffset = PlayerPrefs.GetFloat("FingerOffset", 50f);
        draggingSensitivity = PlayerPrefs.GetFloat("DraggingSensitivity", 1.5f);
        vjSensitivity = PlayerPrefs.GetFloat("VJSensitivity", 1.0f);

        if (fingerOffsetSlider != null)
        {
            fingerOffsetSlider.SetValueWithoutNotify(fingerOffset);
        }
        if (draggingSensitivitySlider != null)
        {
            draggingSensitivitySlider.SetValueWithoutNotify(draggingSensitivity);
        }
        if (vjSensitivitySlider != null)
        {
            vjSensitivitySlider.SetValueWithoutNotify(vjSensitivity);
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("ControlMethod", (int)currentControlMethod);
        PlayerPrefs.SetInt("FiringMode", (int)currentFiringMode);
        PlayerPrefs.SetInt("InvertControl", (int)currentInvertControl);

        PlayerPrefs.SetFloat("FingerOffset", fingerOffset);
        PlayerPrefs.SetFloat("DraggingSensitivity", draggingSensitivity);
        PlayerPrefs.SetFloat("VJSensitivity", vjSensitivity);

        PlayerPrefs.Save();

        ApplySettingsToGame();
    }

    private void ApplySettingsToGame()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.UpdateControlsSettings();
        }

        VirtualJoystick virtualJoystick = FindObjectOfType<VirtualJoystick>();
        if (virtualJoystick != null)
        {
            virtualJoystick.UpdateSensitivity(vjSensitivity);

            bool isLeftSide = currentInvertControl == InvertControl.No;
            virtualJoystick.SetJoystickPosition(isLeftSide);
        }
    }

    private void DecreaseFollowMethod()
    {
        if (currentControlMethod > 0)
        {
            currentControlMethod = (ControlMethod)((int)currentControlMethod - 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void IncreaseFollowMethod()
    {
        if (currentControlMethod < (ControlMethod)(controlMethodNames.Length - 1))
        {
            currentControlMethod = (ControlMethod)((int)currentControlMethod + 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void DecreaseFiringMode()
    {
        if (currentFiringMode > 0)
        {
            currentFiringMode = (FiringMode)((int)currentFiringMode - 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void IncreaseFiringMode()
    {
        if (currentFiringMode < (FiringMode)(firingModeNames.Length - 1))
        {
            currentFiringMode = (FiringMode)((int)currentFiringMode + 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void DecreaseInvertControl()
    {
        if (currentInvertControl > 0)
        {
            currentInvertControl = (InvertControl)((int)currentInvertControl - 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void IncreaseInvertControl()
    {
        if (currentInvertControl < (InvertControl)(invertControlNames.Length - 1))
        {
            currentInvertControl = (InvertControl)((int)currentInvertControl + 1);
            SaveSettings();
            UpdateAllVisuals();
        }
    }

    private void OnFingerOffsetChanged(float value)
    {
        if (currentControlMethod == ControlMethod.Point)
        {
            fingerOffset = value;
            SaveSettings();
        }
    }

    private void OnDraggingSensitivityChanged(float value)
    {
        if (currentControlMethod == ControlMethod.Drag)
        {
            draggingSensitivity = value;
            SaveSettings();
        }
    }

    private void OnVJSensitivityChanged(float value)
    {
        if (currentControlMethod == ControlMethod.VirtualJoystick)
        {
            vjSensitivity = value;
            SaveSettings();
        }
    }

    private void UpdateAllVisuals()
    {
        UpdateFollowMethodText();
        UpdateFiringModeText();
        UpdateInvertControlText();
        UpdateSensitivityRows();
        UpdateNavigationButtons();
    }

    private void UpdateFollowMethodText()
    {
        if (followMethodText != null)
        {
            followMethodText.text = controlMethodNames[(int)currentControlMethod];
        }
    }

    private void UpdateFiringModeText()
    {
        if (firingModeText != null)
        {
            firingModeText.text = firingModeNames[(int)currentFiringMode];
        }
    }

    private void UpdateInvertControlText()
    {
        if (invertControlText != null)
        {
            invertControlText.text = invertControlNames[(int)currentInvertControl];
        }
    }

    private void UpdateSensitivityRows()
    {
        bool pointActive = currentControlMethod == ControlMethod.Point;
        bool dragActive = currentControlMethod == ControlMethod.Drag;
        bool joystickActive = currentControlMethod == ControlMethod.VirtualJoystick;

        SetSliderActive(fingerOffsetSlider, pointActive);
        SetSliderActive(draggingSensitivitySlider, dragActive);
        SetSliderActive(vjSensitivitySlider, joystickActive);

        SetRowTextColor(fingerOffsetRow, pointActive);
        SetRowTextColor(draggingSensitivityRow, dragActive);
        SetRowTextColor(vjSensitivityRow, joystickActive);
    }

    private void SetSliderActive(Slider slider, bool active)
    {
        if (slider == null)
        {
            return;
        }

        slider.interactable = active;

        CanvasGroup canvasGroup = slider.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = slider.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
        canvasGroup.alpha = active ? 1f : 0.4f;
    }

    private void SetRowTextColor(GameObject row, bool active)
    {
        if (row == null) return;

        TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            text.color = active ? activeRowColor : inactiveRowColor;
        }
    }

    private void UpdateNavigationButtons()
    {
        UpdateNavigationButton(followMethodDownButton, (int)currentControlMethod > 0);
        UpdateNavigationButton(followMethodUpButton, (int)currentControlMethod < controlMethodNames.Length - 1);
        UpdateNavigationButton(firingModeDownButton, (int)currentFiringMode > 0);
        UpdateNavigationButton(firingModeUpButton, (int)currentFiringMode < firingModeNames.Length - 1);
        UpdateNavigationButton(invertControlDownButton, (int)currentInvertControl > 0);
        UpdateNavigationButton(invertControlUpButton, (int)currentInvertControl < invertControlNames.Length - 1);
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

    public ControlMethod GetControlMethod() => currentControlMethod;
    public FiringMode GetFiringMode() => currentFiringMode;
    public InvertControl GetInvertControl() => currentInvertControl;
    public float GetFingerOffset() => fingerOffset;
    public float GetDraggingSensitivity() => draggingSensitivity;
    public float GetVJSensitivity() => vjSensitivity;

    public void ResetToDefaults()
    {
        currentControlMethod = ControlMethod.VirtualJoystick;
        currentFiringMode = FiringMode.Automatic;
        currentInvertControl = InvertControl.No;
        fingerOffset = 50f;
        draggingSensitivity = 1.5f;
        vjSensitivity = 1.0f;

        if (fingerOffsetSlider != null) fingerOffsetSlider.SetValueWithoutNotify(fingerOffset);
        if (draggingSensitivitySlider != null) draggingSensitivitySlider.SetValueWithoutNotify(draggingSensitivity);
        if (vjSensitivitySlider != null) vjSensitivitySlider.SetValueWithoutNotify(vjSensitivity);

        SaveSettings();
        UpdateAllVisuals();
    }
}