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
        }
    }
    private void OnEnable()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUpdated += UpdateAchievementUI;
            InitializeUI();
        }
    }

    private void OnDisable()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUpdated -= UpdateAchievementUI;
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