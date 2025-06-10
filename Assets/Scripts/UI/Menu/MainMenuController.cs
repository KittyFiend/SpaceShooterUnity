using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Screen References")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject levelsScreen;
    [SerializeField] private GameObject gameModeSelectPanel;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject gameTitle;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject hangarButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject infoButton;
    [SerializeField] private GameObject settingsButton;

    public void OnPlayButtonClicked()
    {
        HideMainMenuElements();

        if (gameModeSelectPanel != null)
        {
            gameModeSelectPanel.SetActive(true);
        }
        else
        {
            GameObject panel = GameObject.Find("GameModeSelectPanel");
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }
    }

    public void OnInfinityModeClicked()
    {
        PlayerPrefs.SetString("GameMode", "Endless");
        SceneManager.LoadScene("GameScene");
    }

    public void OnLevelModeClicked()
    {
        PlayerPrefs.SetString("GameMode", "Level");
        PlayerPrefs.SetInt("CurrentLevel", 1);
        SceneManager.LoadScene("Level1Scene");
    }

    public void OnBackFromGameModeClicked()
    {
        if (gameModeSelectPanel != null)
        {
            gameModeSelectPanel.SetActive(false);
        }
        else
        {
            GameObject panel = GameObject.Find("GameModeSelectPanel");
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        ShowMainMenuElements();
    }

    private void HideMainMenuElements()
    {
        if (gameTitle != null) gameTitle.SetActive(false);
        if (playButton != null) playButton.SetActive(false);
        if (hangarButton != null) hangarButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);
        if (infoButton != null) infoButton.SetActive(false);
        if (settingsButton != null) settingsButton.SetActive(false);
    }

    private void ShowMainMenuElements()
    {
        if (gameTitle != null) gameTitle.SetActive(true);
        if (playButton != null) playButton.SetActive(true);
        if (hangarButton != null) hangarButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);
        if (infoButton != null) infoButton.SetActive(true);
        if (settingsButton != null) settingsButton.SetActive(true);
    }

    public void OnEndlessModeButtonClicked()
    {
        PlayerPrefs.SetString("GameMode", "Endless");
        SceneManager.LoadScene("GameScene");
    }

    public void OnLevelModeButtonClicked()
    {
        if (ScreenManager.Instance != null && levelsScreen != null)
        {
            ScreenManager.Instance.ShowScreenWithHistory(levelsScreen);
        }
    }

    public void OnSettingsButtonClicked()
    {
        if (ScreenManager.Instance != null && settingsScreen != null)
        {
            ScreenManager.Instance.ShowScreenWithHistory(settingsScreen);
        }
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}