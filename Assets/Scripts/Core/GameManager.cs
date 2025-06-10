using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private string gameMode = "Endless";
    [SerializeField] private int currentLevel = 1;

    [Header("Score")]
    [SerializeField] private int score = 0;
    [SerializeField] private int highScore = 0;

    [Header("Player")]
    [SerializeField] private int lives = 3;
    [SerializeField] private int maxLives = 5;

    [Header("Crystal System")]
    [SerializeField] private int totalCrystals = 0;
    [SerializeField] private int sessionCrystals = 0;
    [SerializeField] private int totalCrystalsCollected = 0;

    [Header("Player Statistics")]
    [SerializeField] private int levelsCompleted = 0;
    [SerializeField] private float totalPlayTime = 0f;
    [SerializeField] private int totalGamesPlayed = 0;
    [SerializeField] private float sessionStartTime = 0f;

    private bool isGameOver = false;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadGameData();
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        InfinityGameOverController infinityController = FindObjectOfType<InfinityGameOverController>();
        if (infinityController != null)
        {
            infinityController.HidePanel();
        }

        LevelGameOverController levelController = FindObjectOfType<LevelGameOverController>();
        if (levelController != null)
        {
            levelController.HidePanel();
        }

        if (scene.name.Contains("GameScene") || scene.name.Contains("Level"))
        {
            InitializeGame();
        }
    }

    private void Start()
    {
    }

    private void LoadGameData()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        totalCrystals = PlayerPrefs.GetInt("TotalCrystals", 0);
        totalCrystalsCollected = PlayerPrefs.GetInt("TotalCrystalsEverCollected", 0);

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentSceneName.Contains("GameScene") || currentSceneName.Contains("Infinity"))
        {
            gameMode = "Endless";
            int endlessHighScore = PlayerPrefs.GetInt("HighScore_Endless", 0);
            highScore = endlessHighScore;
        }
        else if (currentSceneName.Contains("Level"))
        {
            gameMode = "Levels";
            int levelsHighScore = PlayerPrefs.GetInt("HighScore_Levels", 0);
            highScore = levelsHighScore;
        }
        else if (PlayerPrefs.HasKey("GameMode"))
        {
            gameMode = PlayerPrefs.GetString("GameMode");
            if (gameMode == "Endless")
            {
                highScore = PlayerPrefs.GetInt("HighScore_Endless", 0);
            }
            else if (gameMode == "Levels")
            {
                highScore = PlayerPrefs.GetInt("HighScore_Levels", 0);
            }
        }

        levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted", 0);
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        totalGamesPlayed = PlayerPrefs.GetInt("TotalGamesPlayed", 0);
    }

    public void InitializeGame()
    {
        isGameOver = false;
        gameStarted = true;
        score = 0;
        sessionCrystals = 0;

        lives = 3;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.ResetPlayer();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStarted();
            UIManager.Instance.UpdateHUD(score, sessionCrystals, lives);
        }

        IncrementGamesPlayed();
        StartSession();
    }

    public void AddSessionCrystals(int amount)
    {
        if (isGameOver) return;

        sessionCrystals += amount;
        totalCrystalsCollected += amount;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSessionCrystalsAnimated(sessionCrystals);
        }
    }

    private void TransferSessionCrystalsToTotal()
    {
        if (sessionCrystals > 0)
        {
            totalCrystals += sessionCrystals;

            PlayerPrefs.SetInt("TotalCrystals", totalCrystals);
            PlayerPrefs.SetInt("TotalCrystalsEverCollected", totalCrystalsCollected);
            PlayerPrefs.Save();
        }
    }

    public bool SpendTotalCrystals(int amount)
    {
        if (totalCrystals >= amount)
        {
            totalCrystals -= amount;

            PlayerPrefs.SetInt("TotalCrystals", totalCrystals);
            PlayerPrefs.Save();

            return true;
        }

        return false;
    }

    public int GetTotalCrystals() => totalCrystals;
    public int GetSessionCrystals() => sessionCrystals;
    public int GetTotalCrystalsCollected() => totalCrystalsCollected;

    [System.Obsolete("Use AddSessionCrystals instead")]
    public void AddCrystals(int amount)
    {
        AddSessionCrystals(amount);
    }

    [System.Obsolete("Use SpendTotalCrystals instead")]
    public bool SpendCrystals(int amount)
    {
        return SpendTotalCrystals(amount);
    }

    [System.Obsolete("Use GetSessionCrystals or GetTotalCrystals instead")]
    public int GetCrystals()
    {
        return sessionCrystals;
    }

    public void AddScore(int points)
    {
        if (isGameOver)
        {
            return;
        }

        if (lives <= 0)
        {
            return;
        }

        score += points;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreAnimated(score);
        }
    }

    public void AddLife()
    {
        if (lives < maxLives)
        {
            lives++;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateLives(lives);
            }
        }
    }

    public void LoseLife()
    {
        if (isGameOver)
        {
            return;
        }

        lives--;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLives(lives);
        }

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void SetLives(int newLives)
    {
        int oldLives = lives;
        lives = Mathf.Clamp(newLives, 0, maxLives);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLives(lives);
        }

        if (lives <= 0 && !isGameOver)
        {
            GameOver();
        }
    }

    public void SyncLivesWithHealthBar(int healthBarValue)
    {
        SetLives(healthBarValue);
    }

    public int GetHighScoreForMode(string mode)
    {
        if (mode == "Endless")
        {
            return PlayerPrefs.GetInt("HighScore_Endless", 0);
        }
        else if (mode == "Levels")
        {
            return PlayerPrefs.GetInt("HighScore_Levels", 0);
        }

        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public void SaveHighScoreForMode(string mode, int score)
    {
        if (mode == "Endless")
        {
            int currentHigh = PlayerPrefs.GetInt("HighScore_Endless", 0);
            if (score > currentHigh)
            {
                PlayerPrefs.SetInt("HighScore_Endless", score);
                PlayerPrefs.Save();
            }
        }
        else if (mode == "Levels")
        {
            int currentHigh = PlayerPrefs.GetInt("HighScore_Levels", 0);
            if (score > currentHigh)
            {
                PlayerPrefs.SetInt("HighScore_Levels", score);
                PlayerPrefs.Save();
            }
        }

        int generalHigh = PlayerPrefs.GetInt("HighScore", 0);
        if (score > generalHigh)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameStarted = false;

        TransferSessionCrystalsToTotal();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.DisableControl();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverSound();
        }

        SaveHighScoreForMode(gameMode, score);

        int currentModeHighScore = GetHighScoreForMode(gameMode);

        SubmitScoreToLeaderboard();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverScreen(score, currentModeHighScore, sessionCrystals);
        }

        EndSession();
    }

    private void SubmitScoreToLeaderboard()
    {
        if (gameMode == "Endless" && LeaderboardManager.Instance != null && score > 0)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            LeaderboardManager.Instance.SubmitScore(playerName, score);
        }
    }

    public void RestartGame()
    {
        InitializeGame();

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public int GetLives() => lives;
    public int GetScore() => score;
    public int GetHighScore() => highScore;
    public string GetGameMode() => gameMode;
    public bool IsGameOver() => isGameOver;
    public bool IsGameStarted() => gameStarted;
    public int GetCurrentLevel() => currentLevel;
    public int GetLevelsCompleted() => levelsCompleted;
    public float GetTotalPlayTime() => totalPlayTime;
    public int GetTotalGamesPlayed() => totalGamesPlayed;

    public void SetGameMode(string mode)
    {
        gameMode = mode;
        PlayerPrefs.SetString("GameMode", gameMode);
        PlayerPrefs.Save();
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    public void StartSession()
    {
        sessionStartTime = Time.time;
    }

    public void EndSession()
    {
        if (sessionStartTime > 0f)
        {
            float sessionTime = Time.time - sessionStartTime;
            totalPlayTime += sessionTime;

            PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
            PlayerPrefs.Save();

            sessionStartTime = 0f;
        }
    }

    public void CompleteLevel(int levelNumber)
    {
        int highestLevel = PlayerPrefs.GetInt("HighestLevelCompleted", 0);

        if (levelNumber > highestLevel)
        {
            levelsCompleted++;
            PlayerPrefs.SetInt("LevelsCompleted", levelsCompleted);
            PlayerPrefs.SetInt("HighestLevelCompleted", levelNumber);
            PlayerPrefs.Save();
        }
    }

    public void IncrementGamesPlayed()
    {
        totalGamesPlayed++;
        PlayerPrefs.SetInt("TotalGamesPlayed", totalGamesPlayed);
        PlayerPrefs.Save();
    }

    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(totalPlayTime / 3600f);
        int minutes = Mathf.FloorToInt((totalPlayTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalPlayTime % 60f);

        if (hours > 0)
            return $"{hours}h {minutes}m {seconds}s";
        else if (minutes > 0)
            return $"{minutes}m {seconds}s";
        else
            return $"{seconds}s";
    }
}