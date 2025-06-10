using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeFunctionalManager : MonoBehaviour
{
    [Header("Price Display")]
    [SerializeField] private TextMeshProUGUI healthPriceText;
    [SerializeField] private TextMeshProUGUI armorPriceText;
    [SerializeField] private TextMeshProUGUI speedPriceText;
    [SerializeField] private TextMeshProUGUI damagePriceText;
    [SerializeField] private TextMeshProUGUI totalCrystalsText;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button armorUpgradeButton;
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private Button damageUpgradeButton;

    [Header("Confirmation Dialog")]
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private TextMeshProUGUI priceConfirmText;

    [Header("Info Dialog")]
    [SerializeField] private TextMeshProUGUI infoDescriptionText;

    [Header("Price Settings")]
    [SerializeField] private int basePriceHealth = 150;
    [SerializeField] private int basePriceArmor = 150;
    [SerializeField] private int basePriceSpeed = 150;
    [SerializeField] private int basePriceDamage = 150;
    [SerializeField] private float priceMultiplier = 2f;

    private PowerupType currentUpgradeType;
    private int currentUpgradePrice;

    private ShipStatsManager shipStats;
    private HangarFunctionalManager hangarManager;

    private void Start()
    {
        StartCoroutine(WaitForManagers());
    }

    private System.Collections.IEnumerator WaitForManagers()
    {
        while (ShipStatsManager.Instance == null)
        {
            yield return null;
        }

        shipStats = ShipStatsManager.Instance;
        hangarManager = FindObjectOfType<HangarFunctionalManager>();

        RefreshUpgradeData();
    }

    public void RefreshUpgradeData()
    {
        if (shipStats == null)
        {
            return;
        }

        UpdateCrystalsDisplay();

        UpdateUpgradeButton(PowerupType.ExtraLife, healthUpgradeButton, healthPriceText, basePriceHealth);
        UpdateUpgradeButton(PowerupType.Shield, armorUpgradeButton, armorPriceText, basePriceArmor);
        UpdateUpgradeButton(PowerupType.RapidFire, speedUpgradeButton, speedPriceText, basePriceSpeed);
        UpdateUpgradeButton(PowerupType.Bomb, damageUpgradeButton, damagePriceText, basePriceDamage);
    }

    private void UpdateUpgradeButton(PowerupType type, Button button, TextMeshProUGUI priceText, int basePrice)
    {
        if (button == null || priceText == null) return;

        int currentLevel = shipStats.GetUpgradeLevel(type);
        bool canUpgrade = shipStats.CanUpgrade(type);
        int price = CalculateUpgradePrice(basePrice, currentLevel);

        if (canUpgrade)
        {
            button.interactable = true;
            priceText.text = price.ToString();
            priceText.color = Color.white;

            if (GameManager.Instance != null && GameManager.Instance.GetTotalCrystals() < price)
            {
                priceText.color = Color.red;
                button.interactable = false;
            }
        }
        else
        {
            button.interactable = false;
            priceText.text = "MAX";
            priceText.color = Color.gray;
        }
    }

    private int CalculateUpgradePrice(int basePrice, int currentLevel)
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, currentLevel));
    }

    private void UpdateCrystalsDisplay()
    {
        if (totalCrystalsText != null && GameManager.Instance != null)
        {
            int crystals = GameManager.Instance.GetTotalCrystals();
            totalCrystalsText.text = crystals.ToString();
        }
    }

    private int GetBasePrice(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.ExtraLife: return basePriceHealth;
            case PowerupType.Shield: return basePriceArmor;
            case PowerupType.RapidFire: return basePriceSpeed;
            case PowerupType.Bomb: return basePriceDamage;
            default: return 150;
        }
    }

    private string GetUpgradeName(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.ExtraLife: return "ЗДОРОВ'Я";
            case PowerupType.Shield: return "БРОНЮ";
            case PowerupType.RapidFire: return "ШВИДКІСТЬ";
            case PowerupType.Bomb: return "УРОН";
            default: return "ПОКРАЩЕННЯ";
        }
    }

    public void RequestUpgrade(PowerupType type)
    {
        if (!shipStats.CanUpgrade(type))
        {
            return;
        }

        currentUpgradeType = type;

        int currentLevel = shipStats.GetUpgradeLevel(type);
        int basePrice = GetBasePrice(type);
        currentUpgradePrice = CalculateUpgradePrice(basePrice, currentLevel);

        if (GameManager.Instance != null && GameManager.Instance.GetTotalCrystals() < currentUpgradePrice)
        {
            return;
        }
    }

    public void ConfirmUpgrade()
    {
        if (GameManager.Instance != null && GameManager.Instance.SpendTotalCrystals(currentUpgradePrice))
        {
            if (shipStats.UpgradeCharacteristic(currentUpgradeType))
            {
                RefreshUpgradeData();

                if (hangarManager != null)
                {
                    hangarManager.RefreshHangarData();
                }
            }
        }
    }

    public void UpdateConfirmationDialog()
    {
        string upgradeName = GetUpgradeName(currentUpgradeType);
        if (confirmationText != null)
            confirmationText.text = $"Покращити {upgradeName}?";

        if (priceConfirmText != null)
            priceConfirmText.text = currentUpgradePrice.ToString();
    }

    public void UpdateInfoDialog()
    {
        if (infoDescriptionText != null && shipStats != null)
        {
            string description = shipStats.GetUpgradeDescription(currentUpgradeType);
            infoDescriptionText.text = description;
        }
    }

    [ContextMenu("Add Test Crystals")]
    public void AddTestCrystals()
    {
        if (GameManager.Instance != null)
        {
            int currentCrystals = GameManager.Instance.GetTotalCrystals();
            PlayerPrefs.SetInt("TotalCrystals", currentCrystals + 1000);
            PlayerPrefs.Save();

            RefreshUpgradeData();
        }
    }

    public (PowerupType type, int price) GetCurrentUpgradeInfo()
    {
        return (currentUpgradeType, currentUpgradePrice);
    }
}