using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExpandingSettingsMenu : MonoBehaviour
{
    [Header("Кнопки")]
    public Button mainSettingsButton;           // Основна кнопка налаштувань
    public Button musicVolumeButton;            // Кнопка гучності музики
    public Button soundEffectsButton;           // Кнопка звукових ефектів

    [Header("Налаштування анімації")]
    public float animationDuration = 0.3f;      // Тривалість анімації
    public float buttonSpacing = 80f;           // Відстань між кнопками

    private bool isExpanded = false;            // Стан меню (розгорнуте/згорнуте)
    private Vector3 originalPosition;           // Початкова позиція головної кнопки

    void Start()
    {
        // Зберігаємо початкову позицію
        originalPosition = mainSettingsButton.transform.position;

        // Ховаємо додаткові кнопки на початку
        musicVolumeButton.gameObject.SetActive(false);
        soundEffectsButton.gameObject.SetActive(false);

        // Підключаємо обробники подій
        mainSettingsButton.onClick.AddListener(ToggleMenu);
        musicVolumeButton.onClick.AddListener(OnMusicButtonClick);
        soundEffectsButton.onClick.AddListener(OnSoundEffectsButtonClick);
    }

    public void ToggleMenu()
    {
        if (isExpanded)
        {
            CollapseMenu();
        }
        else
        {
            ExpandMenu();
        }
    }

    void ExpandMenu()
    {
        isExpanded = true;

        // Показуємо додаткові кнопки
        musicVolumeButton.gameObject.SetActive(true);
        soundEffectsButton.gameObject.SetActive(true);

        // Встановлюємо початкові позиції (в тому ж місці що й основна кнопка)
        musicVolumeButton.transform.position = originalPosition;
        soundEffectsButton.transform.position = originalPosition;

        // Анімуємо появу кнопок
        StartCoroutine(AnimateExpand());
    }

    void CollapseMenu()
    {
        isExpanded = false;
        StartCoroutine(AnimateCollapse());
    }

    IEnumerator AnimateExpand()
    {
        float elapsed = 0f;

        // Цільові позиції для кнопок
        Vector3 musicTargetPos = originalPosition + Vector3.up * buttonSpacing;
        Vector3 soundTargetPos = originalPosition + Vector3.up * (buttonSpacing * 2);

        // Початкові масштаби (маленькі)
        musicVolumeButton.transform.localScale = Vector3.zero;
        soundEffectsButton.transform.localScale = Vector3.zero;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // Використовуємо ease-out функцію для плавності
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            // Анімуємо позиції
            musicVolumeButton.transform.position = Vector3.Lerp(originalPosition, musicTargetPos, easedProgress);
            soundEffectsButton.transform.position = Vector3.Lerp(originalPosition, soundTargetPos, easedProgress);

            // Анімуємо масштаб
            float scale = Mathf.Lerp(0f, 1f, easedProgress);
            musicVolumeButton.transform.localScale = Vector3.one * scale;
            soundEffectsButton.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // Встановлюємо фінальні значення
        musicVolumeButton.transform.position = musicTargetPos;
        soundEffectsButton.transform.position = soundTargetPos;
        musicVolumeButton.transform.localScale = Vector3.one;
        soundEffectsButton.transform.localScale = Vector3.one;
    }

    IEnumerator AnimateCollapse()
    {
        float elapsed = 0f;

        Vector3 musicStartPos = musicVolumeButton.transform.position;
        Vector3 soundStartPos = soundEffectsButton.transform.position;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // Використовуємо ease-in функцію
            float easedProgress = Mathf.Pow(progress, 3f);

            // Анімуємо позиції назад до центру
            musicVolumeButton.transform.position = Vector3.Lerp(musicStartPos, originalPosition, easedProgress);
            soundEffectsButton.transform.position = Vector3.Lerp(soundStartPos, originalPosition, easedProgress);

            // Анімуємо масштаб до нуля
            float scale = Mathf.Lerp(1f, 0f, easedProgress);
            musicVolumeButton.transform.localScale = Vector3.one * scale;
            soundEffectsButton.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // Ховаємо кнопки
        musicVolumeButton.gameObject.SetActive(false);
        soundEffectsButton.gameObject.SetActive(false);
    }

    // Обробники натискання на кнопки
    void OnMusicButtonClick()
    {
        // Тут можна відкрити слайдер для музики або переключити стан
        Debug.Log("Music settings clicked");

        // Приклад переключення музики
        AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;

        // Можна автоматично згорнути меню після вибору
        // CollapseMenu();
    }

    void OnSoundEffectsButtonClick()
    {
        // Тут налаштування звукових ефектів
        Debug.Log("Sound effects settings clicked");

        // Можна автоматично згорнути меню після вибору
        // CollapseMenu();
    }

    // Опціонально: закривати меню при натисканні поза ним
    void Update()
    {
        if (isExpanded && Input.GetMouseButtonDown(0))
        {
            // Перевіряємо чи натиснули поза кнопками
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!IsPointOverButton(mousePos))
            {
                CollapseMenu();
            }
        }
    }

    bool IsPointOverButton(Vector2 point)
    {
        // Простий спосіб перевірити чи знаходиться точка над кнопками
        float distance = Vector2.Distance(point, originalPosition);
        return distance < buttonSpacing * 2.5f; // Збільшуємо зону для зручності
    }
}