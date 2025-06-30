using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "Achievement", order = 2)]
public class AchievementData : ScriptableObject
{
    public string AchievementName;
    public Sprite AchievementIcon;
    public AchievementType Type;
    public float BaseGoal = 100f;
    public float GoalMultiplier = 2f;

    [System.NonSerialized]
    public int CurrentLevel;
    [System.NonSerialized]
    public float CurrentProgress;

    private const string LEVEL_KEY_PREFIX = "Achievement_Level_";
    private const string PROGRESS_KEY_PREFIX = "Achievement_Progress_";

    public void Load()
    {
        CurrentLevel = PlayerPrefs.GetInt(LEVEL_KEY_PREFIX + AchievementName, 0);
        CurrentProgress = PlayerPrefs.GetFloat(PROGRESS_KEY_PREFIX + AchievementName, 0f);
    }

    public void Save()
    {
        PlayerPrefs.SetInt(LEVEL_KEY_PREFIX + AchievementName, CurrentLevel);
        PlayerPrefs.SetFloat(PROGRESS_KEY_PREFIX + AchievementName, CurrentProgress);
        PlayerPrefs.Save();
    }

    public float GetGoalForLevel(int level)
    {
        return BaseGoal * Mathf.Pow(GoalMultiplier, level);
    }

    public void Clear()
    {
        CurrentLevel = 0;
        CurrentProgress = 0f;
        Save();
    }
}

public enum AchievementType
{
    CollectEggs,
    BuyEggs,
    EarnCoins,
    GainExperience,
    FactoryLevel,
    BuyUpgrades
}