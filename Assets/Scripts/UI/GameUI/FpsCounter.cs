using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [Header("FPS Counter Settings")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] private Color goodFpsColor = Color.green;
    [SerializeField] private Color okFpsColor = Color.yellow;
    [SerializeField] private Color badFpsColor = Color.red;

    private float currentFps = 0f;
    private float timer = 0f;
    private bool isActive = false;

    private void Start()
    {
        if (fpsText == null)
            fpsText = GetComponent<TextMeshProUGUI>();

        LoadSettings();
    }

    private void LoadSettings()
    {
        int fpsCounterMode = PlayerPrefs.GetInt("FpsCounterMode", 0);
        bool shouldBeActive = fpsCounterMode == 1;
        SetFpsCounterActive(shouldBeActive);
    }

    private void Update()
    {
        if (!isActive || fpsText == null) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            currentFps = 1f / Time.deltaTime;
            UpdateFpsDisplay();
            timer = 0f;
        }
    }

    private void UpdateFpsDisplay()
    {
        if (fpsText == null) return;

        fpsText.text = $"{currentFps:F0}";

        if (currentFps >= 60f)
        {
            fpsText.color = goodFpsColor;
        }
        else if (currentFps >= 30f)
        {
            fpsText.color = okFpsColor;
        }
        else
        {
            fpsText.color = badFpsColor;
        }
    }

    public void SetFpsCounterActive(bool active)
    {
        isActive = active;
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(active);
        }
    }

    public float GetCurrentFps()
    {
        return currentFps;
    }

    public void RefreshSettings()
    {
        LoadSettings();
    }

    public void SetFpsColors(Color good, Color ok, Color bad)
    {
        goodFpsColor = good;
        okFpsColor = ok;
        badFpsColor = bad;
    }

    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.1f, interval);
    }
}