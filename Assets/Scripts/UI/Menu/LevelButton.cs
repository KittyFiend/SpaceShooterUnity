using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject lockIcon;

    [Header("Visual States")]
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;

    private int levelNumber;
    private bool isUnlocked;

    public void SetupLevel(int level, bool unlocked, int starCount, int bestScore)
    {
        levelNumber = level;
        isUnlocked = unlocked;

        if (levelNumberText != null)
        {
            levelNumberText.text = level.ToString();
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = bestScore > 0 ? bestScore.ToString() : "";
        }

        SetupStars(starCount);
        SetupButtonState(unlocked);

        if (button != null)
        {
            button.onClick.AddListener(OnLevelButtonClicked);
        }
    }

    private void SetupStars(int starCount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                stars[i].SetActive(i < starCount);
            }
        }
    }

    private void SetupButtonState(bool unlocked)
    {
        if (button != null)
        {
            button.interactable = unlocked;
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = unlocked ? unlockedColor : lockedColor;
            }
        }

        if (lockIcon != null)
        {
            lockIcon.SetActive(!unlocked);
        }

        if (levelNumberText != null)
        {
            levelNumberText.color = unlocked ? Color.white : Color.gray;
        }
    }

    private void OnLevelButtonClicked()
    {
        if (!isUnlocked) return;

        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        PlayerPrefs.SetString("GameMode", "Levels");

        SceneManager.LoadScene("GameScene");
    }
}