using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance { get; private set; }

    [Header("HP Bar Parts")]
    [SerializeField] private RectTransform hpBarLeft;
    [SerializeField] private RectTransform hpBarCenter;
    [SerializeField] private RectTransform hpBarRight;
    [SerializeField] private GameObject bossUIPanel;

    [Header("Settings")]
    [SerializeField] private float maxBarWidth = 300f;
    [SerializeField] private float animationSpeed = 3f;

    private float currentHealthPercent = 1f;
    private float targetHealthPercent = 1f;
    private bool isAnimating = false;

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
        HideBossUI();
    }

    private void Update()
    {
        if (isAnimating && Mathf.Abs(currentHealthPercent - targetHealthPercent) > 0.01f)
        {
            currentHealthPercent = Mathf.Lerp(currentHealthPercent, targetHealthPercent, animationSpeed * Time.deltaTime);
            UpdateBarVisual();
        }
        else if (isAnimating)
        {
            currentHealthPercent = targetHealthPercent;
            UpdateBarVisual();
            isAnimating = false;
        }
    }

    public void ShowBossUI()
    {
        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(true);
            ResetHealthBar();
        }
    }

    public void HideBossUI()
    {
        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(false);
        }
    }

    public void SetBossHealth(float currentHealth, float maxHealth)
    {
        targetHealthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        isAnimating = true;
    }

    private void ResetHealthBar()
    {
        currentHealthPercent = 1f;
        targetHealthPercent = 1f;
        UpdateBarVisual();
    }

    private void UpdateBarVisual()
    {
        if (hpBarLeft == null || hpBarCenter == null || hpBarRight == null) return;

        float currentWidth = maxBarWidth * currentHealthPercent;
        hpBarCenter.sizeDelta = new Vector2(currentWidth, hpBarCenter.sizeDelta.y);

        float leftPosition = -(currentWidth / 2) + 5f;
        hpBarLeft.anchoredPosition = new Vector2(leftPosition, hpBarLeft.anchoredPosition.y);

        float rightPosition = (currentWidth / 2) - 5f;
        hpBarRight.anchoredPosition = new Vector2(rightPosition, hpBarRight.anchoredPosition.y);

        if (currentHealthPercent < 0.05f)
        {
            hpBarLeft.gameObject.SetActive(false);
            hpBarRight.gameObject.SetActive(false);
        }
        else if (currentHealthPercent < 0.1f)
        {
            hpBarRight.gameObject.SetActive(false);
            hpBarLeft.gameObject.SetActive(true);
        }
        else
        {
            hpBarLeft.gameObject.SetActive(true);
            hpBarRight.gameObject.SetActive(true);
        }
    }
}