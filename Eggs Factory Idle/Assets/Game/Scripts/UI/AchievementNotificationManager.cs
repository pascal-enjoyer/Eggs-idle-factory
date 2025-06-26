using UnityEngine;

public class AchievementNotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationUIPrefab; // Префаб уведомления

    private void Awake()
    {
        if (notificationUIPrefab == null)
        {
            Debug.LogError("AchievementNotificationManager: Notification UI Prefab не назначен!");
        }
        else
        {
            Debug.Log("AchievementNotificationManager: Инициализирован с префабом: " + notificationUIPrefab.name);
        }
    }

    private void Start()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementLevelUp += ShowNotification;
            Debug.Log("AchievementNotificationManager: Подписан на OnAchievementUpdated");
        }
        else
        {
            Debug.LogError("AchievementNotificationManager: AchievementSystem.Instance не найден!");
        }
    }

    private void OnDestroy()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementLevelUp -= ShowNotification;
            Debug.Log("AchievementNotificationManager: Отписан от OnAchievementUpdated");
        }
    }

    private void ShowNotification(AchievementData achievement)
    {
        if (notificationUIPrefab == null)
        {
            Debug.LogError("AchievementNotificationManager: Notification UI Prefab не назначен!");
            return;
        }

        if (UICanvasRef.Instance == null || UICanvasRef.Instance.Canvas == null)
        {
            Debug.LogError("AchievementNotificationManager: UICanvasRef или Canvas не найдены!");
            return;
        }

        Debug.Log($"AchievementNotificationManager: Создаём уведомление для {achievement.AchievementName} на Canvas: {UICanvasRef.Instance.Canvas.name}");
        GameObject notificationObj = Instantiate(notificationUIPrefab, UICanvasRef.Instance.Canvas.transform, false);
        AchievementNotificationUI notificationUI = notificationObj.GetComponent<AchievementNotificationUI>();
        if (notificationUI != null)
        {
            notificationUI.Show(achievement);
        }
        else
        {
            Debug.LogError("AchievementNotificationManager: AchievementNotificationUI не найден на префабе!");
            Destroy(notificationObj);
        }
    }
}