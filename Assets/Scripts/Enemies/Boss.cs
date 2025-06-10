using System.Collections;
using UnityEngine;

public enum BossPhase
{
    Phase1,
    Phase2,
    Phase3
}

public class Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int scoreValue = 500;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float movementRange = 3f;
    [SerializeField] private Vector2 targetPosition = new Vector2(0, 1.5f);
    [SerializeField] private float verticalMovementRange = 2.5f;
    [SerializeField] private float maxChaoticSpeed = 8f;
    [SerializeField] private float directionChangeInterval = 5f;
    [SerializeField] private float accelerationTime = 1.2f;

    [Header("Shooting - Phase 1")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float phase1FireRate = 2f;

    [Header("Shooting - Phase 2")]
    [SerializeField] private float phase2FireRate = 1f;

    [Header("Shooting - Phase 3")]
    [SerializeField] private float phase3FireRate = 0.5f;
    [SerializeField] private int phase3BurstCount = 3;

    [Header("Effects")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject phaseChangeEffect;

    private int currentHealth;
    private BossPhase currentPhase;
    private bool hasReachedPosition = false;
    private float nextFireTime;
    private Vector2 moveDirection = Vector2.right;
    private Vector2 basePosition;
    private float lastDirectionChangeTime = 0f;
    private Vector2 currentTargetPoint;
    private Vector2 movementStartPosition;
    private float movementStartTime = 0f;
    private bool isMovingToNewTarget = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentPhase = BossPhase.Phase1;

        if (BossHealthBar.Instance != null)
        {
            BossHealthBar.Instance.ShowBossUI();
        }

        StartCoroutine(MoveToPosition());
    }

    private void Update()
    {
        if (hasReachedPosition)
        {
            if (currentPhase == BossPhase.Phase3)
            {
                HandleChaoticMovement();
            }
            else
            {
                HandleSimpleMovement();
            }

            HandleShooting();
        }
    }

    private IEnumerator MoveToPosition()
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        hasReachedPosition = true;
        basePosition = transform.position;
        currentTargetPoint = basePosition;
        nextFireTime = Time.time + 1f;
    }

    private void HandleMovement()
    {
        if (currentPhase == BossPhase.Phase1)
        {
            HandleSimpleMovement();
        }
        else
        {
            HandleChaoticMovement();
        }
    }

    private void HandleSimpleMovement()
    {
        float currentMoveSpeed = moveSpeed;

        if (currentPhase == BossPhase.Phase2)
            currentMoveSpeed *= 1.3f;

        Vector2 newPosition = (Vector2)transform.position + moveDirection * currentMoveSpeed * Time.deltaTime;

        if (newPosition.x >= movementRange || newPosition.x <= -movementRange)
        {
            moveDirection *= -1;
        }

        transform.position = new Vector2(newPosition.x, basePosition.y);
    }

    private void HandleChaoticMovement()
    {
        if (!isMovingToNewTarget && Time.time - lastDirectionChangeTime >= directionChangeInterval)
        {
            StartMovementToNewTarget();
        }

        if (isMovingToNewTarget)
        {
            MoveToTargetWithAcceleration();
        }
    }

    private void StartMovementToNewTarget()
    {
        ChooseNewRandomTarget();

        movementStartPosition = transform.position;
        movementStartTime = Time.time;
        isMovingToNewTarget = true;
        lastDirectionChangeTime = Time.time;
    }

    private void MoveToTargetWithAcceleration()
    {
        float timeElapsed = Time.time - movementStartTime;
        float totalDistance = Vector2.Distance(movementStartPosition, currentTargetPoint);
        float distanceToTarget = Vector2.Distance(transform.position, currentTargetPoint);

        float accelerationCurve;
        float brakingDistance = totalDistance * 0.3f;

        if (timeElapsed < accelerationTime)
        {
            float normalizedTime = timeElapsed / accelerationTime;
            accelerationCurve = Mathf.SmoothStep(0f, 1f, normalizedTime);
        }
        else if (distanceToTarget < brakingDistance)
        {
            float brakingProgress = distanceToTarget / brakingDistance;
            accelerationCurve = Mathf.SmoothStep(0.2f, 1f, brakingProgress);
        }
        else
        {
            accelerationCurve = 1f;
        }

        float currentMaxSpeed = maxChaoticSpeed;
        if (currentPhase == BossPhase.Phase3)
            currentMaxSpeed *= 1.4f;

        float currentSpeed = currentMaxSpeed * accelerationCurve;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTargetPoint,
            currentSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, currentTargetPoint) < 0.2f)
        {
            isMovingToNewTarget = false;
        }
    }

    private void ChooseNewRandomTarget()
    {
        Vector2 currentPos = transform.position;
        Vector2 newTarget;

        do
        {
            float randomX = Random.Range(-movementRange, movementRange);
            float minY = basePosition.y - verticalMovementRange;
            float maxY = basePosition.y + (verticalMovementRange * 0.5f);
            float randomY = Random.Range(minY, maxY);

            randomY = Mathf.Clamp(randomY, -1f, 3.5f);
            newTarget = new Vector2(randomX, randomY);

        } while (Vector2.Distance(currentPos, newTarget) < 3f);

        currentTargetPoint = newTarget;
    }

    private void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            switch (currentPhase)
            {
                case BossPhase.Phase1:
                    FirePhase1();
                    nextFireTime = Time.time + phase1FireRate;
                    break;

                case BossPhase.Phase2:
                    FirePhase2();
                    nextFireTime = Time.time + phase2FireRate;
                    break;

                case BossPhase.Phase3:
                    StartCoroutine(FirePhase3Burst());
                    nextFireTime = Time.time + phase3FireRate;
                    break;
            }
        }
    }

    private void FirePhase1()
    {
        if (firePoints.Length > 0 && projectilePrefab != null)
        {
            Instantiate(projectilePrefab, firePoints[0].position, Quaternion.identity);
        }
    }

    private void FirePhase2()
    {
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint != null && projectilePrefab != null)
            {
                Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            }
        }
    }

    private IEnumerator FirePhase3Burst()
    {
        for (int i = 0; i < phase3BurstCount; i++)
        {
            foreach (Transform firePoint in firePoints)
            {
                if (firePoint != null && projectilePrefab != null)
                {
                    Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                }
            }

            if (i < phase3BurstCount - 1)
                yield return new WaitForSeconds(0.2f);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (BossHealthBar.Instance != null)
        {
            BossHealthBar.Instance.SetBossHealth(currentHealth, maxHealth);
        }

        CheckPhaseChange();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckPhaseChange()
    {
        BossPhase newPhase = currentPhase;

        if (currentHealth <= maxHealth * 0.33f && currentPhase != BossPhase.Phase3)
        {
            newPhase = BossPhase.Phase3;
        }
        else if (currentHealth <= maxHealth * 0.66f && currentPhase == BossPhase.Phase1)
        {
            newPhase = BossPhase.Phase2;
        }

        if (newPhase != currentPhase)
        {
            ChangePhase(newPhase);
        }
    }

    private void ChangePhase(BossPhase newPhase)
    {
        BossPhase oldPhase = currentPhase;
        currentPhase = newPhase;

        if (currentPhase != BossPhase.Phase3)
        {
            isMovingToNewTarget = false;
        }

        if (phaseChangeEffect != null)
        {
            Instantiate(phaseChangeEffect, transform.position, Quaternion.identity);
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
            LevelManager.Instance.BossDestroyed();
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
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
        }

        if (other.CompareTag("Projectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }
}