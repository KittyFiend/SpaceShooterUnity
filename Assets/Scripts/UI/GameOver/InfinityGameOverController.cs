using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InfinityGameOverController : MonoBehaviour
{
    [Header("Infinity Game Over Panel")]
    [SerializeField] private GameObject infinityGameOverPanel;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerText;

    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordText;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button hangarButton;
    [SerializeField] private Button replayButton;

    [Header("Visual Settings")]
    [SerializeField] private Color gameOverColor = Color.red;
    [SerializeField] private float showAnimationDuration = 0.5f;

    private int currentScore = 0;
    private int bestRecord = 0;
    private float gameTime = 0f;
    private bool isNewRecord = false;

    private void Start()
    {
        SetupButtons();

        if (infinityGameOverPanel != null)
            infinityGameOverPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);

        if (hangarButton != null)
            hangarButton.onClick.AddListener(OnHangarClicked);

        if (replayButton != null)
            replayButton.onClick.AddListener(OnReplayClicked);
    }

    public void ShowInfinityGameOver(int score, int record, float gameTime, bool newRecord = false)
    {
        currentScore = score;
        bestRecord = record;
        this.gameTime = gameTime;
        isNewRecord = newRecord;

        if (infinityGameOverPanel != null)
            infinityGameOverPanel.SetActive(true);

        SetupContent();
        StartCoroutine(ShowAnimation());
    }

    private void SetupContent()
    {
        if (headerText != null)
        {
            headerText.text = "GAME OVER";
        }

        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }

        if (recordText != null)
        {
            recordText.text = bestRecord.ToString();
        }
    }

    private IEnumerator ShowAnimation()
    {
        if (infinityGameOverPanel == null) yield break;

        Transform panelTransform = infinityGameOverPanel.transform;
        Vector3 originalScale = panelTransform.localScale;

        panelTransform.localScale = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < showAnimationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / showAnimationDuration;

            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);

            panelTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, easeProgress);
            yield return null;
        }

        panelTransform.localScale = originalScale;

        if (isNewRecord)
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(NewRecordCelebration());
        }
    }

    private IEnumerator NewRecordCelebration()
    {
        if (headerText != null)
        {
            string originalText = headerText.text;
            Color originalColor = headerText.color;

            for (int i = 0; i < 3; i++)
            {
                headerText.text = "NEW RECORD!";
                headerText.color = Color.yellow;
                yield return new WaitForSeconds(0.3f);

                headerText.text = originalText;
                headerText.color = originalColor;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    private IEnumerator PulseAnimation(Transform target, float maxScale, float duration)
    {
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * maxScale;

        float halfDuration = duration * 0.5f;

        while (infinityGameOverPanel.activeInHierarchy)
        {
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

            yield return new WaitForSeconds(0.5f);
        }

        target.localScale = originalScale;
    }

    private void OnMenuClicked()
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

    private void OnReplayClicked()
    {
        HidePanel();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    public void HidePanel()
    {
        if (infinityGameOverPanel != null)
            infinityGameOverPanel.SetActive(false);

        StopAllCoroutines();
    }

    public bool IsPanelActive()
    {
        return infinityGameOverPanel != null && infinityGameOverPanel.activeInHierarchy;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetCurrentRecord()
    {
        return bestRecord;
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }

    public void ShowSimpleGameOver(int score, int record)
    {
        ShowInfinityGameOver(score, record, 0f, score > record);
    }

    public void UpdateScores(int newScore, int newRecord)
    {
        currentScore = newScore;
        bestRecord = newRecord;
        isNewRecord = newScore > newRecord;

        if (scoreText != null)
            scoreText.text = newScore.ToString();

        if (recordText != null)
            recordText.text = newRecord.ToString();
    }
}