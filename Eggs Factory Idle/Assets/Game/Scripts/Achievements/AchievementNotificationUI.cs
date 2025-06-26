using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementNotificationUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;
    [SerializeField] private Text achievementNameText; // Legacy Text
    [SerializeField] private Text achievementLevelText; // Legacy Text
    [SerializeField] private float displayDuration = 3f; // ����� ����������� �����������
    public float DisplayDuration => displayDuration; // ����� ����������� �����������
    [SerializeField] private float fadeInDuration = 0.5f; // ������������ ���������
    [SerializeField] private float fadeOutDuration = 0.5f; // ������������ ������������

    private Image backgroundImage; // Image �� ������� �� ��������
    private Color iconOriginalColor;
    private Color nameTextOriginalColor;
    private Color levelTextOriginalColor;
    private Color backgroundOriginalColor;

    private void Awake()
    {
        // �������� Image �� �������
        backgroundImage = GetComponent<Image>();
        if (backgroundImage == null)
        {
            Debug.LogWarning("AchievementNotificationUI: Image component not found on this GameObject!");
            backgroundImage = gameObject.AddComponent<Image>();
        }

        // ��������� �������� �����
        iconOriginalColor = achievementIcon.color;
        nameTextOriginalColor = achievementNameText.color;
        levelTextOriginalColor = achievementLevelText.color;
        backgroundOriginalColor = backgroundImage.color;

        // ������������� ��������� ������������ � 0
        SetAlpha(0f);
    }

    public void Show(AchievementData achievement)
    {
        // ��������� UI
        achievementIcon.sprite = achievement.AchievementIcon;
        achievementNameText.text = achievement.AchievementName;
        achievementLevelText.text = $"Level {achievement.CurrentLevel}";

        // ��������� ��������
        StartCoroutine(ShowNotification());
    }

    private IEnumerator ShowNotification()
    {
        // ������� ���������
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1f); // ��������, ��� ����� ����� 1

        // �������� ������� �����������
        yield return new WaitForSeconds(displayDuration);

        // ������� ������������
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f); // ��������, ��� ����� ����� 0

        // ���������� ������ �����������
        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        // �������� �����-����� ��� ������� ��������, ������ � �������
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