using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private Image experienceBar;
    [SerializeField] private Text experienceText; // Legacy Text
    [SerializeField] private Color highlightColor = Color.white; // Цвет для моргания (контрастный)
    [SerializeField] private float blinkDuration = 2f; // Длительность эффекта
    [SerializeField] private float blinkSpeed = 2f; // Скорость моргания
    private bool isSubscribed;
    private Color originalColor;

    private void Awake()
    {
        if (experienceText != null)
        {
            originalColor = experienceText.color; // Сохраняем исходный цвет
        }
        else
        {
            Debug.LogError("ExperienceDisplay: experienceText не назначен!");
        }
    }

    private void Start()
    {
        TrySubscribe();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (PlayerEconomy.Instance != null && isSubscribed)
        {
            PlayerEconomy.Instance.ExperienceChanged -= UpdateExperienceDisplay;
            PlayerEconomy.OnLevelUp -= HandleLevelUp;
            isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (!isSubscribed && PlayerEconomy.Instance != null)
        {
            PlayerEconomy.Instance.ExperienceChanged += UpdateExperienceDisplay;
            PlayerEconomy.OnLevelUp += HandleLevelUp;
            isSubscribed = true;
            UpdateExperienceDisplay();
        }
        else
        {
            Debug.LogWarning("ExperienceDisplay: PlayerEconomy.Instance не найден!");
        }
    }

    private void UpdateExperienceDisplay()
    {
        if (PlayerEconomy.Instance == null)
        {
            Debug.LogWarning("ExperienceDisplay: PlayerEconomy.Instance is null!");
            return;
        }

        int currentExp = PlayerEconomy.Instance.GetExperience();
        int requiredExp = PlayerEconomy.Instance.GetExperienceForNextLevel();
        int level = PlayerEconomy.Instance.GetLevel();

        experienceBar.fillAmount = (float)currentExp / requiredExp;
        experienceText.text = $"Level {level}: {currentExp}/{requiredExp}";
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log($"ExperienceDisplay: Повышение уровня до {newLevel}. Запускаем моргание.");
        StopAllCoroutines(); // Останавливаем предыдущие анимации
        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            // Плавное изменение цвета между originalColor и highlightColor
            float t = Mathf.PingPong(elapsedTime * blinkSpeed, 1f);
            experienceText.color = Color.Lerp(originalColor, highlightColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Возвращаем исходный цвет
        experienceText.color = originalColor;
    }
}