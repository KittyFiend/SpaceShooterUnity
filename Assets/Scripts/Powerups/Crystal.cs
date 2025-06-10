using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Crystal Settings")]
    [SerializeField] private int crystalValue = 1;
    [SerializeField] private float magnetDistance = 0f;
    [SerializeField] private float magnetSpeed = 8f;
    [SerializeField] private float lifetime = 15f;

    [Header("Physics Settings")]
    [SerializeField] private float bounceForce = 0.7f;
    [SerializeField] private float minBounceVelocity = 2f;
    [SerializeField] private float groundLevel = -5f;
    [SerializeField] private float leftWall = -9f;
    [SerializeField] private float rightWall = 9f;
    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float groundStopTime = 2f;

    [Header("Visual Effects")]
    [SerializeField] private float rotationSpeed = 180f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isBeingMagneted = false;
    private int bounceCount = 0;
    private bool hasLandedOnGround = false;
    private float groundTimer = 0f;
    private Vector3 originalScale;
    private bool isDestroying = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        rb.gravityScale = 1.5f;
        rb.drag = 0.5f;

        Vector2 randomForce = new Vector2(
            Random.Range(-3f, 3f),
            Random.Range(0f, 2f)
        );
        rb.AddForce(randomForce, ForceMode2D.Impulse);

        rb.angularVelocity = Random.Range(-360f, 360f);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        CheckBounce();
        CheckGroundTimer();
        HandleMagnetism();

        if (transform.position.y < -8f || Mathf.Abs(transform.position.x) > 12f)
        {
            Destroy(gameObject);
        }
    }

    private void CheckBounce()
    {
        bool bounced = false;

        if (transform.position.y <= groundLevel && rb.velocity.y < -1f && bounceCount < maxBounces)
        {
            bounceCount++;
            float currentBounceForce = bounceForce * (1f - (bounceCount - 1) * 0.3f);

            transform.position = new Vector3(transform.position.x, groundLevel + 0.1f, transform.position.z);
            rb.velocity = new Vector2(
                rb.velocity.x * 0.7f,
                Mathf.Max(3f, Mathf.Abs(rb.velocity.y) * currentBounceForce)
            );

            bounced = true;
        }

        if (transform.position.x <= leftWall && rb.velocity.x < 0 && bounceCount < maxBounces)
        {
            if (!bounced) bounceCount++;

            transform.position = new Vector3(leftWall + 0.1f, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(
                Mathf.Abs(rb.velocity.x) * bounceForce,
                rb.velocity.y
            );
        }

        if (transform.position.x >= rightWall && rb.velocity.x > 0 && bounceCount < maxBounces)
        {
            if (!bounced) bounceCount++;

            transform.position = new Vector3(rightWall - 0.1f, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(
                -Mathf.Abs(rb.velocity.x) * bounceForce,
                rb.velocity.y
            );
        }
    }

    private void CheckGroundTimer()
    {
        bool isOnGround = transform.position.y <= groundLevel;
        bool isSlowEnough = rb.velocity.magnitude < 1f;

        if (isOnGround && isSlowEnough)
        {
            if (!hasLandedOnGround)
            {
                hasLandedOnGround = true;
                groundTimer = 0f;
            }

            groundTimer += Time.deltaTime;

            rb.angularVelocity *= 0.95f;

            if (groundTimer >= groundStopTime * 0.7f && !isDestroying)
            {
                StartDestroying();
            }
        }
        else
        {
            hasLandedOnGround = false;
            groundTimer = 0f;
        }
    }

    private void StartDestroying()
    {
        if (isDestroying) return;

        isDestroying = true;
        StartCoroutine(DestroyWithFade());
    }

    private System.Collections.IEnumerator DestroyWithFade()
    {
        float fadeTime = groundStopTime * 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeTime;
            float scale = Mathf.Lerp(1f, 0f, progress);
            transform.localScale = originalScale * scale;

            yield return null;
        }

        Destroy(gameObject);
    }

    private void HandleMagnetism()
    {
        // Магнетизм відключений
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddSessionCrystals(crystalValue);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCrystalPickupSound();
            }

            transform.localScale = originalScale * 1.5f;

            Destroy(gameObject);
        }
    }

    public void SetCrystalValue(int value)
    {
        crystalValue = value;
    }

    public void SetMagnetDistance(float distance)
    {
        magnetDistance = distance;
    }

    public void AddInitialForce(Vector2 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void AddInitialTorque(float torque)
    {
        if (rb != null)
        {
            rb.AddTorque(torque);
        }
    }
}