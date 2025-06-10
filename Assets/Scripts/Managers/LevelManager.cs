using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private int currentWave = 0;

    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemiesPerWave = 5;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;

    private int enemiesAlive = 0;
    private bool levelActive = false;
    private FormationManager currentFormation;
    private GameObject currentBoss;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        StartLevel();
    }

    public void StartLevel()
    {
        levelActive = true;
        currentWave = 0;
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            CompleteLevel();
            return;
        }

        currentWave++;

        if (currentWave == totalWaves)
        {
            StartCoroutine(SpawnBoss());
        }
        else
        {
            StartCoroutine(SpawnWaveEnemies());
        }
    }

    private IEnumerator SpawnBoss()
    {
        if (currentFormation != null)
        {
            Destroy(currentFormation.gameObject);
            currentFormation = null;
        }

        yield return new WaitForSeconds(1f);

        if (bossPrefab != null)
        {
            Vector2 bossSpawnPos = new Vector2(0, 6f);
            currentBoss = Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity);

            enemiesAlive = 1;
        }
        else
        {
            StartCoroutine(SpawnWaveEnemies());
        }
    }

    private IEnumerator SpawnWaveEnemies()
    {
        if (currentFormation != null)
        {
            Destroy(currentFormation.gameObject);
            currentFormation = null;
        }

        yield return new WaitForSeconds(0.5f);

        GameObject formationGO = new GameObject($"Formation_Wave_{currentWave}");
        FormationManager formation = formationGO.AddComponent<FormationManager>();
        currentFormation = formation;

        int rows, cols;

        if (currentWave <= 4)
        {
            rows = 2;
            cols = 1 + currentWave;
        }
        else
        {
            rows = 3;
            cols = 2 + (currentWave - 4);
        }

        formation.enemiesPerRow = cols;
        formation.numberOfRows = rows;
        formation.spacingX = 1.6f;
        formation.spacingY = 1.0f;
        formation.formationCenter = new Vector2(0, 3.5f);
        formation.horizontalSpeed = 0.3f;
        formation.movementRange = 3f;
        formation.enemyPrefabs = enemyPrefabs;

        enemiesAlive = formation.enemiesPerRow * formation.numberOfRows;

        yield return null;
    }

    public void EnemyDestroyed()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && levelActive)
        {
            if (currentWave == totalWaves)
            {
                CompleteLevel();
            }
            else
            {
                CancelInvoke("StartNextWave");
                Invoke("StartNextWave", 3f);
            }
        }
    }

    public void BossDestroyed()
    {
        EnemyDestroyed();
    }

    private void CompleteLevel()
    {
        levelActive = false;

        ShowVictoryScreen();
    }

    private void ShowVictoryScreen()
    {
        if (UIManager.Instance != null && GameManager.Instance != null)
        {
            int finalScore = GameManager.Instance.GetScore();
            int sessionCrystals = GameManager.Instance.GetSessionCrystals();

            UIManager.Instance.ShowLevelVictory(finalScore, sessionCrystals);
        }
        else
        {
            Invoke("ReturnToMenu", 3f);
        }
    }

    private void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}