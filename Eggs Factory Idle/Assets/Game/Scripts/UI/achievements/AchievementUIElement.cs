using UnityEngine;
using UnityEngine.UI;

public class AchievementUIElement : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text text;

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
        if (text!= null)
            text.text = Achievement.AchievementName + $"\nLevel: {Achievement.CurrentLevel}";
        if (Achievement.Type == AchievementType.FactoryLevel)
        {
            // Для FactoryLevel показываем текущий уровень игрока
            text.text += $"\n{Mathf.FloorToInt(Achievement.CurrentProgress)}/{Mathf.FloorToInt(Achievement.GetGoalForLevel(Achievement.CurrentLevel))}";
            }
            else
            {
            text.text += $"\n{Mathf.FloorToInt(Achievement.CurrentProgress)}/{Mathf.FloorToInt(Achievement.GetGoalForLevel(Achievement.CurrentLevel))}";
            }
    }
}