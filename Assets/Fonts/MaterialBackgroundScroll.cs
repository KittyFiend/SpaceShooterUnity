using UnityEngine;

public class MaterialBackgroundScroll : MonoBehaviour
{
    [Header("Налаштування")]
    public float scrollSpeed = 2f;

    private Material backgroundMaterial;
    private Vector2 offset;

    void Start()
    {
        // Отримуємо матеріал
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            backgroundMaterial = renderer.material;
        }
    }

    void Update()
    {
        if (backgroundMaterial == null) return;

        // Зсуваємо UV координати текстури
        offset.y += scrollSpeed * Time.deltaTime;
        backgroundMaterial.mainTextureOffset = offset;
    }

    void OnDestroy()
    {
        if (backgroundMaterial != null)
        {
            backgroundMaterial.mainTextureOffset = Vector2.zero;
        }
    }
}