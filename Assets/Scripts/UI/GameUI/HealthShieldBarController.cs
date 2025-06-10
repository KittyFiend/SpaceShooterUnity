using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthShieldBarController : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Transform healthBarContainer;
    [SerializeField] private Color healthActiveColor = Color.green;
    [SerializeField] private Color healthInactiveColor = Color.gray;

    [Header("Shield Bar")]
    [SerializeField] private Transform shieldBarContainer;
    [SerializeField] private Color shieldActiveColor = Color.cyan;
    [SerializeField] private Color shieldInactiveColor = Color.gray;

    [Header("Animation Settings")]
    [SerializeField] private float blinkDuration = 0.5f;
    [SerializeField] private int blinkCount = 3;

    private List<Image> healthDots = new List<Image>();
    private List<Image> shieldDots = new List<Image>();

    private int currentHealth = 0;
    private int maxHealth = 3;
    private int currentShield = 0;
    private int maxShield = 5;

    private int bonusMaxHealth = 0;
    private int bonusShieldDuration = 0;

    private bool isInitialized = false;
    private bool healthBarInitialized = false;

    private void Start()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;

        FindHealthDots();
        FindShieldDots();
        InitializeHealthBar();
        InitializeShieldBar();
    }

    private void FindHealthDots()
    {
        healthDots.Clear();

        if (healthBarContainer != null)
        {
            for (int i = 0; i < healthBarContainer.childCount; i++)
            {
                Transform child = healthBarContainer.GetChild(i);
                if (child.name.Contains("HealthDot"))
                {
                    Image dotImage = child.GetComponent<Image>();
                    if (dotImage != null)
                    {
                        healthDots.Add(dotImage);
                    }
                }
            }
        }
    }

    private void FindShieldDots()
    {
        shieldDots.Clear();

        if (shieldBarContainer != null)
        {
            for (int i = 0; i < shieldBarContainer.childCount; i++)
            {
                Transform child = shieldBarContainer.GetChild(i);
                if (child.name.Contains("ShieldDot"))
                {
                    Image dotImage = child.GetComponent<Image>();
                    if (dotImage != null)
                    {
                        shieldDots.Add(dotImage);
                    }
                }
            }
        }
    }

    private void InitializeHealthBar()
    {
        if (healthBarInitialized)
        {
            return;
        }

        healthBarInitialized = true;

        int upgradeHealth = 0;
        if (ShipStatsManager.Instance != null)
        {
            upgradeHealth = ShipStatsManager.Instance.GetFinalHealth() - 3;
        }

        maxHealth = Mathf.Min(3 + bonusMaxHealth + upgradeHealth, healthDots.Count);
        currentHealth = maxHealth;

        UpdateHealthDisplay();
    }

    private void InitializeShieldBar()
    {
        int upgradeShield = 0;
        if (ShipStatsManager.Instance != null)
        {
            upgradeShield = (int)(ShipStatsManager.Instance.GetFinalShieldDuration() - 5f);
        }
        maxShield = Mathf.Min(5 + bonusShieldDuration + upgradeShield, shieldDots.Count);
        currentShield = 0;

        UpdateShieldDisplay();
    }

    private void UpdateHealthDisplay()
    {
        for (int i = 0; i < healthDots.Count; i++)
        {
            if (i < currentHealth)
            {
                healthDots[i].color = healthActiveColor;
                healthDots[i].gameObject.SetActive(true);
            }
            else
            {
                healthDots[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateShieldDisplay()
    {
        for (int i = 0; i < shieldDots.Count; i++)
        {
            if (i < currentShield)
            {
                shieldDots[i].color = shieldActiveColor;
                shieldDots[i].gameObject.SetActive(true);
            }
            else
            {
                shieldDots[i].gameObject.SetActive(false);
            }
        }
    }

    public void AddHealth(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentHealth < maxHealth)
            {
                int segmentIndex = currentHealth;
                currentHealth++;

                StartCoroutine(AnimateHealthGain(segmentIndex));
            }
            else if (maxHealth < healthDots.Count)
            {
                maxHealth++;
                currentHealth = maxHealth;

                UpdateHealthDisplay();
                StartCoroutine(AnimateHealthGain(maxHealth - 1));
            }
            else
            {
                break;
            }
        }

        UpdateHealthDisplay();
    }

    public void TakeDamage(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentHealth > 0)
            {
                currentHealth--;
                int segmentIndex = currentHealth;

                StartCoroutine(AnimateHealthLoss(segmentIndex));

                if (currentHealth <= 0)
                {
                    break;
                }
            }
        }
    }

    public void ActivateShield()
    {
        currentShield = maxShield;
        UpdateShieldDisplay();

        StartCoroutine(ShieldCountdown());
    }

    private IEnumerator ShieldCountdown()
    {
        while (currentShield > 0)
        {
            yield return new WaitForSeconds(1f);

            currentShield--;
            UpdateShieldDisplay();
        }
    }

    private IEnumerator AnimateHealthGain(int segmentIndex)
    {
        if (segmentIndex >= 0 && segmentIndex < healthDots.Count)
        {
            Image dotImage = healthDots[segmentIndex];

            for (int i = 0; i < blinkCount; i++)
            {
                dotImage.color = Color.white;
                yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));

                dotImage.color = healthActiveColor;
                yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));
            }

            dotImage.color = healthActiveColor;
        }
    }

    private IEnumerator AnimateHealthLoss(int segmentIndex)
    {
        if (segmentIndex >= 0 && segmentIndex < healthDots.Count)
        {
            Image dotImage = healthDots[segmentIndex];

            for (int i = 0; i < blinkCount; i++)
            {
                dotImage.color = Color.red;
                yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));

                dotImage.color = healthActiveColor;
                yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));
            }

            dotImage.gameObject.SetActive(false);
        }
    }

    public void SetHealthUpgrade(int bonusHealth)
    {
        bonusMaxHealth = bonusHealth;
        InitializeHealthBar();
    }

    public void SetShieldUpgrade(int bonusShieldTime)
    {
        bonusShieldDuration = bonusShieldTime;
        InitializeShieldBar();
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetCurrentShield() => currentShield;
    public int GetMaxShield() => maxShield;
    public bool HasShield() => currentShield > 0;
}