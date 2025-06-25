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
            Debug.Log("AchievementUIManager: �������� �� AchievementSystem ���������");
        }
        else
        {
            Debug.LogError("AchievementUIManager: AchievementSystem.Instance �� ���������������!");
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
            Debug.LogError("AchievementUIManager: gridLayout ��� achievementUIPrefab �� ���������!");
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
                Debug.LogError("AchievementUIManager: achievementUIPrefab �� �������� AchievementUIElement!");
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