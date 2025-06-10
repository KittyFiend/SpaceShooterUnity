using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public string date;
    public string deviceId;

    public LeaderboardEntry(string name, int playerScore, string gameDate, string devId = "")
    {
        playerName = name;
        score = playerScore;
        date = gameDate;
        deviceId = devId;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private DatabaseReference databaseReference;
    private bool firebaseInitialized = false;
    private List<LeaderboardEntry> localLeaderboard = new List<LeaderboardEntry>();
    private List<LeaderboardEntry> firebaseLeaderboard = new List<LeaderboardEntry>();
    private List<LeaderboardEntry> combinedLeaderboard = new List<LeaderboardEntry>();

    private string currentPlayerName = "";
    private string currentDeviceId = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadLocalScores();
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Invoke("AutoLoadFirebaseScores", 3f);
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                if (app.Options.DatabaseUrl != null)
                {
                    string databaseUrl = app.Options.DatabaseUrl.ToString();
                    databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                    firebaseInitialized = true;

                    TestFirebaseConnection();
                }
                else
                {
                    firebaseInitialized = false;
                }
            }
            else
            {
                firebaseInitialized = false;
            }
        });
    }

    private void TestFirebaseConnection()
    {
        if (databaseReference != null)
        {
            databaseReference.Child("test").SetValueAsync("connection_test").ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    databaseReference.Child("test").RemoveValueAsync();
                }
            });
        }
    }

    public void OnPlayerNameSet(string playerName, string deviceId)
    {
        currentPlayerName = playerName;
        currentDeviceId = deviceId;

        LoadPlayerRecord();
    }

    private void LoadPlayerRecord()
    {
        if (!firebaseInitialized || databaseReference == null || string.IsNullOrEmpty(currentDeviceId))
            return;

        databaseReference.Child("players").Child(currentDeviceId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Exists)
            {
                var data = task.Result.Value as Dictionary<string, object>;
                if (data != null)
                {
                    string playerName = data.ContainsKey("playerName") ? data["playerName"].ToString() : currentPlayerName;
                    int bestScore = data.ContainsKey("bestScore") ? Convert.ToInt32(data["bestScore"]) : 0;
                    string date = data.ContainsKey("date") ? data["date"].ToString() : DateTime.Now.ToString("dd/MM/yyyy");
                }
            }
        });
    }

    public void SubmitScore(string playerName, int score)
    {
        if (!string.IsNullOrEmpty(currentPlayerName))
            playerName = currentPlayerName;

        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, currentDate, currentDeviceId);

        localLeaderboard.Add(newEntry);
        localLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        if (localLeaderboard.Count > 10)
            localLeaderboard.RemoveRange(10, localLeaderboard.Count - 10);

        SaveLocalScores();

        if (firebaseInitialized && databaseReference != null)
        {
            if (!string.IsNullOrEmpty(currentDeviceId))
            {
                CheckAndSubmitBestScore(newEntry);
            }
        }

        UpdateCombinedLeaderboard();
    }

    private void CheckAndSubmitBestScore(LeaderboardEntry newEntry)
    {
        databaseReference.Child("players").Child(currentDeviceId).GetValueAsync().ContinueWith(task =>
        {
            bool shouldUpdate = false;

            if (task.IsCompleted && !task.IsFaulted)
            {
                if (task.Result.Exists)
                {
                    var data = task.Result.Value as Dictionary<string, object>;
                    if (data != null && data.ContainsKey("bestScore"))
                    {
                        int existingBestScore = Convert.ToInt32(data["bestScore"]);

                        if (newEntry.score > existingBestScore)
                        {
                            shouldUpdate = true;
                        }
                    }
                    else
                    {
                        shouldUpdate = true;
                    }
                }
                else
                {
                    shouldUpdate = true;
                }

                if (shouldUpdate)
                {
                    SubmitPlayerRecord(newEntry);
                }
            }
            else if (task.IsFaulted)
            {
                SubmitPlayerRecord(newEntry);
            }
        });
    }

    private void SubmitPlayerRecord(LeaderboardEntry entry)
    {
        try
        {
            var playerData = new Dictionary<string, object>
            {
                ["playerName"] = entry.playerName,
                ["bestScore"] = entry.score,
                ["date"] = entry.date,
                ["lastUpdated"] = DateTime.Now.ToBinary(),
                ["deviceId"] = entry.deviceId
            };

            databaseReference.Child("players").Child(currentDeviceId).SetValueAsync(playerData).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    LoadFirebaseScores();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating player record: {e.Message}");
        }
    }

    private void AutoLoadFirebaseScores()
    {
        if (firebaseInitialized)
        {
            LoadFirebaseScores();
        }
    }

    public void LoadFirebaseScores()
    {
        if (!firebaseInitialized || databaseReference == null)
        {
            return;
        }

        databaseReference.Child("players").OrderByChild("bestScore").LimitToLast(50).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Exists)
            {
                firebaseLeaderboard.Clear();

                foreach (var child in task.Result.Children)
                {
                    var data = child.Value as Dictionary<string, object>;
                    if (data != null)
                    {
                        string playerName = data.ContainsKey("playerName") ? data["playerName"].ToString() : "Unknown";
                        int bestScore = data.ContainsKey("bestScore") ? Convert.ToInt32(data["bestScore"]) : 0;
                        string date = data.ContainsKey("date") ? data["date"].ToString() : DateTime.Now.ToString("dd/MM/yyyy");
                        string deviceId = data.ContainsKey("deviceId") ? data["deviceId"].ToString() : "";

                        firebaseLeaderboard.Add(new LeaderboardEntry(playerName, bestScore, date, deviceId));
                    }
                }

                firebaseLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));
                UpdateCombinedLeaderboard();
            }
            else if (task.IsFaulted)
            {
                firebaseLeaderboard.Clear();
                UpdateCombinedLeaderboard();
            }
            else
            {
                firebaseLeaderboard.Clear();
                UpdateCombinedLeaderboard();
            }
        });
    }

    private void UpdateCombinedLeaderboard()
    {
        combinedLeaderboard.Clear();
        combinedLeaderboard.AddRange(firebaseLeaderboard);

        combinedLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        if (combinedLeaderboard.Count > 10)
        {
            combinedLeaderboard.RemoveRange(10, combinedLeaderboard.Count - 10);
        }
    }

    private void SaveLocalScores()
    {
        try
        {
            string json = JsonUtility.ToJson(new ScoreList(localLeaderboard));
            PlayerPrefs.SetString("Leaderboard", json);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError("Save error: " + e.Message);
        }
    }

    private void LoadLocalScores()
    {
        try
        {
            if (PlayerPrefs.HasKey("Leaderboard"))
            {
                string json = PlayerPrefs.GetString("Leaderboard");
                ScoreList loadedData = JsonUtility.FromJson<ScoreList>(json);
                localLeaderboard = loadedData.scores ?? new List<LeaderboardEntry>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Load error: " + e.Message);
            localLeaderboard = new List<LeaderboardEntry>();
        }
    }

    public List<LeaderboardEntry> GetLocalLeaderboard()
    {
        return new List<LeaderboardEntry>(combinedLeaderboard);
    }

    public void ShowLeaderboard()
    {
        if (combinedLeaderboard.Count == 0)
        {
            LoadFirebaseScores();
            return;
        }

        for (int i = 0; i < combinedLeaderboard.Count; i++)
        {
            var entry = combinedLeaderboard[i];
        }
    }

    public void RefreshLeaderboard()
    {
        LoadFirebaseScores();
    }

    public void ClearLeaderboard()
    {
        localLeaderboard.Clear();
        combinedLeaderboard.Clear();
        PlayerPrefs.DeleteKey("Leaderboard");
        PlayerPrefs.Save();
    }

    public bool HasScores()
    {
        return combinedLeaderboard.Count > 0;
    }

    public int GetScoreCount()
    {
        return combinedLeaderboard.Count;
    }

    public bool IsFirebaseReady()
    {
        return firebaseInitialized;
    }

    public string GetCurrentPlayerName()
    {
        return currentPlayerName;
    }

    public string GetCurrentDeviceId()
    {
        return currentDeviceId;
    }

    public void UpdatePlayerName(string newName)
    {
        if (string.IsNullOrEmpty(newName) || !firebaseInitialized || databaseReference == null)
        {
            return;
        }

        currentPlayerName = newName;

        if (!string.IsNullOrEmpty(currentDeviceId))
        {
            databaseReference.Child("players").Child(currentDeviceId).GetValueAsync().ContinueWith(task => {
                if (task.IsCompleted && !task.IsCanceled && task.Result.Exists)
                {
                    Dictionary<string, object> updates = new Dictionary<string, object>();
                    updates["playerName"] = newName;
                    updates["lastUpdated"] = DateTime.Now.ToBinary();

                    databaseReference.Child("players").Child(currentDeviceId).UpdateChildrenAsync(updates).ContinueWith(updateTask => {
                        if (updateTask.IsCompleted && !updateTask.IsCanceled)
                        {
                            UpdateLocalLeaderboardName(newName);
                            LoadFirebaseScores();
                        }
                    });
                }
                else
                {
                    var playerData = new Dictionary<string, object>
                    {
                        ["playerName"] = newName,
                        ["bestScore"] = 0,
                        ["date"] = DateTime.Now.ToString("dd/MM/yyyy"),
                        ["lastUpdated"] = DateTime.Now.ToBinary(),
                        ["deviceId"] = currentDeviceId
                    };

                    databaseReference.Child("players").Child(currentDeviceId).SetValueAsync(playerData).ContinueWith(createTask => {
                        if (createTask.IsCompleted && !createTask.IsCanceled)
                        {
                            LoadFirebaseScores();
                        }
                    });
                }
            });
        }
    }

    private void UpdateLocalLeaderboardName(string newName)
    {
        var playerEntry = firebaseLeaderboard.FirstOrDefault(entry => entry.deviceId == currentDeviceId);
        if (playerEntry != null)
        {
            playerEntry.playerName = newName;
        }

        var combinedEntry = combinedLeaderboard.FirstOrDefault(entry => entry.deviceId == currentDeviceId);
        if (combinedEntry != null)
        {
            combinedEntry.playerName = newName;
        }
    }
}

[System.Serializable]
public class ScoreList
{
    public List<LeaderboardEntry> scores;

    public ScoreList(List<LeaderboardEntry> scoreList)
    {
        scores = scoreList;
    }
}