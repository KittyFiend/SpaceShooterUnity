using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button randomNameButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Random Names")]
    [SerializeField]
    private string[] randomFirstNames = {
        "Star", "Cosmic", "Galactic", "Nebula", "Astro", "Rocket", "Plasma", "Laser",
        "Quantum", "Cyber", "Nova", "Orbit", "Solar", "Lunar", "Phoenix", "Thunder"
    };
    [SerializeField]
    private string[] randomLastNames = {
        "Pilot", "Hunter", "Shooter", "Warrior", "Ace", "Hero", "Master", "Legend",
        "Champion", "Guardian", "Striker", "Blaster", "Commander", "Captain", "Knight", "Ghost"
    };

    [Header("Settings")]
    [SerializeField] private int minNameLength = 2;
    [SerializeField] private int maxNameLength = 15;

    private string deviceId;
    private bool nameEntered = false;

    private void Start()
    {
        deviceId = GetOrCreateDeviceId();
        CheckPlayerName();
        SetupUI();
    }

    private void SetupUI()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonPressed);

        if (randomNameButton != null)
            randomNameButton.onClick.AddListener(OnRandomNameButtonPressed);

        if (nameInputField != null)
        {
            nameInputField.onValueChanged.AddListener(OnNameInputChanged);
            nameInputField.characterLimit = maxNameLength;
        }

        if (titleText != null)
            titleText.text = "WELCOME!";

        if (instructionText != null)
            instructionText.text = $"PLEASE ENTER YOUR NAME:";
    }

    private string GetOrCreateDeviceId()
    {
        string id = PlayerPrefs.GetString("DeviceId", "");

        if (string.IsNullOrEmpty(id))
        {
            id = SystemInfo.deviceUniqueIdentifier;

            if (string.IsNullOrEmpty(id) || id == SystemInfo.unsupportedIdentifier)
            {
                id = System.Guid.NewGuid().ToString();
            }

            PlayerPrefs.SetString("DeviceId", id);
            PlayerPrefs.Save();
        }

        return id;
    }

    private void CheckPlayerName()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "");

        if (!string.IsNullOrEmpty(savedName))
        {
            nameEntered = true;
            HideNameInputPanel();

            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.OnPlayerNameSet(savedName, deviceId);
            }
        }
        else
        {
            ShowNameInputPanel();
        }
    }

    private void ShowNameInputPanel()
    {
        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void HideNameInputPanel()
    {
        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnNameInputChanged(string newName)
    {
        bool isValid = IsNameValid(newName);

        if (confirmButton != null)
            confirmButton.interactable = isValid;
    }

    private bool IsNameValid(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        if (name.Length < minNameLength || name.Length > maxNameLength)
            return false;

        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '-' && c != ' ')
                return false;
        }

        return true;
    }

    private void OnConfirmButtonPressed()
    {
        if (nameInputField == null)
            return;

        string enteredName = nameInputField.text.Trim();

        if (IsNameValid(enteredName))
        {
            SavePlayerName(enteredName);
        }
    }

    private void OnRandomNameButtonPressed()
    {
        string randomName = GenerateRandomName();
        SavePlayerName(randomName);
    }

    private string GenerateRandomName()
    {
        string firstName = randomFirstNames[UnityEngine.Random.Range(0, randomFirstNames.Length)];
        string lastName = randomLastNames[UnityEngine.Random.Range(0, randomLastNames.Length)];

        int randomNumber = UnityEngine.Random.Range(100, 999);

        return $"{firstName}{lastName}{randomNumber}";
    }

    private void SavePlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("IsRandomName", randomNameButton != null && randomNameButton == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() ? 1 : 0);
        PlayerPrefs.Save();

        nameEntered = true;
        HideNameInputPanel();

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.OnPlayerNameSet(playerName, deviceId);
        }
    }

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName", "Player");
    }

    public string GetDeviceId()
    {
        return deviceId;
    }

    public bool IsNameEntered()
    {
        return nameEntered;
    }

    public bool IsRandomName()
    {
        return PlayerPrefs.GetInt("IsRandomName", 0) == 1;
    }

    public void ShowNameChangeDialog()
    {
        if (nameInputField != null)
        {
            nameInputField.text = GetPlayerName();
        }
        ShowNameInputPanel();
    }

    public void ResetPlayerName()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("IsRandomName");
        PlayerPrefs.Save();
        nameEntered = false;
        CheckPlayerName();
    }

    public string GetNewRandomName()
    {
        return GenerateRandomName();
    }
}