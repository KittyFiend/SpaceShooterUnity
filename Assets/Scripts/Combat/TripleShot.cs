using UnityEngine;

public class TripleShot : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;

    void Start()
    {
        foreach (Transform firePoint in firePoints)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }

        Destroy(gameObject, 0.1f);
    }
}