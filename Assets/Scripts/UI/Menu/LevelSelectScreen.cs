using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectScreen : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform levelButtonsParent;
    [SerializeField] private int totalLevels = 15;

    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetProgressButton;

    private void Start()
    {
        SetupButtons();
        GenerateLevelButtons();
    }

    private void SetupButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(OnResetProgressButtonClicked);
        }
    }

    private void GenerateLevelButtons()
    {
        foreach (Transform child in levelButtonsParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= totalLevels; i++)
        {
            CreateLevelButton(i);
        }
    }

    private void CreateLevelButton(int levelNumber)
    {
        GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonsParent);
        LevelButton levelButton = buttonObj.GetComponent<LevelButton>();

        if (levelButton != null)
        {
            bool isUnlocked = IsLevelUnlocked(levelNumber);
            int stars = GetLevelStars(levelNumber);
            int bestScore = GetLevelBestScore(levelNumber);

            levelButton.SetupLevel(levelNumber, isUnlocked, stars, bestScore);
        }
    }

    private bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1) return true;

        return PlayerPrefs.GetInt($"Level_{levelNumber}_Unlocked", 0) == 1;
    }

    private int GetLevelStars(int levelNumber)
    {
        return PlayerPrefs.GetInt($"Level_{levelNumber}_Stars", 0);
    }

    private int GetLevelBestScore(int levelNumber)
    {
        return PlayerPrefs.GetInt($"Level_{levelNumber}_BestScore", 0);
    }

    private void OnBackButtonClicked()
    {
        if (ScreenManager.Instance != null)
        {
            ScreenManager.Instance.GoBack();
        }
    }

    private void OnResetProgressButtonClicked()
    {
        if (ScreenManager.Instance != null)
        {
            ScreenManager.Instance.ShowScreenWithHistory(GameObject.Find("SettingsScreen"));
        }
    }

    public void RefreshLevelButtons()
    {
        GenerateLevelButtons();
    }
}