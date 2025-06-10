using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject hitEffect;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        CheckBounds();
    }

    private void CheckBounds()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector2 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.y > screenBounds.y + 1f ||
            transform.position.y < -screenBounds.y - 1f ||
            transform.position.x < -screenBounds.x - 1f ||
            transform.position.x > screenBounds.x + 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Asteroid"))
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}