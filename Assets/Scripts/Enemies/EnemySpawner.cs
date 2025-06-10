using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyTypeInfo
    {
        public GameObject prefab;
        public float spawnWeight = 1f;
    }

    [Header("Enemies")]
    [SerializeField] private EnemyTypeInfo[] enemyTypes;
    [SerializeField] private float enemySpawnRate = 2f;
    [SerializeField] private float minEnemySpawnRate = 0.5f;

    [Header("Obstacles")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float asteroidSpawnRate = 20f;
    [SerializeField] private float minAsteroidSpawnRate = 15f;

    [Header("Spawn Areas")]
    [SerializeField] private float spawnOffsetY = 2f;
    [SerializeField] private float spawnOffsetX = 3f;

    [Header("Difficulty")]
    [SerializeField] private float difficultyIncreaseTime = 10f;
    [SerializeField] private float spawnRateDecrease = 0.1f;

    private Camera mainCamera;
    private float spawnWidth;
    private float spawnHeight;
    private float enemyTimer = 0f;
    private float asteroidTimer = 0f;
    private float difficultyTimer = 0f;

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateSpawnBounds();
    }

    private void Update()
    {
        enemyTimer += Time.deltaTime;
        if (enemyTimer >= enemySpawnRate)
        {
            SpawnEnemy();
            enemyTimer = 0f;
        }

        asteroidTimer += Time.deltaTime;
        if (asteroidTimer >= asteroidSpawnRate)
        {
            SpawnAsteroid();
            asteroidTimer = 0f;
        }

        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseTime)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    private void CalculateSpawnBounds()
    {
        Vector2 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        spawnWidth = screenBounds.x;
        spawnHeight = screenBounds.y;
    }

    private void SpawnEnemy()
    {
        if (enemyTypes.Length == 0) return;

        float totalWeight = 0f;
        foreach (var enemyType in enemyTypes)
        {
            totalWeight += enemyType.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float weightSum = 0f;
        GameObject selectedPrefab = enemyTypes[0].prefab;

        foreach (var enemyType in enemyTypes)
        {
            weightSum += enemyType.spawnWeight;
            if (randomValue <= weightSum)
            {
                selectedPrefab = enemyType.prefab;
                break;
            }
        }

        float randomX = Random.Range(-spawnWidth * 0.95f, spawnWidth * 0.95f);
        Vector2 spawnPosition = new Vector2(randomX, spawnHeight + spawnOffsetY);

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnAsteroid()
    {
        if (asteroidPrefab == null) return;

        Vector2 spawnPosition = new Vector2(
            Random.Range(-spawnWidth * 0.95f, spawnWidth * 0.95f),
            spawnHeight + spawnOffsetY
        );

        Vector2 targetDirection = new Vector2(
            Random.Range(-0.7f, 0.7f),
            Random.Range(-1f, -0.6f)
        ).normalized;

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
        if (asteroidScript != null)
        {
            asteroidScript.SetMovementDirection(targetDirection);
        }
    }

    private void IncreaseDifficulty()
    {
        enemySpawnRate = Mathf.Max(enemySpawnRate - spawnRateDecrease, minEnemySpawnRate);
        asteroidSpawnRate = Mathf.Max(asteroidSpawnRate - spawnRateDecrease, minAsteroidSpawnRate);
    }
}