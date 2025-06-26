using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementNotificationUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;
    [SerializeField] private Text achievementNameText; // Legacy Text
    [SerializeField] private Text achievementLevelText; // Legacy Text
    [SerializeField] private float displayDuration = 3f; // Время отображения уведомления
    public float DisplayDuration => displayDuration; // Время отображения уведомления
    [SerializeField] private float fadeInDuration = 0.5f; // Длительность появления
    [SerializeField] private float fadeOutDuration = 0.5f; // Длительность исчезновения

    private Image backgroundImage; // Image на объекте со скриптом
    private Color iconOriginalColor;
    private Color nameTextOriginalColor;
    private Color levelTextOriginalColor;
    private Color backgroundOriginalColor;

    private void Awake()
    {
        // Получаем Image на объекте
        backgroundImage = GetComponent<Image>();
        if (backgroundImage == null)
        {
            Debug.LogWarning("AchievementNotificationUI: Image component not found on this GameObject!");
            backgroundImage = gameObject.AddComponent<Image>();
        }

        // Сохраняем исходные цвета
        iconOriginalColor = achievementIcon.color;
        nameTextOriginalColor = achievementNameText.color;
        levelTextOriginalColor = achievementLevelText.color;
        backgroundOriginalColor = backgroundImage.color;

        // Устанавливаем начальную прозрачность в 0
        SetAlpha(0f);
    }

    public void Show(AchievementData achievement)
    {
        // Настройка UI
        achievementIcon.sprite = achievement.AchievementIcon;
        achievementNameText.text = achievement.AchievementName;
        achievementLevelText.text = $"Level {achievement.CurrentLevel}";

        // Запускаем анимацию
        StartCoroutine(ShowNotification());
    }

    private IEnumerator ShowNotification()
    {
        // Плавное появление
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1f); // Убедимся, что альфа точно 1

        // Ожидание времени отображения
        yield return new WaitForSeconds(displayDuration);

        // Плавное исчезновение
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f); // Убедимся, что альфа точно 0

        // Уничтожаем объект уведомления
        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        // Изменяем альфа-канал для фоновой картинки, иконки и текстов
        Color backgroundColor = backgroundOriginalColor;
        backgroundColor.a = alpha;
        backgroundImage.color = backgroundColor;

        Color iconColor = iconOriginalColor;
        iconColor.a = alpha;
        achievementIcon.color = iconColor;

        Color nameTextColor = nameTextOriginalColor;
        nameTextColor.a = alpha;
        achievementNameText.color = nameTextColor;

        Color levelTextColor = levelTextOriginalColor;
        levelTextColor.a = alpha;
        achievementLevelText.color = levelTextColor;
    }
}