using UnityEngine;
using UnityEngine.UI;

public class HangarNavigationManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button hangarButton;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject[] mainMenuElements;

    [Header("Hangar Panels")]
    [SerializeField] private GameObject hangarPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject infoPanel;

    [Header("Hangar Buttons")]
    [SerializeField] private Button upgradeButton;

    public enum HangarState
    {
        MainMenu,
        Hangar,
        Upgrade,
        Confirmation,
        Info
    }

    private HangarState currentState = HangarState.MainMenu;

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);

        if (hangarButton != null)
            hangarButton.onClick.AddListener(OnHangarButtonPressed);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);

        SetState(HangarState.MainMenu);
    }

    public void OnHangarButtonPressed()
    {
        SetState(HangarState.Hangar);
    }

    public void OnUpgradeButtonPressed()
    {
        SetState(HangarState.Upgrade);
    }

    public void OnBackButtonPressed()
    {
        switch (currentState)
        {
            case HangarState.Hangar:
                SetState(HangarState.MainMenu);
                break;

            case HangarState.Upgrade:
                SetState(HangarState.Hangar);
                break;

            case HangarState.Confirmation:
            case HangarState.Info:
                SetState(HangarState.Upgrade);
                break;

            default:
                SetState(HangarState.MainMenu);
                break;
        }
    }

    private void SetState(HangarState newState)
    {
        currentState = newState;

        HideAllPanels();

        switch (newState)
        {
            case HangarState.MainMenu:
                ShowMainMenu();
                break;

            case HangarState.Hangar:
                ShowHangar();
                break;

            case HangarState.Upgrade:
                ShowUpgrade();
                break;

            case HangarState.Confirmation:
                ShowConfirmation();
                break;

            case HangarState.Info:
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

    private void ShowHangar()
    {
        if (hangarPanel != null) hangarPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowUpgrade()
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowConfirmation()
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (confirmationPanel != null) confirmationPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowInfo()
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    public HangarState GetCurrentState()
    {
        return currentState;
    }

    public void ShowConfirmationDialog()
    {
        SetState(HangarState.Confirmation);
    }

    public void ShowInfoDialog()
    {
        SetState(HangarState.Info);
    }

    public void ReturnToMainMenu()
    {
        SetState(HangarState.MainMenu);
    }

    public bool IsInHangar()
    {
        return currentState != HangarState.MainMenu;
    }
}