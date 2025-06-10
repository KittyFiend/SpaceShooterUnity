using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FireButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Fire Button Settings")]
    [SerializeField] private Button fireButton;
    [SerializeField] private RectTransform fireButtonTransform;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.yellow;
    [SerializeField] private float pressedScale = 0.9f;

    public bool IsPressed { get; private set; }

    private Image buttonImage;
    private Vector3 originalScale;
    private Vector2 originalPosition;

    private void Start()
    {
        if (fireButton == null)
            fireButton = GetComponent<Button>();

        if (fireButtonTransform == null)
            fireButtonTransform = GetComponent<RectTransform>();

        buttonImage = GetComponent<Image>();

        originalScale = fireButtonTransform.localScale;
        originalPosition = fireButtonTransform.anchoredPosition;

        IsPressed = false;
        LoadAndApplySettings();
    }

    private void LoadAndApplySettings()
    {
        int firingMode = PlayerPrefs.GetInt("FiringMode", 0);
        int invertControl = PlayerPrefs.GetInt("InvertControl", 0);

        bool shouldShow = (firingMode == 1);
        gameObject.SetActive(shouldShow);

        if (shouldShow)
        {
            ApplyInvertControl(invertControl == 1);
        }
    }

    private void ApplyInvertControl(bool isInverted)
    {
        if (fireButtonTransform == null) return;

        Vector2 newPosition = originalPosition;

        if (isInverted)
        {
            newPosition.x = -Mathf.Abs(originalPosition.x);
        }
        else
        {
            newPosition.x = Mathf.Abs(originalPosition.x);
        }

        fireButtonTransform.anchoredPosition = newPosition;
    }

    private bool IsVibrationEnabled()
    {
        bool vibrationEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
        return vibrationEnabled;
    }

    private void TriggerVibrationIfEnabled()
    {
        if (!IsVibrationEnabled())
        {
            return;
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Handheld.Vibrate();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        ApplyPressedVisuals();

        TriggerVibrationIfEnabled();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        ApplyNormalVisuals();
    }

    private void ApplyPressedVisuals()
    {
        if (buttonImage != null)
            buttonImage.color = pressedColor;

        if (fireButtonTransform != null)
            fireButtonTransform.localScale = originalScale * pressedScale;
    }

    private void ApplyNormalVisuals()
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;

        if (fireButtonTransform != null)
            fireButtonTransform.localScale = originalScale;
    }

    public void RefreshSettings()
    {
        LoadAndApplySettings();
    }

    public void SetColors(Color normal, Color pressed)
    {
        normalColor = normal;
        pressedColor = pressed;

        if (!IsPressed)
            ApplyNormalVisuals();
    }

    public void SetButtonActive(bool active)
    {
        if (fireButton != null)
            fireButton.interactable = active;

        if (!active)
        {
            IsPressed = false;
            ApplyNormalVisuals();
        }
    }

    public void TestVibration()
    {
        TriggerVibrationIfEnabled();
    }
}