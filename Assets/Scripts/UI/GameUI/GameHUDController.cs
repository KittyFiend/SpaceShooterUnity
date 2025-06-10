using UnityEngine;
using TMPro;

public class GameHUDController : MonoBehaviour
{
    [Header("HUD Panel Elements")]
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI sessionCrystalsText;

    [Header("Additional Game Info")]
    [SerializeField] private TextMeshProUGUI livesText;

    private float gameStartTime;
    private float pausedTime = 0f;
    private bool gameIsRunning = false;

    private void Start()
    {
        ResetGameTime();
        UpdateHUD(0, 0, 3);
        Invoke(nameof(AutoStartTimer), 0.5f);
    }

    private void AutoStartTimer()
    {
        if (!gameIsRunning)
        {
            StartGameTimer();
        }
    }

    private void Update()
    {
        if (gameIsRunning)
        {
            UpdateGameTime();
        }
    }

    public void StartGameTimer()
    {
        gameStartTime = Time.time;
        gameIsRunning = true;
    }

    public void StopGameTimer()
    {
        if (gameIsRunning)
        {
            pausedTime = Time.time - gameStartTime;
            gameIsRunning = false;
        }
    }

    public void ResumeGameTimer()
    {
        if (!gameIsRunning)
        {
            gameStartTime = Time.time - pausedTime;
            gameIsRunning = true;
        }
    }

    public void ResetGameTime()
    {
        gameStartTime = Time.time;
        gameIsRunning = false;
        UpdateGameTimeDisplay(0f);
    }

    public float GetCurrentGameTime()
    {
        if (gameIsRunning)
        {
            return Time.time - gameStartTime;
        }
        else
        {
            return pausedTime;
        }
    }

    private void UpdateGameTime()
    {
        float currentTime = GetCurrentGameTime();
        UpdateGameTimeDisplay(currentTime);
    }

    private void UpdateGameTimeDisplay(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);

        if (gameTimeText != null)
        {
            gameTimeText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateHUD(int score, int sessionCrystals, int lives)
    {
        UpdateScore(score);
        UpdateSessionCrystals(sessionCrystals);
        UpdateLives(lives);
    }

    public void UpdateScore(int score)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = score.ToString();
        }
    }

    public void UpdateSessionCrystals(int crystals)
    {
        if (sessionCrystalsText != null)
        {
            sessionCrystalsText.text = crystals.ToString();
        }
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives.ToString();
        }
    }

    public void AnimateScoreIncrease(int newScore)
    {
        UpdateScore(newScore);

        if (currentScoreText != null)
        {
            StartCoroutine(PulseAnimation(currentScoreText.transform, 1.1f, 0.2f));
        }
    }

    public void AnimateCrystalCollected(int newCrystalCount)
    {
        UpdateSessionCrystals(newCrystalCount);

        if (sessionCrystalsText != null)
        {
            StartCoroutine(PulseAnimation(sessionCrystalsText.transform, 1.15f, 0.25f));
        }
    }

    private System.Collections.IEnumerator PulseAnimation(Transform target, float maxScale, float duration)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * maxScale;

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

    public void OnGameStarted()
    {
        ResetGameTime();
        StartGameTimer();
        UpdateHUD(0, 0, 3);
    }

    public void OnGamePaused()
    {
        StopGameTimer();
    }

    public void OnGameResumed()
    {
        ResumeGameTimer();
    }

    public void OnGameEnded()
    {
        StopGameTimer();
        float finalTime = GetCurrentGameTime();
    }

    public string GetFormattedGameTime()
    {
        float timeInSeconds = GetCurrentGameTime();
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}