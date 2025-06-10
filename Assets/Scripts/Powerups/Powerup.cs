using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private PowerupType powerupType;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float duration = 5f;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.down * moveSpeed;
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                ApplyPowerup(player);
            }
            Destroy(gameObject);
        }
    }

    private void ApplyPowerup(PlayerController player)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerupSound();
        }

        player.ActivatePowerup(powerupType, duration);
    }
}