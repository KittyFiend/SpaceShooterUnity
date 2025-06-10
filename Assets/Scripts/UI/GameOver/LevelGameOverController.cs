using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelGameOverController : MonoBehaviour
{
    [Header("Level Game Over Panel")]
    [SerializeField] private GameObject levelGameOverPanel;

    [Header("Header with Animation")]
    [SerializeField] private TextMeshProUGUI headerText;

    [Header("Stars System")]
    [SerializeField] private GameObject starsContainer;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite bronzeStar;
    [SerializeField] private Sprite silverStar;
    [SerializeField] private Sprite goldStar;

    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Defeat Buttons")]
    [SerializeField] private GameObject defeatButtonsContainer;
    [SerializeField] private Button defeatReplayButton;
    [SerializeField] private Button defeatMainMenuButton;
    [SerializeField] private Button defeatHangarButton;

    [Header("Victory Buttons")]
    [SerializeField] private GameObject victoryButtonsContainer;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button victoryReplayButton;
    [SerializeField] private Button victoryMainMenuButton;
    [SerializeField] private Button victoryHangarButton;

    [Header("Animation Settings")]
    [SerializeField] private float headerAnimationInterval = 2f;
    [SerializeField] private Color loseColor = Color.red;
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color levelColor = Color.yellow;

    private bool isVictory = false;
    private int currentLevel = 1;
    private float completionTime = 0f;
    private int finalScore = 0;
    private Coroutine headerAnimationCoroutine;

    [System.Serializable]
    public class LevelTimeRating
    {
        public float goldTime = 60f;
        public float silverTime = 90f;
        public float bronzeTime = 120f;
    }

    [Header("Level Rating System")]
    [SerializeField] private LevelTimeRating[] levelRatings;

    private void Start()
    {
        SetupButtons();

        if (levelGameOverPanel != null)
            levelGameOverPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (defeatReplayButton != null)
            defeatReplayButton.onClick.AddListener(OnDefeatReplayClicked);
        if (defeatMainMenuButton != null)
            defeatMainMenuButton.onClick.AddListener(OnMainMenuClicked);
        if (defeatHangarButton != null)
            defeatHangarButton.onClick.AddListener(OnHangarClicked);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        if (victoryReplayButton != null)
            victoryReplayButton.onClick.AddListener(OnVictoryReplayClicked);
        if (victoryMainMenuButton != null)
            victoryMainMenuButton.onClick.AddListener(OnMainMenuClicked);
        if (victoryHangarButton != null)
            victoryHangarButton.onClick.AddListener(OnHangarClicked);
    }

    public void ShowLevelGameOver(bool victory, int score, float gameTime, int level)
    {
        isVictory = victory;
        currentLevel = level;
        completionTime = gameTime;
        finalScore = score;

        if (levelGameOverPanel != null)
            levelGameOverPanel.SetActive(true);

        SetupContent();
        SetupButtonsVisibility();
        StartHeaderAnimation();

        if (victory)
        {
            SetupStars(gameTime, level);
        }
        else
        {
            SetupEmptyStars();
        }
    }

    private void SetupContent()
    {
        if (scoreText != null)
            scoreText.text = finalScore.ToString();

        string formattedTime = FormatTime(completionTime);

        if (timeText != null)
        {
            timeText.text = formattedTime;
        }
    }

    private void SetupButtonsVisibility()
    {
        if (defeatButtonsContainer != null)
            defeatButtonsContainer.SetActive(!isVictory);

        if (victoryButtonsContainer != null)
            victoryButtonsContainer.SetActive(isVictory);
    }

    private void SetupStars(float gameTime, int level)
    {
        if (starImages == null || starImages.Length == 0) return;

        foreach (var star in starImages)
        {
            if (star != null) star.sprite = emptyStar;
        }

        LevelTimeRating rating = GetRatingForLevel(level);
        int starsEarned = CalculateStarsEarned(gameTime, rating);

        for (int i = 0; i < starsEarned && i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].sprite = GetStarSpriteForPosition(i, gameTime, rating);
            }
        }

        StartCoroutine(AnimateStars(starsEarned));
    }

    private void SetupEmptyStars()
    {
        if (starImages == null) return;

        foreach (var star in starImages)
        {
            if (star != null) star.sprite = emptyStar;
        }
    }

    private LevelTimeRating GetRatingForLevel(int level)
    {
        if (levelRatings != null && level > 0 && level <= levelRatings.Length)
        {
            return levelRatings[level - 1];
        }

        return new LevelTimeRating { goldTime = 60f, silverTime = 90f, bronzeTime = 120f };
    }

    private int CalculateStarsEarned(float gameTime, LevelTimeRating rating)
    {
        if (gameTime <= rating.goldTime)
            return 3;
        else if (gameTime <= rating.silverTime)
            return 3;
        else if (gameTime <= rating.bronzeTime)
            return 3;
        else
            return 3;
    }

    private Sprite GetStarSpriteForPosition(int position, float gameTime, LevelTimeRating rating)
    {
        if (gameTime <= rating.goldTime)
        {
            return goldStar;
        }
        else if (gameTime <= rating.silverTime)
        {
            return silverStar;
        }
        else if (gameTime <= rating.bronzeTime)
        {
            return bronzeStar;
        }
        else
        {
            return bronzeStar;
        }
    }

    private void StartHeaderAnimation()
    {
        if (headerAnimationCoroutine != null)
            StopCoroutine(headerAnimationCoroutine);

        if (!isVictory)
        {
            headerAnimationCoroutine = StartCoroutine(AnimateHeaderLevelLose());
        }
        else
        {
            headerAnimationCoroutine = StartCoroutine(AnimateHeaderLevelWin());
        }
    }

    private IEnumerator AnimateHeaderLevelWin()
    {
        bool showWin = true;

        while (levelGameOverPanel.activeInHierarchy)
        {
            if (showWin)
            {
                headerText.text = "YOU WIN!";
            }
            else
            {
                headerText.text = $"LEVEL {currentLevel}";
            }

            showWin = !showWin;
            yield return new WaitForSeconds(headerAnimationInterval);
        }
    }

    private IEnumerator AnimateHeaderLevelLose()
    {
        bool showLose = true;

        while (levelGameOverPanel.activeInHierarchy)
        {
            if (showLose)
            {
                headerText.text = "YOU LOSE";
            }
            else
            {
                headerText.text = $"LEVEL {currentLevel}";
            }

            showLose = !showLose;
            yield return new WaitForSeconds(headerAnimationInterval);
        }
    }

    private void SetStaticHeaderText()
    {
        if (headerText == null) return;

        headerText.text = "YOU WIN!";
    }

    private IEnumerator AnimateStars(int starsCount)
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < starsCount && i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                StartCoroutine(PulseAnimation(starImages[i].transform, 1.3f, 0.3f));
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator PulseAnimation(Transform target, float maxScale, float duration)
    {
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * maxScale;

        float halfDuration = duration * 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / halfDuration;
            target.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / halfDuration;
            target.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        target.localScale = originalScale;
    }

    private void OnDefeatReplayClicked()
    {
        HidePanel();

        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    private void OnVictoryReplayClicked()
    {
        HidePanel();

        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    private void OnNextLevelClicked()
    {
        HidePanel();
    }

    private void OnMainMenuClicked()
    {
        HidePanel();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnHangarClicked()
    {

    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }

    public void HidePanel()
    {
        if (headerAnimationCoroutine != null)
        {
            StopCoroutine(headerAnimationCoroutine);
            headerAnimationCoroutine = null;
        }

        if (levelGameOverPanel != null)
            levelGameOverPanel.SetActive(false);
    }

    public bool IsPanelActive()
    {
        return levelGameOverPanel != null && levelGameOverPanel.activeInHierarchy;
    }

    public int GetCurrentLevel() => currentLevel;
    public float GetCompletionTime() => completionTime;
    public int GetFinalScore() => finalScore;
    public bool IsVictory() => isVictory;
}