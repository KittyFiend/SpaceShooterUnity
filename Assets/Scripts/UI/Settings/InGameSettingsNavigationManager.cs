using UnityEngine;
using UnityEngine.UI;

public class InGameSettingsNavigationManager : MonoBehaviour
{
    [Header("Back Button")]
    [SerializeField] private Button backButton;

    [Header("Settings Panels")]
    [SerializeField] private GameObject settingsCategoriesPanel;
    [SerializeField] private GameObject soundSettingsPanel;
    [SerializeField] private GameObject graphicsSettingsPanel;
    [SerializeField] private GameObject controlsSettingsPanel;

    [Header("Category Buttons")]
    [SerializeField] private Button soundCategoryButton;
    [SerializeField] private Button graphicsCategoryButton;
    [SerializeField] private Button controlsCategoryButton;

    public enum InGameSettingsState
    {
        SettingsCategories,
        SoundSettings,
        GraphicsSettings,
        ControlsSettings
    }

    private InGameSettingsState currentState = InGameSettingsState.SettingsCategories;

    private void Start()
    {
        SetupButtons();

        HideAllPanels();
        if (backButton != null)
            backButton.gameObject.SetActive(false);
    }

    private void SetupButtons()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);

        if (soundCategoryButton != null)
            soundCategoryButton.onClick.AddListener(OnSoundCategoryPressed);

        if (graphicsCategoryButton != null)
            graphicsCategoryButton.onClick.AddListener(OnGraphicsCategoryPressed);

        if (controlsCategoryButton != null)
            controlsCategoryButton.onClick.AddListener(OnControlsCategoryPressed);
    }

    public void ShowSettingsCategories()
    {
        SetState(InGameSettingsState.SettingsCategories);
    }

    public void HideSettingsAndReturnToPause()
    {
        HideAllPanels();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ReturnFromSettingsToPause();
        }
    }

    public void OnSoundCategoryPressed()
    {
        SetState(InGameSettingsState.SoundSettings);
    }

    public void OnGraphicsCategoryPressed()
    {
        SetState(InGameSettingsState.GraphicsSettings);
    }

    public void OnControlsCategoryPressed()
    {
        SetState(InGameSettingsState.ControlsSettings);
    }

    public void OnBackButtonPressed()
    {
        switch (currentState)
        {
            case InGameSettingsState.SoundSettings:
            case InGameSettingsState.GraphicsSettings:
            case InGameSettingsState.ControlsSettings:
                SetState(InGameSettingsState.SettingsCategories);
                break;

            case InGameSettingsState.SettingsCategories:
                HideSettingsAndReturnToPause();
                break;
        }
    }

    private void SetState(InGameSettingsState newState)
    {
        currentState = newState;

        HideAllPanels();

        switch (newState)
        {
            case InGameSettingsState.SettingsCategories:
                ShowSettingsCategoriesPanel();
                break;

            case InGameSettingsState.SoundSettings:
                ShowSoundSettingsPanel();
                break;

            case InGameSettingsState.GraphicsSettings:
                ShowGraphicsSettingsPanel();
                break;

            case InGameSettingsState.ControlsSettings:
                ShowControlsSettingsPanel();
                break;
        }
    }

    private void HideAllPanels()
    {
        if (settingsCategoriesPanel != null) settingsCategoriesPanel.SetActive(false);
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(false);
        if (graphicsSettingsPanel != null) graphicsSettingsPanel.SetActive(false);
        if (controlsSettingsPanel != null) controlsSettingsPanel.SetActive(false);

        if (backButton != null) backButton.gameObject.SetActive(false);
    }

    private void ShowSettingsCategoriesPanel()
    {
        if (settingsCategoriesPanel != null) settingsCategoriesPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowSoundSettingsPanel()
    {
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowGraphicsSettingsPanel()
    {
        if (graphicsSettingsPanel != null) graphicsSettingsPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    private void ShowControlsSettingsPanel()
    {
        if (controlsSettingsPanel != null) controlsSettingsPanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
    }

    public InGameSettingsState GetCurrentState()
    {
        return currentState;
    }

    public bool IsInSettings()
    {
        return true;
    }
}