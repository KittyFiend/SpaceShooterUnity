using UnityEngine;

public enum EnemyMovementPattern
{
    Straight,
    Zigzag,
    Circular,
    Wave,
    Formation
}

public class Enemy : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int health = 1;
    [SerializeField] private int scoreValue = 10;

    [Header("Movement Pattern")]
    [SerializeField] private EnemyMovementPattern movementPattern = EnemyMovementPattern.Straight;
    [SerializeField] private float amplitudeX = 2f;
    [SerializeField] private float frequencyX = 2f;
    [SerializeField] private float circleRadius = 1f;

    [Header("Powerup Drop")]
    [SerializeField] private GameObject[] powerupPrefabs;
    [SerializeField] private float powerupDropChance = 20f;

    [Header("Crystal Drop")]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDropChance = 80f;
    [SerializeField] private int minCrystals = 1;
    [SerializeField] private int maxCrystals = 3;
    [SerializeField] private float crystalEjectionForce = 5f;

    [Header("Effects")]
    [SerializeField] protected GameObject deathEffect;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float timeCounter = 0f;
    private bool hasDied = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        switch (movementPattern)
        {
            case EnemyMovementPattern.Straight:
                rb.velocity = Vector2.down * moveSpeed;
                break;
            case EnemyMovementPattern.Zigzag:
            case EnemyMovementPattern.Circular:
            case EnemyMovementPattern.Wave:
                break;
        }
    }

    private void Update()
    {
        switch (movementPattern)
        {
            case EnemyMovementPattern.Straight:
                break;

            case EnemyMovementPattern.Zigzag:
                timeCounter += Time.deltaTime;
                float x = startPosition.x + Mathf.Sin(timeCounter * frequencyX) * amplitudeX;
                float y = startPosition.y - moveSpeed * timeCounter;
                transform.position = new Vector2(x, y);
                break;

            case EnemyMovementPattern.Circular:
                timeCounter += Time.deltaTime * moveSpeed;
                float circleX = startPosition.x + Mathf.Cos(timeCounter) * circleRadius;
                float circleY = startPosition.y + Mathf.Sin(timeCounter) * circleRadius - moveSpeed * timeCounter * 0.5f;
                transform.position = new Vector2(circleX, circleY);
                break;

            case EnemyMovementPattern.Wave:
                timeCounter += Time.deltaTime;
                float waveX = startPosition.x + Mathf.Sin(timeCounter * frequencyX) * amplitudeX;
                float waveY = startPosition.y - moveSpeed * timeCounter;
                transform.position = new Vector2(waveX, waveY);
                break;

            case EnemyMovementPattern.Formation:
                break;
        }

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (hasDied)
        {
            return;
        }

        hasDied = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosionSound();
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.EnemyDestroyed();
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        TryDropPowerup();
        TryDropCrystal();

        Destroy(gameObject);
    }

    private void TryDropCrystal()
    {
        if (crystalPrefab == null) return;

        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= crystalDropChance)
        {
            int crystalCount = Random.Range(minCrystals, maxCrystals + 1);

            for (int i = 0; i < crystalCount; i++)
            {
                Vector3 spawnPosition = transform.position + new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.2f, 0.2f),
                    0
                );

                GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);
                Crystal crystalScript = crystal.GetComponent<Crystal>();

                if (crystalScript != null)
                {
                    Vector2 ejectionDirection = new Vector2(
                        Random.Range(-1f, 1f),
                        Random.Range(0.2f, 1f)
                    ).normalized;

                    Vector2 ejectionForce = ejectionDirection * Random.Range(crystalEjectionForce * 0.7f, crystalEjectionForce * 1.3f);
                    crystalScript.AddInitialForce(ejectionForce);

                    float randomTorque = Random.Range(-300f, 300f);
                    crystalScript.AddInitialTorque(randomTorque);
                }
            }
        }
    }

    private void TryDropPowerup()
    {
        if (powerupPrefabs.Length == 0) return;

        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= powerupDropChance)
        {
            int randomIndex = Random.Range(0, powerupPrefabs.Length);
            GameObject powerupPrefab = powerupPrefabs[randomIndex];

            Instantiate(powerupPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
            }

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.EnemyDestroyed();
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Projectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    public void SetMovementPattern(EnemyMovementPattern pattern)
    {
        movementPattern = pattern;

        if (pattern == EnemyMovementPattern.Formation)
        {
            if (rb != null)
                rb.velocity = Vector2.zero;
        }
    }
}