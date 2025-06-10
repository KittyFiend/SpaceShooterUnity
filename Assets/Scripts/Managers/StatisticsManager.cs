using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StatisticsManager : MonoBehaviour
{
    [Header("Statistics Panel")]
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button backgroundButton;

    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button changeNameButton;

    [Header("Name Change Panel")]
    [SerializeField] private GameObject nameChangePanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmNameButton;
    [SerializeField] private Button cancelNameButton;

    [Header("Statistics Display")]
    [SerializeField] private TextMeshProUGUI infinityRecordText;
    [SerializeField] private TextMeshProUGUI levelsCompletedText;
    [SerializeField] private TextMeshProUGUI totalCrystalsText;
    [SerializeField] private TextMeshProUGUI totalPlayTimeText;
    [SerializeField] private TextMeshProUGUI gamesPlayedText;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;

    private bool isPanelOpen = false;
    private bool isNameChangePanelOpen = false;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HideStatistics);

        if (backgroundButton != null)
            backgroundButton.onClick.AddListener(HideStatistics);

        if (changeNameButton != null)
            changeNameButton.onClick.AddListener(OpenNameChangeDialog);

        if (confirmNameButton != null)
            confirmNameButton.onClick.AddListener(ConfirmNameChange);

        if (cancelNameButton != null)
            cancelNameButton.onClick.AddListener(CancelNameChange);

        if (statisticsPanel != null)
            statisticsPanel.SetActive(false);

        if (nameChangePanel != null)
            nameChangePanel.SetActive(false);
    }

    public void ShowStatistics()
    {
        if (isPanelOpen) return;

        isPanelOpen = true;

        LoadStatisticsData();

        if (statisticsPanel != null)
        {
            statisticsPanel.SetActive(true);
            StartCoroutine(AnimatePanel(statisticsPanel, Vector3.zero, Vector3.one));
        }
    }

    public void HideStatistics()
    {
        if (!isPanelOpen) return;

        isPanelOpen = false;

        if (statisticsPanel != null)
        {
            StartCoroutine(AnimatePanel(statisticsPanel, Vector3.one, Vector3.zero, () => {
                statisticsPanel.SetActive(false);
            }));
        }
    }

    private void LoadStatisticsData()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        if (playerNameText != null)
            playerNameText.text = playerName;

        int infinityRecord = PlayerPrefs.GetInt("HighScore_Endless", 0);
        if (infinityRecordText != null)
            infinityRecordText.text = $"{infinityRecord:N0}";

        int levelsCompleted = GameManager.Instance != null ? GameManager.Instance.GetLevelsCompleted() : PlayerPrefs.GetInt("LevelsCompleted", 0);
        if (levelsCompletedText != null)
            levelsCompletedText.text = $"{levelsCompleted}";

        int totalCrystals = GameManager.Instance != null ? GameManager.Instance.GetTotalCrystalsCollected() : PlayerPrefs.GetInt("TotalCrystalsEverCollected", 0);
        if (totalCrystalsText != null)
            totalCrystalsText.text = $"{totalCrystals:N0}";

        string playTime = GameManager.Instance != null ? GameManager.Instance.GetFormattedPlayTime() : FormatTime(PlayerPrefs.GetFloat("TotalPlayTime", 0f));
        if (totalPlayTimeText != null)
            totalPlayTimeText.text = playTime;

        int gamesPlayed = GameManager.Instance != null ? GameManager.Instance.GetTotalGamesPlayed() : PlayerPrefs.GetInt("TotalGamesPlayed", 0);
        if (gamesPlayedText != null)
            gamesPlayedText.text = $"{gamesPlayed}";
    }

    private string FormatTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);

        if (hours > 0)
            return $"{hours}h {minutes}m {secs}s";
        else if (minutes > 0)
            return $"{minutes}m {secs}s";
        else
            return $"{secs}s";
    }

    private void OpenNameChangeDialog()
    {
        if (isNameChangePanelOpen) return;

        isPanelOpen = false;

        if (statisticsPanel != null)
            statisticsPanel.SetActive(false);

        if (nameChangePanel != null)
        {
            isNameChangePanelOpen = true;
            nameChangePanel.SetActive(true);

            if (nameInputField != null)
            {
                string currentName = PlayerPrefs.GetString("PlayerName", "Player");
                nameInputField.text = currentName;
                nameInputField.Select();
            }

            StartCoroutine(AnimatePanel(nameChangePanel, Vector3.zero, Vector3.one));
        }
    }

    private void ConfirmNameChange()
    {
        if (!isNameChangePanelOpen) return;

        string newName = nameInputField != null ? nameInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(newName))
        {
            return;
        }

        if (newName.Length > 20)
        {
            newName = newName.Substring(0, 20);
        }

        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.Save();

        SyncNameWithFirebase(newName);

        CloseNameChangeDialog();
    }

    private void CancelNameChange()
    {
        CloseNameChangeDialog();
    }

    private void CloseNameChangeDialog()
    {
        if (!isNameChangePanelOpen) return;

        isNameChangePanelOpen = false;

        if (nameChangePanel != null)
        {
            StartCoroutine(AnimatePanel(nameChangePanel, Vector3.one, Vector3.zero, () => {
                nameChangePanel.SetActive(false);

                isPanelOpen = true;
                if (statisticsPanel != null)
                {
                    statisticsPanel.SetActive(true);
                    LoadStatisticsData();
                    StartCoroutine(AnimatePanel(statisticsPanel, Vector3.zero, Vector3.one));
                }
            }));
        }
    }

    private IEnumerator AnimatePanel(GameObject panel, Vector3 fromScale, Vector3 toScale, System.Action onComplete = null)
    {
        if (panel == null) yield break;

        RectTransform panelTransform = panel.GetComponent<RectTransform>();
        if (panelTransform == null) yield break;

        float elapsed = 0f;
        panelTransform.localScale = fromScale;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / animationDuration;

            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            panelTransform.localScale = Vector3.Lerp(fromScale, toScale, easeProgress);

            yield return null;
        }

        panelTransform.localScale = toScale;
        onComplete?.Invoke();
    }

    public void RefreshStatistics()
    {
        if (isPanelOpen)
        {
            LoadStatisticsData();
        }
    }

    private void SyncNameWithFirebase(string newName)
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.UpdatePlayerName(newName);
        }
    }

    public bool IsPanelOpen()
    {
        return isPanelOpen || isNameChangePanelOpen;
    }
}