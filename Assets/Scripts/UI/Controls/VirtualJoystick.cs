using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    [SerializeField] private float joystickRange = 100f;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private RectTransform joystickBackground;

    [Header("Visual Settings")]
    [SerializeField] private float handleReturnSpeed = 15f;
    [SerializeField] private bool snapToCenter = true;

    public Vector2 InputDirection { get; private set; }
    public float InputMagnitude { get; private set; }
    public bool IsPressed { get; private set; }

    private Vector2 joystickCenter;
    private bool isDragging = false;
    private Canvas parentCanvas;
    private Camera uiCamera;

    private float sensitivity = 1.0f;
    private Vector2 originalPosition;

    private void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            uiCamera = parentCanvas.worldCamera;
        else
            uiCamera = null;

        joystickCenter = joystickBackground.anchoredPosition;

        originalPosition = GetComponent<RectTransform>().anchoredPosition;

        InputDirection = Vector2.zero;
        InputMagnitude = 0f;
        IsPressed = false;

        if (joystickHandle != null && snapToCenter)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }

        LoadControlsSettingsFromPlayerPrefs();
    }

    private void LoadControlsSettingsFromPlayerPrefs()
    {
        sensitivity = PlayerPrefs.GetFloat("VJSensitivity", 1.0f);

        int invertControl = PlayerPrefs.GetInt("InvertControl", 0);
        ApplyInvertControl(invertControl);
    }

    private void ApplyInvertControl(int invertControl)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector2 currentPosition = rectTransform.anchoredPosition;

        if (invertControl == 1)
        {
            Vector2 newPosition = currentPosition;
            newPosition.x = -currentPosition.x;
            rectTransform.anchoredPosition = newPosition;
        }

        Vector2 finalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (!isDragging && joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.Lerp(
                joystickHandle.anchoredPosition,
                Vector2.zero,
                handleReturnSpeed * Time.deltaTime
            );
            UpdateInput();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        IsPressed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (joystickHandle == null || joystickBackground == null)
            return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            uiCamera,
            out localPoint
        );

        Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, joystickRange);
        joystickHandle.anchoredPosition = clampedPosition;
        UpdateInput();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        IsPressed = false;

        if (snapToCenter && joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
            UpdateInput();
        }
    }

    private void UpdateInput()
    {
        if (joystickHandle == null)
        {
            InputDirection = Vector2.zero;
            InputMagnitude = 0f;
            return;
        }

        Vector2 handlePosition = joystickHandle.anchoredPosition;
        InputMagnitude = handlePosition.magnitude / joystickRange;
        InputDirection = handlePosition.normalized;
        InputMagnitude = Mathf.Clamp01(InputMagnitude);

        if (InputMagnitude < 0.1f)
        {
            InputDirection = Vector2.zero;
            InputMagnitude = 0f;
        }
    }

    public Vector2 GetInputVector()
    {
        Vector2 baseInput = InputDirection * InputMagnitude;
        Vector2 result = baseInput * sensitivity;

        return result;
    }

    public float GetHorizontalInput()
    {
        return InputDirection.x;
    }

    public float GetVerticalInput()
    {
        return InputDirection.y;
    }

    public Vector2 GetRawInputVector()
    {
        return InputDirection * InputMagnitude;
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    public void RefreshSettingsFromPlayerPrefs()
    {
        LoadControlsSettingsFromPlayerPrefs();
    }

    public void SetJoystickRange(float newRange)
    {
        joystickRange = newRange;
    }

    public void SetJoystickPosition(bool isLeftSide)
    {
        int invertControl = isLeftSide ? 0 : 1;
        ApplyInvertControl(invertControl);
    }

    public void SetJoystickActive(bool active)
    {
        gameObject.SetActive(active);

        if (!active)
        {
            InputDirection = Vector2.zero;
            InputMagnitude = 0f;
            IsPressed = false;
        }
    }
}