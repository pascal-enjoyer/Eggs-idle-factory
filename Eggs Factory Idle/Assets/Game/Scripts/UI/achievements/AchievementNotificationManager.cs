using UnityEngine;

public class AchievementNotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationUIPrefab;

    private void Start()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementLevelUp += ShowNotification;
        }
    }

    private void OnDestroy()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementLevelUp -= ShowNotification;
        }
    }

    private void ShowNotification(AchievementData achievement)
    {
        if (notificationUIPrefab == null)
        {
            return;
        }

        if (UICanvasRef.Instance == null || UICanvasRef.Instance.Canvas == null)
        {
            return;
        }
        GameObject notificationObj = Instantiate(notificationUIPrefab, UICanvasRef.Instance.Canvas.transform, false);
        AchievementNotificationUI notificationUI = notificationObj.GetComponent<AchievementNotificationUI>();
        if (notificationUI != null)
        {
            notificationUI.Show(achievement);
        }
        else
        {
            Destroy(notificationObj);
        }
    }
}