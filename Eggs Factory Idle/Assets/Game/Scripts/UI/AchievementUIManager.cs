using UnityEngine;
using UnityEngine.UI;

public class AchievementUIManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private GameObject achievementUIPrefab;

    private void Start()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUpdated += UpdateAchievementUI;
            InitializeUI();
            Debug.Log("AchievementUIManager: Подписка на AchievementSystem выполнена");
        }
        else
        {
            Debug.LogError("AchievementUIManager: AchievementSystem.Instance не инициализирован!");
        }
    }

    private void OnDestroy()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUpdated -= UpdateAchievementUI;
        }
    }

    private void InitializeUI()
    {
        if (gridLayout == null || achievementUIPrefab == null)
        {
            Debug.LogError("AchievementUIManager: gridLayout или achievementUIPrefab не назначены!");
            return;
        }

        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var achievement in AchievementSystem.Instance.GetAchievements())
        {
            GameObject uiElement = Instantiate(achievementUIPrefab, gridLayout.transform);
            AchievementUIElement uiComponent = uiElement.GetComponent<AchievementUIElement>();
            if (uiComponent != null)
            {
                uiComponent.Initialize(achievement);
            }
            else
            {
                Debug.LogError("AchievementUIManager: achievementUIPrefab не содержит AchievementUIElement!");
                Destroy(uiElement);
            }
        }
    }

    private void UpdateAchievementUI(AchievementData achievement)
    {
        foreach (Transform child in gridLayout.transform)
        {
            AchievementUIElement uiElement = child.GetComponent<AchievementUIElement>();
            if (uiElement != null && uiElement.Achievement == achievement)
            {
                uiElement.UpdateUI();
            }
        }
    }
}