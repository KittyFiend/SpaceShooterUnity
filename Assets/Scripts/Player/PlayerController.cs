using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 150f;
    [SerializeField] private float padding = 0.5f;

    [Header("DEBUG - Test Sensitivity")]
    [SerializeField] private bool disableMoveSpeed = false;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Mobile Controls")]
    [SerializeField] private bool useTouchControls = true;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private FireButtonManager fireButton;

    [Header("Player Stats")]
    [SerializeField] private float invincibilityDuration = 1f;

    [Header("Health/Shield UI System")]
    [SerializeField] private HealthShieldBarController healthShieldBar;

    [Header("Powerup Settings")]
    [SerializeField] private GameObject tripleShotPrefab;
    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private float powerupMultiplier = 2f;
    [SerializeField] private float shieldDuration = 5f;

    private float minX, maxX, minY, maxY;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float shootCooldown = 0f;
    private float originalFireRate;

    private bool controlEnabled = true;
    private bool isInvincible = false;

    private bool hasTripleShot = false;
    private bool hasRapidFire = false;
    private float tripleShotTimer = 0f;
    private float rapidFireTimer = 0f;

    private int currentControlMethod;
    private int currentFiringMode;

    private Vector2 dragStartPosition;
    private bool isDragging = false;
    private Camera mainCamera;

    private float fingerOffset;
    private float draggingSensitivity;
    private float vjSensitivity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        originalFireRate = fireRate;
        ResetPowerups();
        CalculateScreenBoundaries();

#if UNITY_ANDROID || UNITY_IOS
        useTouchControls = true;
#else
        useTouchControls = false;
