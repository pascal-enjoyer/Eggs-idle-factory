using UnityEngine;

[CreateAssetMenu(fileName = "EggData", menuName = "Item", order = 1)]
public class EggData : ScriptableObject
{
    public Sprite EggSprite;
    public string EggName;
    public float BaseSpawnInterval = 5f;
    public float BaseUpgradeCost = 10f;
    public float BaseIncome = 1f;
    [System.NonSerialized]
    public bool IsFirstInList = false;
    [System.NonSerialized]
    public int UpgradeLevel = 0;
    [System.NonSerialized]
    public bool IsUnlocked = false;

    public float CurrentSpawnInterval => BaseSpawnInterval;

    public float CurrentIncome
    {
        get
        {
            if (!IsUnlocked || UpgradeLevel == 0) return 0f;
            return CalculateIncome(UpgradeLevel);
        }
    }

    public float CurrentUpgradeCost
    {
        get
        {
            if (!IsUnlocked) return BaseUpgradeCost;
            if (IsFirstInList && UpgradeLevel == 0) return 0f;
            return CalculateUpgradeCost(UpgradeLevel);
        }
    }

    private float CalculateIncome(int level)
    {
        return BaseIncome * level;
    }

    private float CalculateUpgradeCost(int level)
    {
        if (level == 0) return BaseUpgradeCost;
        return BaseUpgradeCost + BaseUpgradeCost / 2f * level;
    }
}