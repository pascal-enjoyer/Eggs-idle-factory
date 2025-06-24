using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Game/UpgradeConfig", order = 2)]
public class UpgradeConfigSO : ScriptableObject
{
    [System.Serializable]
    public class UpgradeInfo
    {
        public UpgradeType Type;
        public int MaxLevel;
        public int CostPerLevel;
    }

    public List<UpgradeInfo> Upgrades = new List<UpgradeInfo>
    {
        new UpgradeInfo { Type = UpgradeType.ConveyorCount, MaxLevel = 5, CostPerLevel = 2 },
        new UpgradeInfo { Type = UpgradeType.EggIncome, MaxLevel = 10, CostPerLevel = 1 },
        new UpgradeInfo { Type = UpgradeType.EggExperience, MaxLevel = 10, CostPerLevel = 1 },
        new UpgradeInfo { Type = UpgradeType.DoubleIncomeChance, MaxLevel = 10, CostPerLevel = 2 },
        new UpgradeInfo { Type = UpgradeType.DoubleExperienceChance, MaxLevel = 10, CostPerLevel = 2 },
        new UpgradeInfo { Type = UpgradeType.EggCostReduction, MaxLevel = 10, CostPerLevel = 1 },
        new UpgradeInfo { Type = UpgradeType.DoubleEggSpawnChance, MaxLevel = 10, CostPerLevel = 2 },
        new UpgradeInfo { Type = UpgradeType.DoubleEggPurchaseChance, MaxLevel = 10, CostPerLevel = 2 },
        new UpgradeInfo { Type = UpgradeType.DoubleFruitSpeedChance, MaxLevel = 10, CostPerLevel = 2 }
    };
}