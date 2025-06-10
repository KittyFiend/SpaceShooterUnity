using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("NEW HUD System")]
    [SerializeField] private GameHUDController gameHUD;

    [Header("In-Game UI Controls")]
    [SerializeField] private Button pauseButton;

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI pauseScoreText;
    [SerializeField] private Button pauseSettingsButton;
    [SerializeField] private Button pauseMainMenuButton;
    [SerializeField] private Button pauseReplayButton;
    [SerializeField] private Button pauseResumeButton;

    [Header("InGame Settings Navigation")]
    [SerializeField] private InGameSettingsNavigationManager inGameSettingsNav;

    [Header("NEW Game Over System")]
    [SerializeField] private InfinityGameOverController infinityGameOverController;
    [SerializeField] private LevelGameOverController levelGameOverController;

    private bool isPaused = false;
    private bool gameIsActive = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SetupButtons();
        InitializePanels();
        InitializeHUD();

        TestGameOverControllers();
    }

    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (pauseSettingsButton != null)
            pauseSettingsButton.onClick.AddListener(OnPauseSettingsClicked);
        if (pauseMainMenuButton != null)
            pauseMainMenuButton.onClick.AddListener(OnPauseMainMenuClicked);
        if (pauseReplayButton != null)
            pauseReplayButton.onClick.AddListener(OnPauseReplayClicked);
        if (pauseResumeButton != null)
            pauseResumeButton.onClick.AddListener(OnPauseResumeClicked);
    }

    private void InitializePanels()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void InitializeHUD()
    {
        if (gameHUD == null)
        {
            gameHUD = FindObjectOfType<GameHUDController>();
        }

        if (inGameSettingsNav == null)
        {
            inGameSettingsNav = FindObjectOfType<InGameSettingsNavigationManager>();
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void TestGameOverControllers()
    {
        InfinityGameOverController infinityController = FindObjectOfType<InfinityGameOverController>();
        LevelGameOverController levelController = FindObjectOfType<LevelGameOverController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameIsActive)
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Menu) && gameIsActive)
        {
            TogglePause();
        }
    }

    public void UpdateScore(int score)
    {
        if (gameHUD != null)
        {
            gameHUD.UpdateScore(score);
        }
    }

    public void UpdateScoreAnimated(int score)
    {
        if (gameHUD != null)
        {
            gameHUD.AnimateScoreIncrease(score);
        }
    }

    public void UpdateSessionCrystals(int crystals)
    {
        if (gameHUD != null)
        {
            gameHUD.UpdateSessionCrystals(crystals);
        }
    }

    public void UpdateSessionCrystalsAnimated(int crystals)
    {
        if (gameHUD != null)
        {
            gameHUD.AnimateCrystalCollected(crystals);
        }
    }

    public void UpdateLives(int lives)
    {
        if (gameHUD != null)
        {
            gameHUD.UpdateLives(lives);
        }
    }

    public void UpdateHUD(int score, int sessionCrystals, int lives)
    {
        if (gameHUD != null)
        {
            gameHUD.UpdateHUD(score, sessionCrystals, lives);
        }
    }

    public void OnGameStarted()
    {
        gameIsActive = true;
        if (gameHUD != null)
        {
            gameHUD.OnGameStarted();
        }
    }

    public void OnGameEnded()
    {
        gameIsActive = false;
        if (gameHUD != null)
        {
            gameHUD.OnGameEnded();
        }
    }

    public void TogglePause()
    {
        if (!gameIsActive) return;

        isPaused = !isPaused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;

        if (gameHUD != null)
        {
            if (isPaused)
            {
                gameHUD.OnGamePaused();
            }
            else
            {
                gameHUD.OnGameResumed();
            }
        }

        if (isPaused && pauseScoreText != null && GameManager.Instance != null)
        {
            pauseScoreText.text = GameManager.Instance.GetScore().ToString();
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(!isPaused);
        }
    }

    private void OnPauseSettingsClicked()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (inGameSettingsNav != null)
        {
            inGameSettingsNav.ShowSettingsCategories();
        }
    }

    public void ReturnFromSettingsToPause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        ApplySettingsChanges();
    }

    private void ApplySettingsChanges()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.UpdateControlsSettings();
        }

        GraphicsAutoLoader graphicsLoader = FindObjectOfType<GraphicsAutoLoader>();
        if (graphicsLoader != null)
        {
            graphicsLoader.RefreshGraphicsSettings();
        }
    }

    private void OnPauseMainMenuClicked()
    {
        Time.timeScale = 1f;
        isPaused = false;
        OnGameEnded();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnPauseReplayClicked()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnPauseResumeClicked()
    {
        TogglePause();
    }

    private string DetermineGameModeByScene(string sceneName)
    {
        string lowerSceneName = sceneName.ToLower();

        if (lowerSceneName.Contains("level") || lowerSceneName.Contains("lvl"))
        {
            return "Levels";
        }
        else if (lowerSceneName.Contains("game") || lowerSceneName.Contains("infinity") || lowerSceneName.Contains("endless"))
        {
            return "Endless";
        }
        else
        {
            return "Endless";
        }
    }

    public void ShowGameOverScreen(int finalScore, int highScore, int sessionCrystals)
    {
        OnGameEnded();

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string gameMode = DetermineGameModeByScene(sceneName);

        float gameTime = gameHUD != null ? gameHUD.GetCurrentGameTime() : 0f;

        if (gameMode == "Endless")
        {
            ShowInfinityGameOver(finalScore, highScore, gameTime);
        }
        else if (gameMode == "Levels")
        {
            ShowLevelGameOver(finalScore, gameTime);
        }
        else
        {
            ShowInfinityGameOver(finalScore, highScore, gameTime);
        }
    }

    private void ShowInfinityGameOver(int finalScore, int highScore, float gameTime)
    {
        InfinityGameOverController controller = FindObjectOfType<InfinityGameOverController>();

        if (controller != null)
        {
            bool isNewRecord = finalScore > highScore;
            controller.ShowInfinityGameOver(finalScore, highScore, gameTime, isNewRecord);
        }
        else
        {
            ShowFallbackGameOver(finalScore, gameTime);
        }
    }

    private void ShowLevelGameOver(int finalScore, float gameTime)
    {
        LevelGameOverController levelController = FindObjectOfType<LevelGameOverController>();

        if (levelController != null)
        {
            bool victory = false;
            int currentLevel = GetCurrentLevelFromGameManager();

            levelController.ShowLevelGameOver(victory, finalScore, gameTime, currentLevel);
        }
        else
        {
            InfinityGameOverController infinityController = FindObjectOfType<InfinityGameOverController>();

            if (infinityController != null)
            {
                infinityController.ShowInfinityGameOver(finalScore, 0, gameTime, false);
            }
            else
            {
                ShowFallbackGameOver(finalScore, gameTime);
            }
        }
    }

    private void ShowFallbackGameOver(int finalScore, float gameTime)
    {
    }

    private int GetCurrentLevelFromGameManager()
    {
        if (GameManager.Instance != null)
        {
            return GameManager.Instance.GetCurrentLevel();
        }

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (sceneName.ToLower().Contains("level"))
        {
            for (int i = 1; i <= 10; i++)
            {
                if (sceneName.Contains(i.ToString()))
                {
                    return i;
                }
            }
        }

        return 1;
    }

    public bool IsGamePaused() => isPaused;
    public bool IsGameActive() => gameIsActive;
    public void SetGameActive(bool active) => gameIsActive = active;

    public void PauseGame()
    {
        if (!isPaused && gameIsActive)
        {
            TogglePause();
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public string GetCurrentGameTime()
    {
        return gameHUD != null ? gameHUD.GetFormattedGameTime() : "0:00";
    }

    public void ShowLevelVictory(int finalScore, int sessionCrystals)
    {
        OnGameEnded();

        float gameTime = gameHUD != null ? gameHUD.GetCurrentGameTime() : 0f;

        LevelGameOverController levelController = FindObjectOfType<LevelGameOverController>();

        if (levelController != null)
        {
            bool victory = true;
            int currentLevel = GetCurrentLevelFromGameManager();

            levelController.ShowLevelGameOver(victory, finalScore, gameTime, currentLevel);
        }
        else
        {
            InfinityGameOverController infinityController = FindObjectOfType<InfinityGameOverController>();
            if (infinityController != null)
            {
                infinityController.ShowInfinityGameOver(finalScore, 0, gameTime, true);
            }
        }
    }
}