using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Stats")]
    [SerializeField] private int health = 3;
    [SerializeField] private int scoreValue = 25;

    [Header("Powerup Drop")]
    [SerializeField] private GameObject[] powerupPrefabs;
    [SerializeField] private float powerupDropChance = 15f;

    [Header("Crystal Drop")]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDropChance = 90f;
    [SerializeField] private int minCrystals = 3;
    [SerializeField] private int maxCrystals = 6;
    [SerializeField] private float crystalEjectionForce = 8f;

    [Header("Size & Splitting")]
    [SerializeField] private AsteroidSize asteroidSize = AsteroidSize.Large;
    [SerializeField] private GameObject mediumAsteroidPrefab;
    [SerializeField] private GameObject smallAsteroidPrefab;
    [SerializeField] private float splitForce = 3f;

    private Rigidbody2D rb;
    private float speed;
    private Vector2 movementDirection = Vector2.down;
    private bool directionSet = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = Random.Range(minSpeed, maxSpeed);

        if (!directionSet)
        {
            movementDirection = Vector2.down;
        }

        rb.velocity = movementDirection * speed;
        rb.angularVelocity = Random.Range(-rotationSpeed, rotationSpeed);

        SetSizeByType();
    }

    private void SetSizeByType()
    {
        float scale = 1f;

        switch (asteroidSize)
        {
            case AsteroidSize.Large:
                scale = Random.Range(1.2f, 1.8f);
                health = 3;
                scoreValue = 25;
                break;
            case AsteroidSize.Medium:
                scale = Random.Range(0.8f, 1.2f);
                health = 2;
                scoreValue = 15;
                break;
            case AsteroidSize.Small:
                scale = Random.Range(0.4f, 0.8f);
                health = 1;
                scoreValue = 10;
                break;
        }

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void Update()
    {
        CheckBounds();
    }

    private void CheckBounds()
    {
        Camera mainCamera = Camera.main;
        Vector2 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float margin = 3f;

        if (transform.position.y < -screenBounds.y - margin ||
            transform.position.y > screenBounds.y + margin ||
            transform.position.x < -screenBounds.x - margin ||
            transform.position.x > screenBounds.x + margin)
        {
            Destroy(gameObject);
        }
    }

    public void SetMovementDirection(Vector2 direction)
    {
        movementDirection = direction.normalized;
        directionSet = true;

        if (rb != null)
        {
            rb.velocity = movementDirection * speed;
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

        SplitAsteroid();
        TryDropPowerup();
        TryDropCrystal();

        Destroy(gameObject);
    }

    private void SplitAsteroid()
    {
        GameObject prefabToSpawn = null;
        AsteroidSize newSize = AsteroidSize.Small;

        switch (asteroidSize)
        {
            case AsteroidSize.Large:
                prefabToSpawn = mediumAsteroidPrefab;
                newSize = AsteroidSize.Medium;
                break;
            case AsteroidSize.Medium:
                prefabToSpawn = smallAsteroidPrefab;
                newSize = AsteroidSize.Small;
                break;
            case AsteroidSize.Small:
                return;
        }

        if (prefabToSpawn == null) return;

        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.3f, 0.3f),
                0
            );

            GameObject newAsteroid = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            Asteroid newAsteroidScript = newAsteroid.GetComponent<Asteroid>();

            if (newAsteroidScript != null)
            {
                newAsteroidScript.SetAsteroidSize(newSize);

                Vector2 splitDirection = new Vector2(
                    Random.Range(-0.8f, 0.8f),
                    Random.Range(-1f, -0.2f)
                ).normalized;

                newAsteroidScript.SetMovementDirection(splitDirection);

                Rigidbody2D newRb = newAsteroid.GetComponent<Rigidbody2D>();
                if (newRb != null)
                {
                    newRb.AddForce(splitDirection * splitForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void SetAsteroidSize(AsteroidSize size)
    {
        asteroidSize = size;
        SetSizeByType();
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

    private void TryDropCrystal()
    {
        if (asteroidSize != AsteroidSize.Small) return;

        if (crystalPrefab == null) return;

        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= crystalDropChance)
        {
            int crystalCount = Random.Range(minCrystals, maxCrystals + 1);

            for (int i = 0; i < crystalCount; i++)
            {
                Vector3 spawnPosition = transform.position + new Vector3(
                    Random.Range(-0.6f, 0.6f),
                    Random.Range(-0.4f, 0.4f),
                    0
                );

                GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);
                Crystal crystalScript = crystal.GetComponent<Crystal>();

                if (crystalScript != null)
                {
                    Vector2 ejectionDirection = new Vector2(
                        Random.Range(-1f, 1f),
                        Random.Range(-0.2f, 1.5f)
                    ).normalized;

                    Vector2 ejectionForce = ejectionDirection * Random.Range(crystalEjectionForce * 0.6f, crystalEjectionForce * 1.5f);
                    crystalScript.AddInitialForce(ejectionForce);

                    float randomTorque = Random.Range(-500f, 500f);
                    crystalScript.AddInitialTorque(randomTorque);
                }
            }
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
            Destroy(gameObject);
        }

        if (other.CompareTag("Projectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }
}