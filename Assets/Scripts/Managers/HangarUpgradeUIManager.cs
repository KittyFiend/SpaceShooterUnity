using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HangarUpgradeUIManager : MonoBehaviour
{
    [Header("Price Display")]
    [SerializeField] private TextMeshProUGUI healthPriceText;
    [SerializeField] private TextMeshProUGUI armorPriceText;
    [SerializeField] private TextMeshProUGUI speedPriceText;
    [SerializeField] private TextMeshProUGUI damagePriceText;

    [Header("Crystal Display")]
    [SerializeField] private TextMeshProUGUI totalCrystalsText;

    [Header("Buttons")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button armorUpgradeButton;
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private Button damageUpgradeButton;

    [Header("Upgrade Settings")]
    [SerializeField] private int basePricePerUpgrade = 150;
    [SerializeField] private float priceMultiplier = 1.5f;

    [Header("Level Display")]
    [SerializeField] private TextMeshProUGUI healthLevelText;
    [SerializeField] private TextMeshProUGUI armorLevelText;
    [SerializeField] private TextMeshProUGUI speedLevelText;
    [SerializeField] private TextMeshProUGUI damageLevelText;

    private void Start()
    {
        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.AddListener(() => SelectUpgrade(PowerupType.ExtraLife));

        if (armorUpgradeButton != null)
            armorUpgradeButton.onClick.AddListener(() => SelectUpgrade(PowerupType.Shield));

        if (speedUpgradeButton != null)
            speedUpgradeButton.onClick.AddListener(() => SelectUpgrade(PowerupType.RapidFire));

        if (damageUpgradeButton != null)
            damageUpgradeButton.onClick.AddListener(() => SelectUpgrade(PowerupType.Bomb));

        RefreshUpgradeUI();
    }

    public void RefreshUpgradeUI()
    {
        if (ShipStatsManager.Instance == null || GameManager.Instance == null)
        {
            return;
        }

        UpdateUpgradePrices();
        UpdateUpgradeLevels();
        UpdateButtonStates();
        UpdateCrystalDisplay();
    }

    private void UpdateUpgradePrices()
    {
        var statsManager = ShipStatsManager.Instance;

        if (healthPriceText != null)
        {
            int price = CalculateUpgradePrice(PowerupType.ExtraLife);
            if (statsManager.CanUpgrade(PowerupType.ExtraLife))
            {
                healthPriceText.text = price.ToString();
            }
            else
            {
                healthPriceText.text = "MAX";
            }
        }

        if (armorPriceText != null)
        {
            int price = CalculateUpgradePrice(PowerupType.Shield);
            if (statsManager.CanUpgrade(PowerupType.Shield))
            {
                armorPriceText.text = price.ToString();
            }
            else
            {
                armorPriceText.text = "MAX";
            }
        }

        if (speedPriceText != null)
        {
            int price = CalculateUpgradePrice(PowerupType.RapidFire);
            if (statsManager.CanUpgrade(PowerupType.RapidFire))
            {
                speedPriceText.text = price.ToString();
            }
            else
            {
                speedPriceText.text = "MAX";
            }
        }

        if (damagePriceText != null)
        {
            int price = CalculateUpgradePrice(PowerupType.Bomb);
            if (statsManager.CanUpgrade(PowerupType.Bomb))
            {
                damagePriceText.text = price.ToString();
            }
            else
            {
                damagePriceText.text = "MAX";
            }
        }
    }

    private int CalculateUpgradePrice(PowerupType type)
    {
        var statsManager = ShipStatsManager.Instance;
        int currentLevel = statsManager.GetUpgradeLevel(type);

        int price = Mathf.RoundToInt(basePricePerUpgrade * Mathf.Pow(priceMultiplier, currentLevel));
        return price;
    }

    private void UpdateButtonStates()
    {
        var statsManager = ShipStatsManager.Instance;
        var gameManager = GameManager.Instance;
        int availableCrystals = gameManager.GetTotalCrystals();

        if (healthUpgradeButton != null)
        {
            bool canUpgrade = statsManager.CanUpgrade(PowerupType.ExtraLife);
            int price = CalculateUpgradePrice(PowerupType.ExtraLife);
            bool canAfford = availableCrystals >= price;

            healthUpgradeButton.interactable = canUpgrade && canAfford;
        }

        if (armorUpgradeButton != null)
        {
            bool canUpgrade = statsManager.CanUpgrade(PowerupType.Shield);
            int price = CalculateUpgradePrice(PowerupType.Shield);
            bool canAfford = availableCrystals >= price;

            armorUpgradeButton.interactable = canUpgrade && canAfford;
        }

        if (speedUpgradeButton != null)
        {
            bool canUpgrade = statsManager.CanUpgrade(PowerupType.RapidFire);
            int price = CalculateUpgradePrice(PowerupType.RapidFire);
            bool canAfford = availableCrystals >= price;

            speedUpgradeButton.interactable = canUpgrade && canAfford;
        }

        if (damageUpgradeButton != null)
        {
            bool canUpgrade = statsManager.CanUpgrade(PowerupType.Bomb);
            int price = CalculateUpgradePrice(PowerupType.Bomb);
            bool canAfford = availableCrystals >= price;

            damageUpgradeButton.interactable = canUpgrade && canAfford;
        }
    }

    private void UpdateCrystalDisplay()
    {
        if (totalCrystalsText != null && GameManager.Instance != null)
        {
            int totalCrystalsCollected = GameManager.Instance.GetTotalCrystalsCollected();
            totalCrystalsText.text = totalCrystalsCollected.ToString();
        }
    }

    public void SelectUpgrade(PowerupType upgradeType)
    {
        var statsManager = ShipStatsManager.Instance;
        var gameManager = GameManager.Instance;

        if (statsManager == null || gameManager == null)
        {
            return;
        }

        if (!statsManager.CanUpgrade(upgradeType))
        {
            return;
        }

        int price = CalculateUpgradePrice(upgradeType);

        if (gameManager.GetTotalCrystals() < price)
        {
            return;
        }

        PlayerPrefs.SetString("SelectedUpgrade", upgradeType.ToString());
        PlayerPrefs.SetInt("SelectedUpgradePrice", price);
        PlayerPrefs.Save();
    }

    public bool PurchaseSelectedUpgrade()
    {
        var statsManager = ShipStatsManager.Instance;
        var gameManager = GameManager.Instance;

        if (statsManager == null || gameManager == null)
        {
            return false;
        }

        string upgradeTypeString = PlayerPrefs.GetString("SelectedUpgrade", "");
        int price = PlayerPrefs.GetInt("SelectedUpgradePrice", 0);

        if (string.IsNullOrEmpty(upgradeTypeString))
        {
            return false;
        }

        if (!System.Enum.TryParse<PowerupType>(upgradeTypeString, out PowerupType upgradeType))
        {
            return false;
        }

        if (!statsManager.CanUpgrade(upgradeType))
        {
            return false;
        }

        if (!gameManager.SpendTotalCrystals(price))
        {
            return false;
        }

        bool success = statsManager.UpgradeCharacteristic(upgradeType);

        if (success)
        {
            RefreshUpgradeUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetSelectedUpgradePrice()
    {
        return PlayerPrefs.GetInt("SelectedUpgradePrice", 0);
    }

    public void OnHangarPanelOpened()
    {
        RefreshUpgradeUI();
    }

    public void OnUpgradePanelOpened()
    {
        RefreshUpgradeUI();
    }

    private void UpdateUpgradeLevels()
    {
        var statsManager = ShipStatsManager.Instance;
        int maxLevel = statsManager.GetMaxUpgradeLevel();

        if (healthLevelText != null)
        {
            int healthLevel = statsManager.GetUpgradeLevel(PowerupType.ExtraLife);
            healthLevelText.text = $"{healthLevel}/{maxLevel}";
        }

        if (armorLevelText != null)
        {
            int armorLevel = statsManager.GetUpgradeLevel(PowerupType.Shield);
            armorLevelText.text = $"{armorLevel}/{maxLevel}";
        }

        if (speedLevelText != null)
        {
            int speedLevel = statsManager.GetUpgradeLevel(PowerupType.RapidFire);
            speedLevelText.text = $"{speedLevel}/{maxLevel}";
        }

        if (damageLevelText != null)
        {
            int damageLevel = statsManager.GetUpgradeLevel(PowerupType.Bomb);
            damageLevelText.text = $"{damageLevel}/{maxLevel}";
        }
    }
}