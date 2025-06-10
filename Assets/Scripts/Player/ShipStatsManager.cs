using UnityEngine;

public class ShipStatsManager : MonoBehaviour
{
    public static ShipStatsManager Instance { get; private set; }

    [Header("Upgrade Settings")]
    [SerializeField] private int maxUpgradeLevel = 5;

    [Header("Base Stats")]
    [SerializeField] private int baseHealth = 3;
    [SerializeField] private float baseShieldDuration = 5f;
    [SerializeField] private float baseFireRate = 0.2f;
    [SerializeField] private float baseDamageMultiplier = 1f;

    private int healthUpgradeLevel = 0;
    private int armorUpgradeLevel = 0;
    private int speedUpgradeLevel = 0;
    private int damageUpgradeLevel = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadUpgrades()
    {
        healthUpgradeLevel = PlayerPrefs.GetInt("UpgradeLevel_Health", 0);
        armorUpgradeLevel = PlayerPrefs.GetInt("UpgradeLevel_Armor", 0);
        speedUpgradeLevel = PlayerPrefs.GetInt("UpgradeLevel_Speed", 0);
        damageUpgradeLevel = PlayerPrefs.GetInt("UpgradeLevel_Damage", 0);
    }

    private void SaveUpgrades()
    {
        PlayerPrefs.SetInt("UpgradeLevel_Health", healthUpgradeLevel);
        PlayerPrefs.SetInt("UpgradeLevel_Armor", armorUpgradeLevel);
        PlayerPrefs.SetInt("UpgradeLevel_Speed", speedUpgradeLevel);
        PlayerPrefs.SetInt("UpgradeLevel_Damage", damageUpgradeLevel);
        PlayerPrefs.Save();
    }

    public int GetUpgradeLevel(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.ExtraLife: return healthUpgradeLevel;
            case PowerupType.Shield: return armorUpgradeLevel;
            case PowerupType.RapidFire: return speedUpgradeLevel;
            case PowerupType.Bomb: return damageUpgradeLevel;
            default: return 0;
        }
    }

    public bool UpgradeCharacteristic(PowerupType type)
    {
        int currentLevel = GetUpgradeLevel(type);

        if (currentLevel >= maxUpgradeLevel)
        {
            return false;
        }

        switch (type)
        {
            case PowerupType.ExtraLife:
                healthUpgradeLevel++;
                break;
            case PowerupType.Shield:
                armorUpgradeLevel++;
                break;
            case PowerupType.RapidFire:
                speedUpgradeLevel++;
                break;
            case PowerupType.Bomb:
                damageUpgradeLevel++;
                break;
        }

        SaveUpgrades();

        return true;
    }

    public bool CanUpgrade(PowerupType type)
    {
        return GetUpgradeLevel(type) < maxUpgradeLevel;
    }

    public int GetMaxUpgradeLevel()
    {
        return maxUpgradeLevel;
    }

    public int GetFinalHealth()
    {
        return baseHealth + healthUpgradeLevel;
    }

    public float GetFinalShieldDuration()
    {
        return baseShieldDuration + armorUpgradeLevel;
    }

    public float GetFinalFireRate()
    {
        float improvement = speedUpgradeLevel * 0.02f;
        return Mathf.Max(0.1f, baseFireRate - improvement);
    }

    public float GetFinalDamageMultiplier()
    {
        return baseDamageMultiplier + (damageUpgradeLevel * 0.2f);
    }

    public string GetUpgradeDescription(PowerupType type)
    {
        int currentLevel = GetUpgradeLevel(type);

        switch (type)
        {
            case PowerupType.ExtraLife:
                return $"Збільшує стартове здоров'я на +1 HP\nПоточне: {GetFinalHealth()} HP";

            case PowerupType.Shield:
                return $"Збільшує тривалість щита на +1 сек\nПоточна: {GetFinalShieldDuration()} сек";

            case PowerupType.RapidFire:
                return $"Збільшує швидкість стрільби\nПоточна: {(1f / GetFinalFireRate()):F1} пострілів/сек";

            case PowerupType.Bomb:
                return $"Збільшує урон на +20%\nПоточний: {(GetFinalDamageMultiplier() * 100):F0}%";

            default:
                return "Невідоме покращення";
        }
    }

    public void ApplyUpgradesToPlayer(PlayerController player)
    {
        if (player == null)
        {
            return;
        }

        ApplyFireRateUpgrade(player);
    }

    private void ApplyFireRateUpgrade(PlayerController player)
    {
        var fireRateField = typeof(PlayerController).GetField("fireRate",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fireRateField != null)
        {
            fireRateField.SetValue(player, GetFinalFireRate());
        }
    }

    public void ApplyHealthUpgradeToHealthBar(HealthShieldBarController healthBar)
    {
        if (healthBar == null)
        {
            return;
        }

        int finalHealth = GetFinalHealth();
        healthBar.SetHealthUpgrade(healthUpgradeLevel);
    }

    public void ApplyShieldUpgradeToHealthBar(HealthShieldBarController healthBar)
    {
        if (healthBar == null)
        {
            return;
        }

        healthBar.SetShieldUpgrade(armorUpgradeLevel);
    }

    [ContextMenu("Reset All Upgrades")]
    public void ResetAllUpgrades()
    {
        healthUpgradeLevel = 0;
        armorUpgradeLevel = 0;
        speedUpgradeLevel = 0;
        damageUpgradeLevel = 0;
        SaveUpgrades();
    }
}