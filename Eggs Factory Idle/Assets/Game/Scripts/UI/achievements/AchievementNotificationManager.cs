using UnityEngine;

public class AchievementNotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationUIPrefab; // ������ �����������

    private void Awake()
    {
        if (notificationUIPrefab == null)
        {
            Debug.LogError("AchievementNotificationManager: Notification UI Prefab �� ��������!");
        }
        else
        {
            Debug.Log("AchievementNotificationManager: ��������������� � ��������: " + notificationUIPrefab.name);
        }
    }

    private void Start()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementLevelUp += ShowNotification;
            Debug.Log("AchievementNotificationManager: �������� �� OnAchievementUpdated");
        }
        else
        {
            Debug.LogError("AchievementNotificationManager: AchievementSystem.Instance �� ������!");
        }
    }

    private void OnDestroy()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementLevelUp -= ShowNotification;
            Debug.Log("AchievementNotificationManager: ������� �� OnAchievementUpdated");
        }
    }

    private void ShowNotification(AchievementData achievement)
    {
        if (notificationUIPrefab == null)
        {
            Debug.LogError("AchievementNotificationManager: Notification UI Prefab �� ��������!");
            return;
        }

        if (UICanvasRef.Instance == null || UICanvasRef.Instance.Canvas == null)
        {
            Debug.LogError("AchievementNotificationManager: UICanvasRef ��� Canvas �� �������!");
            return;
        }

        Debug.Log($"AchievementNotificationManager: ������ ����������� ��� {achievement.AchievementName} �� Canvas: {UICanvasRef.Instance.Canvas.name}");
        GameObject notificationObj = Instantiate(notificationUIPrefab, UICanvasRef.Instance.Canvas.transform, false);
        AchievementNotificationUI notificationUI = notificationObj.GetComponent<AchievementNotificationUI>();
        if (notificationUI != null)
        {
            notificationUI.Show(achievement);
        }
        else
        {
            Debug.LogError("AchievementNotificationManager: AchievementNotificationUI �� ������ �� �������!");
            Destroy(notificationObj);
        }
    }
}