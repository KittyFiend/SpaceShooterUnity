using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [Header("Screen References")]
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject levelsScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private GameObject gameHUDScreen;

    private Stack<GameObject> screenHistory = new Stack<GameObject>();
    private GameObject currentScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideAllPanels();
        ShowScreen(mainMenuScreen);
    }

    private void HideAllPanels()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            foreach (Transform child in canvas.transform)
            {
                if (child.name.Contains("Panel"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowScreen(GameObject screen)
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }

        if (screen != null)
        {
            screen.SetActive(true);
            currentScreen = screen;
        }

        if (AudioManager.Instance != null)
        {
        }
    }

    public void ShowScreenWithHistory(GameObject screen)
    {
        if (currentScreen != null)
        {
            screenHistory.Push(currentScreen);
        }

        ShowScreen(screen);
    }

    public void GoBack()
    {
        if (screenHistory.Count > 0)
        {
            GameObject previousScreen = screenHistory.Pop();
            ShowScreen(previousScreen);
        }
    }

    public void ClearHistory()
    {
        screenHistory.Clear();
    }
}