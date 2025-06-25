using UnityEngine;
using UnityEngine.UI;

public class AchievementUIElement : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text progressText;

    public AchievementData Achievement { get; private set; }

    public void Initialize(AchievementData achievement)
    {
        Achievement = achievement;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (icon != null)
            icon.sprite = Achievement.AchievementIcon;
        if (nameText != null)
            nameText.text = Achievement.AchievementName;
        if (levelText != null)
            levelText.text = $"Level: {Achievement.CurrentLevel}";
        if (progressText != null)
        {
            if (Achievement.Type == AchievementType.FactoryLevel)
            {
                // Для FactoryLevel показываем текущий уровень игрока
                progressText.text = $"{Mathf.FloorToInt(Achievement.CurrentProgress)}/{Mathf.FloorToInt(Achievement.GetGoalForLevel(Achievement.CurrentLevel))}";
            }
            else
            {
                progressText.text = $"{Mathf.FloorToInt(Achievement.CurrentProgress)}/{Mathf.FloorToInt(Achievement.GetGoalForLevel(Achievement.CurrentLevel))}";
            }
        }
    }
}