using UnityEngine;
using UnityEngine.UI;

public class SettingsNavigationManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button hangarButton;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject[] mainMenuElements;

    [Header("Settings Panels")]
    [SerializeField] private GameObject settingsCategoriesPanel;
    [SerializeField] private GameObject soundSettingsPanel;
    [SerializeField] private GameObject graphicsSettingsPanel;
    [SerializeField] private GameObject controlsSettingsPanel;

    [Header("Hangar Panels")]
    [SerializeField] private GameObject hangarPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject infoPanel;

    [Header("Category Buttons")]
    [SerializeField] private Button soundCategoryButton;
    [SerializeField] private Button graphicsCategoryButton;
    [SerializeField] private Button controlsCategoryButton;

    [Header("Hangar Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button armorButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button damageButton;

    [Header("Confirmation Panel Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button infoConfirmButton;
    [SerializeField] private Button confirmButton;

    [Header("Info Panel Buttons")]
    [SerializeField] private Button infoBackButton;

    public enum NavigationState
    {
        MainMenu,
        SettingsCategories,
        SoundSettings,
        GraphicsSettings,
        ControlsSettings,
        HangarMain,
        HangarUpgrade,
        HangarConfirmation,
        HangarInfo
    }

    private NavigationState currentState = NavigationState.MainMenu;

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);

        if (hangarButton != null)
            hangarButton.onClick.AddListener(OnHangarButtonPressed);

        if (soundCategoryButton != null)
            soundCategoryButton.onClick.AddListener(OnSoundCategoryPressed);

        if (graphicsCategoryButton != null)
            graphicsCategoryButton.onClick.AddListener(OnGraphicsCategoryPressed);

        if (controlsCategoryButton != null)
            controlsCategoryButton.onClick.AddListener(OnControlsCategoryPressed);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);

        if (healthButton != null)
            healthButton.onClick.AddListener(() => OnUpgradeItemPressed("Health"));

        if (armorButton != null)
            armorButton.onClick.AddListener(() => OnUpgradeItemPressed("Armor"));

        if (speedButton != null)
            speedButton.onClick.AddListener(() => OnUpgradeItemPressed("Speed"));

        if (damageButton != null)
            damageButton.onClick.AddListener(() => OnUpgradeItemPressed("Damage"));

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButtonPressed);

        if (infoConfirmButton != null)
            infoConfirmButton.onClick.AddListener(OnInfoConfirmButtonPressed);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonPressed);

        if (infoBackButton != null)
            infoBackButton.onClick.AddListener(OnInfoBackButtonPressed);

        SetState(NavigationState.MainMenu);
    }

    public void OnSettingsButtonPressed()
    {
        SetState(NavigationState.SettingsCategories);
    }

    public void OnHangarButtonPressed()
    {
        SetState(NavigationState.HangarMain);
    }

    public void OnBackButtonPressed()
    {
        switch (currentState)
        {
            case NavigationState.SettingsCategories:
                SetState(NavigationState.MainMenu);
                break;

            case NavigationState.SoundSettings:
            case NavigationState.GraphicsSettings:
            case NavigationState.ControlsSettings:
                SetState(NavigationState.SettingsCategories);
                break;

            case NavigationState.HangarMain:
                SetState(NavigationState.MainMenu);
                break;

            case NavigationState.HangarUpgrade:
                SetState(NavigationState.HangarMain);
                break;

            case NavigationState.HangarConfirmation:
                SetState(NavigationState.HangarUpgrade);
                break;

            case NavigationState.HangarInfo:
                SetState(NavigationState.HangarConfirmation);
                break;

            default:
                SetState(NavigationState.MainMenu);
                break;
        }
    }

    public void OnSoundCategoryPressed()
    {
        SetState(NavigationState.SoundSettings);
    }

    public void OnGraphicsCategoryPressed()
    {
        SetState(NavigationState.GraphicsSettings);
    }

    public void OnControlsCategoryPressed()
    {
        SetState(NavigationState.ControlsSettings);
    }

    public void OnUpgradeButtonPressed()
    {
        SetState(NavigationState.HangarUpgrade);
    }

    public void OnUpgradeItemPressed(string upgradeType)
    {
        PlayerPrefs.SetString("SelectedUpgrade", upgradeType);
        SetState(NavigationState.HangarConfirmation);
    }

    public void OnCancelButtonPressed()
    {
        SetState(NavigationState.HangarUpgrade);
    }

    public void OnInfoConfirmButtonPressed()
    {
        SetState(NavigationState.HangarInfo);
    }

    public void OnConfirmButtonPressed()
    {
        HangarConfirmationManager confirmationManager = FindObjectOfType<HangarConfirmationManager>();
        if (confirmationManager != null)
        {
            bool purchaseSuccess = confirmationManager.ProcessConfirmation();

            if (purchaseSuccess)
            {
                SetState(NavigationState.HangarUpgrade);
            }
        }
        else
        {
            SetState(NavigationState.HangarUpgrade);
        }
    }

    public void OnInfoBackButtonPressed()
    {
        SetState(NavigationState.HangarConfirmation);
    }

    private void SetState(NavigationState newState)
    {
        currentState = newState;

        HideAllPanels();

        switch (newState)
        {
            case NavigationState.MainMenu:
                ShowMainMenu();
                break;

            case NavigationState.SettingsCategories:
                ShowSettingsCategories();
                break;

            case NavigationState.SoundSettings:
                ShowSoundSettings();
                break;

            case NavigationState.GraphicsSettings:
                ShowGraphicsSettings();
                break;

            case NavigationState.ControlsSettings:
                ShowControlsSettings();
                break;

            case NavigationState.HangarMain:
                ShowHangarMain();
                break;

            case NavigationState.HangarUpgrade:
                ShowHangarUpgrade();
                break;

            case NavigationState.HangarConfirmation:
                ShowHangarConfirmation();
                break;

            case NavigationState.HangarInfo:
                ShowHangarInfo();
                break;
        }
    }

    private void HideAllPanels()
    {
        foreach (var element in mainMenuElements)
        {
            if (element != null)
                element.SetActive(false);
        }

        if (settingsCategoriesPanel != null) settingsCategoriesPanel.SetActive(false);
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(false);
        if (graphicsSettingsPanel != null) graphicsSettingsPanel.SetActive(false);
        if (controlsSettingsPanel != null) controlsSettingsPanel.SetActive(false);

        if (hangarPanel != null) hangarPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);

        if (backButton != null) backButton.gameObject.SetActive(false);
    }

    private void ShowMainMenu()
    {
        foreach (var element in mainMenuElements)
        {
            if (element != null)
                element.SetActive(true);
        }

        if (backButton != null) backButton.gameObject.SetActive(false);
    }

    private void ShowSettingsCategories()
    {
        if (settingsCategoriesPanel != null) settingsCategoriesPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowSoundSettings()
    {
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowGraphicsSettings()
    {
        if (graphicsSettingsPanel != null) graphicsSettingsPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowControlsSettings()
    {
        if (controlsSettingsPanel != null) controlsSettingsPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowHangarMain()
    {
        if (hangarPanel != null)
        {
            hangarPanel.SetActive(true);

            HangarUpgradeUIManager upgradeUIManager = FindObjectOfType<HangarUpgradeUIManager>();
            if (upgradeUIManager != null)
            {
                upgradeUIManager.RefreshUpgradeUI();
            }
        }
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowHangarUpgrade()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);

            HangarUpgradeUIManager upgradeUIManager = FindObjectOfType<HangarUpgradeUIManager>();
            if (upgradeUIManager != null)
            {
                upgradeUIManager.OnUpgradePanelOpened();
            }
        }
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowHangarConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);

            HangarConfirmationManager confirmationManager = FindObjectOfType<HangarConfirmationManager>();
            if (confirmationManager != null)
            {
                confirmationManager.OnConfirmationPanelOpened();
            }
        }
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowHangarInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);

            HangarInfoManager infoManager = FindObjectOfType<HangarInfoManager>();
            if (infoManager != null)
            {
                infoManager.OnInfoPanelOpened();
            }
        }
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    public NavigationState GetCurrentState()
    {
        return currentState;
    }

    public void ReturnToMainMenu()
    {
        SetState(NavigationState.MainMenu);
    }

    public bool IsInSettings()
    {
        return currentState == NavigationState.SettingsCategories ||
               currentState == NavigationState.SoundSettings ||
               currentState == NavigationState.GraphicsSettings ||
               currentState == NavigationState.ControlsSettings;
    }

    public bool IsInHangar()
    {
        return currentState == NavigationState.HangarMain ||
               currentState == NavigationState.HangarUpgrade ||
               currentState == NavigationState.HangarConfirmation ||
               currentState == NavigationState.HangarInfo;
    }

    public bool IsInMainMenu()
    {
        return currentState == NavigationState.MainMenu;
    }
}