#endif

        LoadControlsSettingsFromPlayerPrefs();

        if (healthShieldBar == null)
        {
            healthShieldBar = FindObjectOfType<HealthShieldBarController>();
        }

        if (useTouchControls && virtualJoystick == null)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            if (virtualJoystick != null)
            {
                virtualJoystick.RefreshSettingsFromPlayerPrefs();
            }
        }

        if (useTouchControls && fireButton == null)
        {
            fireButton = FindObjectOfType<FireButtonManager>();
        }

        SyncGameManagerWithHealthBar();

        controlEnabled = true;
        RestorePlayerVisibility();

        if (ShipStatsManager.Instance != null)
        {
            ShipStatsManager.Instance.ApplyUpgradesToPlayer(this);

            if (healthShieldBar != null)
            {
                ShipStatsManager.Instance.ApplyHealthUpgradeToHealthBar(healthShieldBar);
                ShipStatsManager.Instance.ApplyShieldUpgradeToHealthBar(healthShieldBar);
            }
        }
    }

    private void SyncGameManagerWithHealthBar()
    {
        if (GameManager.Instance != null && healthShieldBar != null)
        {
            int currentHealth = healthShieldBar.GetCurrentHealth();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateLives(currentHealth);
            }
        }
        else if (GameManager.Instance != null && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateLives(GameManager.Instance.GetLives());
        }
    }

    private void LoadControlsSettingsFromPlayerPrefs()
    {
        currentControlMethod = PlayerPrefs.GetInt("ControlMethod", 2);
        currentFiringMode = PlayerPrefs.GetInt("FiringMode", 0);

        fingerOffset = PlayerPrefs.GetFloat("FingerOffset", 50f);
        draggingSensitivity = PlayerPrefs.GetFloat("DraggingSensitivity", 1.5f);
        vjSensitivity = PlayerPrefs.GetFloat("VJSensitivity", 1.0f);
    }

    public void UpdateControlsSettings()
    {
        LoadControlsSettingsFromPlayerPrefs();

        if (virtualJoystick != null)
        {
            virtualJoystick.RefreshSettingsFromPlayerPrefs();
        }

        if (fireButton != null)
        {
            fireButton.RefreshSettings();
        }

        UpdateJoystickVisibility();
    }

    private void UpdateJoystickVisibility()
    {
        if (virtualJoystick != null)
        {
            bool shouldShowJoystick = (currentControlMethod == 2) && useTouchControls;
            bool currentlyActive = virtualJoystick.gameObject.activeInHierarchy;

            if (currentlyActive != shouldShowJoystick)
            {
                virtualJoystick.SetJoystickActive(shouldShowJoystick);
            }
        }
    }

    private void ResetPowerups()
    {
        hasTripleShot = false;
        hasRapidFire = false;
        fireRate = 0.2f;
        originalFireRate = fireRate;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    private void RestorePlayerVisibility()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
            playerCollider.enabled = true;
    }

    private void Update()
    {
        ProcessMovement();
        ProcessShooting();
        UpdatePowerupTimers();

        if (Time.frameCount % 120 == 0)
        {
            UpdateJoystickVisibility();
        }
    }

    private void CalculateScreenBoundaries()
    {
        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + padding;
        maxX = topRight.x - padding;
        minY = bottomLeft.y + padding;
        maxY = topRight.y - padding;
    }

    private void ProcessMovement()
    {
        if (!controlEnabled || Time.timeScale == 0f)
        {
            return;
        }

        Vector2 movement = Vector2.zero;

        if (useTouchControls)
        {
            switch (currentControlMethod)
            {
                case 0:
                    HandlePointMovement(ref movement);
                    break;
                case 1:
                    HandleDragMovement(ref movement);
                    break;
                case 2:
                    HandleJoystickMovement(ref movement);
                    break;
            }
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            movement = new Vector2(horizontalInput, verticalInput);
        }

        ApplyMovement(movement);
    }

    private void HandlePointMovement(ref Vector2 movement)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }
            }

            Vector3 touchWorldPos = mainCamera.ScreenToWorldPoint(touch.position);
            touchWorldPos.z = 0;

            float offsetInWorldUnits = fingerOffset * (mainCamera.orthographicSize * 2f) / Screen.height;
            touchWorldPos.y += offsetInWorldUnits;

            Vector2 targetPosition = touchWorldPos;
            Vector2 newPosition = targetPosition;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;

            movement = Vector2.zero;
        }
    }

    private void HandleDragMovement(ref Vector2 movement)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragStartPosition = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 currentPosition = touch.position;
                Vector2 dragDelta = currentPosition - dragStartPosition;

                Vector2 normalizedDelta = dragDelta.normalized;
                movement = normalizedDelta * draggingSensitivity;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else
        {
            isDragging = false;
        }
    }

    private void HandleJoystickMovement(ref Vector2 movement)
    {
        if (virtualJoystick != null)
        {
            Vector2 rawInput = virtualJoystick.GetRawInputVector();
            float realSensitivity = vjSensitivity * 2.5f;
            movement = rawInput * realSensitivity;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                Vector2 touchDirection = (touch.position - screenCenter).normalized;
                movement = touchDirection * 0.5f;
            }
        }
    }

    private void ApplyMovement(Vector2 movement)
    {
        if (movement.magnitude > 3.0f)
        {
            movement = movement.normalized * 3.0f;
        }

        Vector2 newPosition;

        if (disableMoveSpeed)
        {
            newPosition = rb.position + movement * Time.deltaTime;
        }
        else
        {
            newPosition = rb.position + movement * moveSpeed * Time.deltaTime;
        }

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rb.MovePosition(newPosition);
    }

    private void ProcessShooting()
    {
        if (!controlEnabled) return;

        if (shootCooldown > 0)
            shootCooldown -= Time.deltaTime;

        bool shouldShoot = DetermineShouldShoot();

        if (shouldShoot && shootCooldown <= 0)
        {
            Shoot();
            shootCooldown = fireRate;
        }
    }

    private bool DetermineShouldShoot()
    {
        if (currentFiringMode == 0)
        {
            if (useTouchControls)
            {
                switch (currentControlMethod)
                {
                    case 0:
                        return Input.touchCount > 0;
                    case 1:
                        return isDragging;
                    case 2:
                        return virtualJoystick != null && virtualJoystick.IsPressed;
                }
            }
            else
            {
                return Input.GetKey(KeyCode.Space);
            }
        }
        else
        {
            if (useTouchControls)
            {
                return fireButton != null && fireButton.IsPressed;
            }
            else
            {
                return Input.GetKey(KeyCode.Space);
            }
        }

        return false;
    }

    private void Shoot()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayShootSound();

        if (hasTripleShot && tripleShotPrefab != null)
        {
            Instantiate(tripleShotPrefab, firePoint.position, Quaternion.identity);
        }
        else if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
    }

    private void UpdatePowerupTimers()
    {
        if (hasTripleShot)
        {
            tripleShotTimer -= Time.deltaTime;
            if (tripleShotTimer <= 0)
                hasTripleShot = false;
        }

        if (hasRapidFire)
        {
            rapidFireTimer -= Time.deltaTime;
            if (rapidFireTimer <= 0)
            {
                hasRapidFire = false;
                fireRate = originalFireRate;
            }
        }
    }

    public void ActivatePowerup(PowerupType type, float duration)
    {
        switch (type)
        {
            case PowerupType.TripleShot:
                hasTripleShot = true;
                tripleShotTimer = duration;
                break;

            case PowerupType.RapidFire:
                hasRapidFire = true;
                fireRate = originalFireRate / powerupMultiplier;
                rapidFireTimer = duration;
                break;

            case PowerupType.Shield:
                if (healthShieldBar != null)
                {
                    healthShieldBar.ActivateShield();

                    if (shieldVisual != null)
                    {
                        shieldVisual.SetActive(true);
                        StartCoroutine(DisableShieldVisualAfterDelay(healthShieldBar.GetMaxShield()));
                    }
                }
                else
                {
                    if (shieldVisual != null)
                        shieldVisual.SetActive(true);
                }
                break;

            case PowerupType.ExtraLife:
                if (healthShieldBar != null)
                {
                    int beforeHP = healthShieldBar.GetCurrentHealth();
                    int beforeMaxHP = healthShieldBar.GetMaxHealth();

                    healthShieldBar.AddHealth(1);

                    int afterHP = healthShieldBar.GetCurrentHealth();
                    int afterMaxHP = healthShieldBar.GetMaxHealth();

                    if (afterHP > beforeHP || afterMaxHP > beforeMaxHP)
                    {
                        SyncGameManagerWithHealthBar();
                    }
                    else
                    {
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.AddLife();
                        }
                    }
                }
                else
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddLife();
                    }
                }
                break;

            case PowerupType.Bomb:
                DestroyAllEnemies();
                break;
        }
    }

    private IEnumerator DisableShieldVisualAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    private void DestroyAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
            enemy.TakeDamage(1000);

        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
        foreach (Asteroid asteroid in asteroids)
            asteroid.TakeDamage(1000);

        ShootingEnemy[] shootingEnemies = FindObjectsOfType<ShootingEnemy>();
        foreach (ShootingEnemy enemy in shootingEnemies)
            enemy.TakeDamage(1000);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        if (healthShieldBar != null && healthShieldBar.HasShield())
        {
            return;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlayerHitSound();

        if (healthShieldBar != null)
        {
            healthShieldBar.TakeDamage(damage);

            if (healthShieldBar.GetCurrentHealth() <= 0)
            {
                HandlePlayerDeath();
            }
            else
            {
                SyncGameManagerWithHealthBar();
            }
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
        }

        StartCoroutine(BecomeInvincible());
    }

    private void HandlePlayerDeath()
    {
        DisableControl();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SyncLivesWithHealthBar(0);
            GameManager.Instance.GameOver();
        }
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        float blinkTime = 0.1f;

        for (float i = 0; i < invincibilityDuration; i += blinkTime * 2)
        {
            float t = 0;
            while (t < blinkTime)
            {
                t += Time.deltaTime;
                spriteRenderer.color = Color.Lerp(originalColor, transparentColor, t / blinkTime);
                yield return null;
            }

            t = 0;
            while (t < blinkTime)
            {
                t += Time.deltaTime;
                spriteRenderer.color = Color.Lerp(transparentColor, originalColor, t / blinkTime);
                yield return null;
            }
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    public void DisableControl()
    {
        controlEnabled = false;
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isInvincible = false;
    }

    public void EnableControl()
    {
        controlEnabled = true;
        isInvincible = false;

        if (rb != null)
            rb.velocity = Vector2.zero;

        RestorePlayerVisibility();
    }

    public void ResetPlayer()
    {
        controlEnabled = true;
        isInvincible = false;

        ResetPowerups();
        RestorePlayerVisibility();
        StopAllCoroutines();

        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    public int GetCurrentHealth()
    {
        return healthShieldBar != null ? healthShieldBar.GetCurrentHealth() : 0;
    }

    public int GetMaxHealth()
    {
        return healthShieldBar != null ? healthShieldBar.GetMaxHealth() : 0;
    }

    public bool HasShield()
    {
        return healthShieldBar != null ? healthShieldBar.HasShield() : false;
    }

    public int GetShieldTime()
    {
        return healthShieldBar != null ? healthShieldBar.GetCurrentShield() : 0;
    }
}