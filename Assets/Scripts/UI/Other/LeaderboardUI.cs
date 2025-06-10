using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Button ratingButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backwardButton;

    [Header("Player Entry Elements")]
    [SerializeField] private Image[] playerButtons;
    [SerializeField] private TextMeshProUGUI[] playerNames;
    [SerializeField] private TextMeshProUGUI[] playerScores;

    [Header("Default Colors")]
    [SerializeField] private Color activeEntryColor = Color.white;
    [SerializeField] private Color inactiveEntryColor = Color.gray;

    private List<LeaderboardEntry> currentLeaderboard = new List<LeaderboardEntry>();
    private int currentPage = 0;
    private int entriesPerPage = 4;

    private void Start()
    {
        if (ratingButton != null)
            ratingButton.onClick.AddListener(ShowLeaderboard);

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(HideLeaderboard);

        if (forwardButton != null)
            forwardButton.onClick.AddListener(NextPage);

        if (backwardButton != null)
            backwardButton.onClick.AddListener(PreviousPage);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        if (LeaderboardManager.Instance != null)
        {
            currentLeaderboard = LeaderboardManager.Instance.GetLocalLeaderboard();
        }
        else
        {
            currentLeaderboard = new List<LeaderboardEntry>();
        }

        currentPage = 0;
        UpdateDisplay();

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < entriesPerPage; i++)
        {
            int entryIndex = currentPage * entriesPerPage + i;

            if (entryIndex < currentLeaderboard.Count)
            {
                var entry = currentLeaderboard[entryIndex];

                if (playerButtons[i] != null)
                    playerButtons[i].color = activeEntryColor;

                if (playerNames[i] != null)
                    playerNames[i].text = entry.playerName;

                if (playerScores[i] != null)
                    playerScores[i].text = entry.score.ToString();
            }
            else
            {
                if (playerButtons[i] != null)
                    playerButtons[i].color = inactiveEntryColor;

                if (playerNames[i] != null)
                    playerNames[i].text = "";

                if (playerScores[i] != null)
                    playerScores[i].text = "";
            }
        }

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        int totalPages = Mathf.CeilToInt((float)currentLeaderboard.Count / entriesPerPage);

        if (backwardButton != null)
            backwardButton.interactable = currentPage > 0;

        if (forwardButton != null)
            forwardButton.interactable = currentPage < totalPages - 1;
    }

    private void NextPage()
    {
        int totalPages = Mathf.CeilToInt((float)currentLeaderboard.Count / entriesPerPage);

        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdateDisplay();
        }
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateDisplay();
        }
    }

    public void RefreshLeaderboard()
    {
        if (leaderboardPanel != null && leaderboardPanel.activeInHierarchy)
        {
            ShowLeaderboard();
        }
    }
}