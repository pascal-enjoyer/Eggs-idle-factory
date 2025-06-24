using UnityEngine;

[CreateAssetMenu(fileName = "EggData", menuName = "Game/EggData", order = 1)]
public class EggData : ScriptableObject
{
    public Sprite EggSprite;
    public string EggName;
    public float BaseSpawnInterval = 5f;
    public int BaseUpgradeCost = 10;
    public int BaseIncome = 1;
    [System.NonSerialized]
    public bool IsFirstInList = false; // Это будем устанавливать извне
    [System.NonSerialized]
    public int UpgradeLevel = 0;
    [System.NonSerialized]
    public bool IsUnlocked = false;

    public float CurrentSpawnInterval => BaseSpawnInterval;

    public int CurrentIncome
    {
        get
        {
            if (!IsUnlocked || UpgradeLevel == 0) return 0;
            return CalculateIncome(UpgradeLevel);
        }
    }

    public int CurrentUpgradeCost
    {
        get
        {
            if (!IsUnlocked) return BaseUpgradeCost;
            if (IsFirstInList && UpgradeLevel == 0) return 0;
            return CalculateUpgradeCost(UpgradeLevel);
        }
    }

    private int CalculateIncome(int level)
    {
        return BaseIncome * level;
    }

    private int CalculateUpgradeCost(int level)
    {
        if (level == 0) return BaseUpgradeCost;
        else 
        return BaseUpgradeCost + BaseUpgradeCost/2*level;
    }
}