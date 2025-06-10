using UnityEngine;
using UnityEngine.UI;

public class MainMenuNavigationManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button backButton;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button hangarButton;

    [Header("Settings Buttons")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button infoMenuButton;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject[] mainMenuElements;
    [SerializeField] private GameObject[] settingsUIElements;

    [Header("Hangar Panels")]
    [SerializeField] private GameObject hangarPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject infoPanel;

    [Header("Hangar Buttons")]
    [SerializeField] private Button upgradeButton;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button armorUpgradeButton;
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private Button damageUpgradeButton;

    [Header("Confirmation Dialog Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button infoButton;

    [Header("Info Dialog Buttons")]
    [SerializeField] private Button infoBackButton;

    public enum MainMenuState
    {
        MainMenu,
        Hangar,
        Upgrade,
        Confirmation,
        Info,
        Settings
    }

    private MainMenuState currentState = MainMenuState.MainMenu;

    private HangarFunctionalManager hangarManager;
    private UpgradeFunctionalManager upgradeManager;

    private void Start()
    {
        hangarManager = FindObjectOfType<HangarFunctionalManager>();
        upgradeManager = FindObjectOfType<UpgradeFunctionalManager>();

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);

        if (hangarButton != null)
            hangarButton.onClick.AddListener(OnHangarButtonPressed);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);

        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.AddListener(() => OnUpgradeTypeButtonPressed(PowerupType.ExtraLife));

        if (armorUpgradeButton != null)
            armorUpgradeButton.onClick.AddListener(() => OnUpgradeTypeButtonPressed(PowerupType.Shield));

        if (speedUpgradeButton != null)
            speedUpgradeButton.onClick.AddListener(() => OnUpgradeTypeButtonPressed(PowerupType.RapidFire));

        if (damageUpgradeButton != null)
            damageUpgradeButton.onClick.AddListener(() => OnUpgradeTypeButtonPressed(PowerupType.Bomb));

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonPressed);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButtonPressed);

        if (infoButton != null)
            infoButton.onClick.AddListener(OnInfoButtonPressed);

        if (infoBackButton != null)
            infoBackButton.onClick.AddListener(OnInfoBackButtonPressed);

        SetState(MainMenuState.MainMenu);
    }

    public void OnHangarButtonPressed()
    {
        SetState(MainMenuState.Hangar);
    }

    public void OnSettingsButtonPressed()
    {
        SettingsNavigationManager settingsNav = FindObjectOfType<SettingsNavigationManager>();
        if (settingsNav != null)
        {
            settingsNav.OnSettingsButtonPressed();
        }
    }

    public void OnUpgradeButtonPressed()
    {
        SetState(MainMenuState.Upgrade);
    }

    public void OnUpgradeTypeButtonPressed(PowerupType upgradeType)
    {
        if (upgradeManager != null)
        {
            upgradeManager.RequestUpgrade(upgradeType);
        }

        SetState(MainMenuState.Confirmation);
    }

    public void OnConfirmButtonPressed()
    {
        if (upgradeManager != null)
        {
            upgradeManager.ConfirmUpgrade();
        }

        SetState(MainMenuState.Upgrade);
    }

    public void OnCancelButtonPressed()
    {
        SetState(MainMenuState.Upgrade);
    }

    public void OnInfoButtonPressed()
    {
        SetState(MainMenuState.Info);
    }

    public void OnInfoBackButtonPressed()
    {
        SetState(MainMenuState.Confirmation);
    }

    public void OnBackButtonPressed()
    {
        switch (currentState)
        {
            case MainMenuState.Hangar:
                SetState(MainMenuState.MainMenu);
                break;

            case MainMenuState.Upgrade:
                SetState(MainMenuState.Hangar);
                break;

            case MainMenuState.Confirmation:
                SetState(MainMenuState.Upgrade);
                break;

            case MainMenuState.Info:
                SetState(MainMenuState.Confirmation);
                break;

            default:
                SetState(MainMenuState.MainMenu);
                break;
        }
    }

    private void SetState(MainMenuState newState)
    {
        currentState = newState;
        HideAllPanels();

        switch (newState)
        {
            case MainMenuState.MainMenu:
                ShowMainMenu();
                break;

            case MainMenuState.Hangar:
                ShowHangar();
                break;

            case MainMenuState.Upgrade:
                ShowUpgrade();
                break;

            case MainMenuState.Confirmation:
                ShowConfirmation();
                break;

            case MainMenuState.Info:
                ShowInfo();
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

        foreach (var element in settingsUIElements)
        {
            if (element != null)
                element.SetActive(false);
        }

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

        foreach (var element in settingsUIElements)
        {
            if (element != null)
                element.SetActive(true);
        }

        if (backButton != null) backButton.gameObject.SetActive(false);
    }

    private void ShowHangar()
    {
        if (hangarPanel != null) hangarPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);

        if (hangarManager != null)
            hangarManager.RefreshHangarData();
    }

    private void ShowUpgrade()
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);

        if (upgradeManager != null)
            upgradeManager.RefreshUpgradeData();
    }

    private void ShowConfirmation()
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (confirmationPanel != null) confirmationPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);

        if (upgradeManager != null)
            upgradeManager.UpdateConfirmationDialog();
    }

    private void ShowInfo()
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (confirmationPanel != null) confirmationPanel.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);

        if (upgradeManager != null)
            upgradeManager.UpdateInfoDialog();
    }

    public MainMenuState GetCurrentState()
    {
        return currentState;
    }

    public void ReturnToMainMenu()
    {
        SetState(MainMenuState.MainMenu);
    }

    public bool IsInMainMenu()
    {
        return currentState == MainMenuState.MainMenu;
    }

    public bool IsInHangar()
    {
        return currentState != MainMenuState.MainMenu;
    }
}