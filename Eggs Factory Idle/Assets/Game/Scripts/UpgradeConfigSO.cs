using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Game/UpgradeConfig", order = 2)]
public class UpgradeConfigSO : ScriptableObject
{
    [System.Serializable]
    public class UpgradeInfo
    {
        public UpgradeType Type; // “ип апгрейда
        public int MaxLevel; // ћаксимальный уровень
        public int CostPerLevel; // —тоимость за уровень
        public Sprite Icon; // »конка апгрейда
        public string Description; // ќписание апгрейда
    }

    public List<UpgradeInfo> Upgrades = new List<UpgradeInfo>
    {
        new UpgradeInfo { Type = UpgradeType.ConveyorCount, MaxLevel = 5, CostPerLevel = 2, Description = "”величивает количество конвейерных этажей на 1" },
        new UpgradeInfo { Type = UpgradeType.EggIncome, MaxLevel = 10, CostPerLevel = 1, Description = "”величивает доход за каждое €йцо на 10%" },
        new UpgradeInfo { Type = UpgradeType.EggExperience, MaxLevel = 10, CostPerLevel = 1, Description = "”величивает опыт за каждое €йцо на 10%" },
        new UpgradeInfo { Type = UpgradeType.DoubleIncomeChance, MaxLevel = 10, CostPerLevel = 2, Description = "”величивает шанс двойного дохода за €йцо на 3%" },
        new UpgradeInfo { Type = UpgradeType.DoubleExperienceChance, MaxLevel = 10, CostPerLevel = 2, Description = "”величивает шанс двойного опыта за €йцо на 3%" },
        new UpgradeInfo { Type = UpgradeType.EggCostReduction, MaxLevel = 10, CostPerLevel = 1, Description = "”меньшает стоимость €иц на 5%" },
        new UpgradeInfo { Type = UpgradeType.DoubleEggSpawnChance, MaxLevel = 10, CostPerLevel = 2, Description = "”величивает шанс по€влени€ двух €иц на 3%" },
        new UpgradeInfo { Type = UpgradeType.DoubleEggPurchaseChance, MaxLevel = 10, CostPerLevel = 2, Description = "”величивает шанс купить два €йца по цене одного на 3%" },
        new UpgradeInfo { Type = UpgradeType.DoubleFruitSpeedChance, MaxLevel = 10, CostPerLevel = 2, Description = "”величивает шанс удвоени€ скорости спавна €иц на 3%" }
    };
}