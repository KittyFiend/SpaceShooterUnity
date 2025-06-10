using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HangarConfirmationManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;

    private void Start()
    {
    }

    public void OnConfirmationPanelOpened()
    {
        UpdateConfirmationUI();
    }

    private void UpdateConfirmationUI()
    {
        int upgradePrice = PlayerPrefs.GetInt("SelectedUpgradePrice", 0);
        string upgradeTypeString = PlayerPrefs.GetString("SelectedUpgrade", "");

        if (priceText != null)
        {
            priceText.text = upgradePrice.ToString();
        }

        if (upgradeDescriptionText != null)
        {
            string description = GetUpgradeDescription(upgradeTypeString);
            upgradeDescriptionText.text = description;
        }
    }

    private string GetUpgradeDescription(string upgradeTypeString)
    {
        switch (upgradeTypeString)
        {
            case "ExtraLife":
                return "HEALTH UPGRADE\n\n" +
                       "Increases starting health by +1 HP\n" +
                       "You will be able to take more hits\n" +
                       "before being destroyed";

            case "Shield":
                return "ARMOR UPGRADE\n\n" +
                       "Increases shield duration by +1 second\n" +
                       "Shield powerups will last longer\n" +
                       "providing better protection";

            case "RapidFire":
                return "SPEED UPGRADE\n\n" +
                       "Increases firing speed\n" +
                       "Shorter delay between shots\n" +
                       "allowing faster enemy destruction";

            case "Bomb":
                return "DAMAGE UPGRADE\n\n" +
                       "Increases weapon damage by +1\n" +
                       "Your projectiles will deal more damage\n" +
                       "to enemies and bosses";

            default:
                return "UNKNOWN UPGRADE\n\nNo description available";
        }
    }

    public bool ProcessConfirmation()
    {
        return PurchaseUpgradeDirectly();
    }

    private bool PurchaseUpgradeDirectly()
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
            RefreshUpgradeUIWhenNeeded();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RefreshUpgradeUIWhenNeeded()
    {
        PlayerPrefs.SetInt("NeedRefreshUpgradeUI", 1);
        PlayerPrefs.Save();
    }

    public void OnCancelPressed()
    {
        PlayerPrefs.DeleteKey("SelectedUpgrade");
        PlayerPrefs.DeleteKey("SelectedUpgradePrice");
    }

    public void OnInfoPressed()
    {
    }
}