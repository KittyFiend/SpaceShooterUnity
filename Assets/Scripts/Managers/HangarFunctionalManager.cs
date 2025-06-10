using UnityEngine;
using TMPro;

public class HangarFunctionalManager : MonoBehaviour
{
    [Header("Ship Stats Display")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI damageText;

    private ShipStatsManager shipStats;

    private void Start()
    {
        StartCoroutine(WaitForShipStatsManager());
    }

    private System.Collections.IEnumerator WaitForShipStatsManager()
    {
        while (ShipStatsManager.Instance == null)
        {
            yield return null;
        }
        shipStats = ShipStatsManager.Instance;

        RefreshHangarData();
    }

    public void RefreshHangarData()
    {
        if (shipStats == null)
        {
            return;
        }

        int healthLevel = shipStats.GetUpgradeLevel(PowerupType.ExtraLife);
        int armorLevel = shipStats.GetUpgradeLevel(PowerupType.Shield);
        int speedLevel = shipStats.GetUpgradeLevel(PowerupType.RapidFire);
        int damageLevel = shipStats.GetUpgradeLevel(PowerupType.Bomb);

        int maxLevel = shipStats.GetMaxUpgradeLevel();

        if (healthText != null)
            healthText.text = $"{healthLevel}/{maxLevel}";

        if (armorText != null)
            armorText.text = $"{armorLevel}/{maxLevel}";

        if (speedText != null)
            speedText.text = $"{speedLevel}/{maxLevel}";

        if (damageText != null)
            damageText.text = $"{damageLevel}/{maxLevel}";
    }

    public string GetStatDescription(PowerupType type)
    {
        if (shipStats == null) return "Завантаження...";

        return shipStats.GetUpgradeDescription(type);
    }

    public (int current, int max, string finalValue) GetStatInfo(PowerupType type)
    {
        if (shipStats == null) return (0, 0, "N/A");

        int current = shipStats.GetUpgradeLevel(type);
        int max = shipStats.GetMaxUpgradeLevel();

        string finalValue = "";
        switch (type)
        {
            case PowerupType.ExtraLife:
                finalValue = $"{shipStats.GetFinalHealth()} HP";
                break;
            case PowerupType.Shield:
                finalValue = $"{shipStats.GetFinalShieldDuration()}s";
                break;
            case PowerupType.RapidFire:
                finalValue = $"{(1f / shipStats.GetFinalFireRate()):F1}/s";
                break;
            case PowerupType.Bomb:
                finalValue = $"{(shipStats.GetFinalDamageMultiplier() * 100):F0}%";
                break;
        }

        return (current, max, finalValue);
    }

    public void ApplyUpgradesToPlayer()
    {
        if (shipStats == null)
        {
            return;
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            shipStats.ApplyUpgradesToPlayer(player);
        }

        HealthShieldBarController healthBar = FindObjectOfType<HealthShieldBarController>();
        if (healthBar != null)
        {
            shipStats.ApplyHealthUpgradeToHealthBar(healthBar);
        }
    }
}