using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    private bool inFormation = false;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float minFireInterval = 1f;
    [SerializeField] private float maxFireInterval = 3f;

    [Header("Stats")]
    [SerializeField] private int health = 2;
    [SerializeField] private int scoreValue = 15;

    [Header("Powerup Drop")]
    [SerializeField] private GameObject[] powerupPrefabs;
    [SerializeField] private float powerupDropChance = 30f;

    [Header("Crystal Drop")]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDropChance = 85f;
    [SerializeField] private int minCrystals = 2;
    [SerializeField] private int maxCrystals = 4;
    [SerializeField] private float crystalEjectionForce = 6f;

    [Header("Effects")]
    [SerializeField] protected GameObject deathEffect;

    private Rigidbody2D rb;
    private float nextFireTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!inFormation)
        {
            rb.velocity = Vector2.down * moveSpeed;
        }

        nextFireTime = Time.time + Random.Range(minFireInterval, maxFireInterval);
    }

    private void Update()
    {
        if (!inFormation && transform.position.y < -6f)
        {
            Destroy(gameObject);
            return;
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + Random.Range(minFireInterval, maxFireInterval);
        }
    }

    public void SetMovementPattern(EnemyMovementPattern pattern)
    {
        if (pattern == EnemyMovementPattern.Formation)
        {
            inFormation = true;
            if (rb != null)
                rb.velocity = Vector2.zero;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
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

    private void Die()
    {
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
                    Random.Range(-0.4f, 0.4f),
                    Random.Range(-0.3f, 0.3f),
                    0
                );

                GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);
                Crystal crystalScript = crystal.GetComponent<Crystal>();

                if (crystalScript != null)
                {
                    Vector2 ejectionDirection = new Vector2(
                        Random.Range(-1f, 1f),
                        Random.Range(0.3f, 1.2f)
                    ).normalized;

                    Vector2 ejectionForce = ejectionDirection * Random.Range(crystalEjectionForce * 0.8f, crystalEjectionForce * 1.4f);
                    crystalScript.AddInitialForce(ejectionForce);

                    float randomTorque = Random.Range(-400f, 400f);
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
}