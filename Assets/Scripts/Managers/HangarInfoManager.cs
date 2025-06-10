using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HangarInfoManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI infoContentText;

    private void Start()
    {
    }

    public void OnInfoPanelOpened()
    {
        UpdateInfoUI();
    }

    private void UpdateInfoUI()
    {
        string upgradeTypeString = PlayerPrefs.GetString("SelectedUpgrade", "");

        if (string.IsNullOrEmpty(upgradeTypeString))
        {
            ShowErrorInfo();
            return;
        }

        if (!System.Enum.TryParse<PowerupType>(upgradeTypeString, out PowerupType upgradeType))
        {
            ShowErrorInfo();
            return;
        }

        string detailedInfo = GetDetailedUpgradeInfo(upgradeType);

        if (infoContentText != null)
        {
            infoContentText.text = detailedInfo;
        }
    }

    private string GetDetailedUpgradeInfo(PowerupType upgradeType)
    {
        if (ShipStatsManager.Instance == null || GameManager.Instance == null)
        {
            return "INFORMATION UNAVAILABLE\n\nManagers not found";
        }

        var statsManager = ShipStatsManager.Instance;
        var gameManager = GameManager.Instance;

        int currentLevel = statsManager.GetUpgradeLevel(upgradeType);
        int maxLevel = statsManager.GetMaxUpgradeLevel();
        int price = PlayerPrefs.GetInt("SelectedUpgradePrice", 0);
        int playerCrystals = gameManager.GetTotalCrystals();

        string info = "";

        switch (upgradeType)
        {
            case PowerupType.ExtraLife:
                int currentHealth = statsManager.GetFinalHealth();
                int nextHealth = currentHealth + 1;

                info = "HEALTH UPGRADE\n\n" +
                       $"Current Health: {currentHealth} HP\n" +
                       $"After Upgrade: {nextHealth} HP\n\n" +
                       "DESCRIPTION:\n" +
                       "Increases your starting health by +1 HP.\n" +
                       "This means you can take more hits from\n" +
                       "enemies before being destroyed.\n\n" +
                       $"Upgrade Level: {currentLevel}/{maxLevel}\n" +
                       $"Cost: {price} crystals\n" +
                       $"Your Crystals: {playerCrystals}";
                break;

            case PowerupType.Shield:
                float currentShield = statsManager.GetFinalShieldDuration();
                float nextShield = currentShield + 1f;

                info = "ARMOR UPGRADE\n\n" +
                       $"Current Shield Duration: {currentShield} sec\n" +
                       $"After Upgrade: {nextShield} sec\n\n" +
                       "DESCRIPTION:\n" +
                       "Increases shield powerup duration by +1 second.\n" +
                       "When you collect a shield powerup, it will\n" +
                       "provide protection for a longer time.\n\n" +
                       $"Upgrade Level: {currentLevel}/{maxLevel}\n" +
                       $"Cost: {price} crystals\n" +
                       $"Your Crystals: {playerCrystals}";
                break;

            case PowerupType.RapidFire:
                float currentFireRate = statsManager.GetFinalFireRate();
                float currentShotsPerSec = 1f / currentFireRate;
                float nextFireRate = Mathf.Max(0.1f, currentFireRate - 0.02f);
                float nextShotsPerSec = 1f / nextFireRate;

                info = "SPEED UPGRADE\n\n" +
                       $"Current Firing Speed: {currentShotsPerSec:F1} shots/sec\n" +
                       $"After Upgrade: {nextShotsPerSec:F1} shots/sec\n\n" +
                       "DESCRIPTION:\n" +
                       "Increases your firing speed by reducing\n" +
                       "the time between shots by 0.02 seconds.\n" +
                       "Allows you to destroy enemies faster.\n\n" +
                       $"Upgrade Level: {currentLevel}/{maxLevel}\n" +
                       $"Cost: {price} crystals\n" +
                       $"Your Crystals: {playerCrystals}";
                break;

            case PowerupType.Bomb:
                float currentDamage = statsManager.GetFinalDamageMultiplier();
                float nextDamage = currentDamage + 0.2f;

                info = "DAMAGE UPGRADE\n\n" +
                       $"Current Damage: {(currentDamage * 100):F0}%\n" +
                       $"After Upgrade: {(nextDamage * 100):F0}%\n\n" +
                       "DESCRIPTION:\n" +
                       "Increases your weapon damage by +20%.\n" +
                       "Your projectiles will deal more damage\n" +
                       "to enemies and bosses.\n\n" +
                       $"Upgrade Level: {currentLevel}/{maxLevel}\n" +
                       $"Cost: {price} crystals\n" +
                       $"Your Crystals: {playerCrystals}";
                break;

            default:
                info = "UNKNOWN UPGRADE\n\nNo information available";
                break;
        }

        if (playerCrystals >= price)
        {
            info += "\n\nYOU CAN AFFORD THIS UPGRADE!";
        }
        else
        {
            int needed = price - playerCrystals;
            info += $"\n\nYOU NEED {needed} MORE CRYSTALS";
        }

        return info;
    }

    private void ShowErrorInfo()
    {
        if (infoContentText != null)
        {
            infoContentText.text = "ERROR\n\nNo upgrade selected.\nPlease select an upgrade first.";
        }
    }

    public void OnBackPressed()
    {
    }
}