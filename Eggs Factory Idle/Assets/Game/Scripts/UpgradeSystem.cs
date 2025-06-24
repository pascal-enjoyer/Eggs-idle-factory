using UnityEngine;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance { get; private set; }

    [SerializeField] private UpgradeConfigSO upgradeConfig;
    private Dictionary<UpgradeType, UpgradeData> upgradeData = new Dictionary<UpgradeType, UpgradeData>();
    private int upgradePoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUpgrades();
        LoadUpgradePoints();
    }

    private void InitializeUpgrades()
    {
        foreach (var upgrade in upgradeConfig.Upgrades)
        {
            upgradeData[upgrade.Type] = new UpgradeData
            {
                CurrentLevel = PlayerPrefs.GetInt($"Upgrade_{upgrade.Type}", 0),
                MaxLevel = upgrade.MaxLevel,
                CostPerLevel = upgrade.CostPerLevel
            };
        }
    }

    private void LoadUpgradePoints()
    {
        upgradePoints = PlayerPrefs.GetInt("UpgradePoints", 0);
    }

    private void SaveUpgradePoints()
    {
        PlayerPrefs.SetInt("UpgradePoints", upgradePoints);
        PlayerPrefs.Save();
    }

    private void SaveUpgradeLevel(UpgradeType type)
    {
        PlayerPrefs.SetInt($"Upgrade_{type}", upgradeData[type].CurrentLevel);
        PlayerPrefs.Save();
    }

    public bool CanPurchaseUpgrade(UpgradeType type)
    {
        if (!upgradeData.ContainsKey(type)) return false;
        var data = upgradeData[type];
        return data.CurrentLevel < data.MaxLevel && upgradePoints >= data.CostPerLevel;
    }

    public void PurchaseUpgrade(UpgradeType type)
    {
        if (!CanPurchaseUpgrade(type)) return;
        var data = upgradeData[type];
        upgradePoints -= data.CostPerLevel;
        data.CurrentLevel++;
        SaveUpgradePoints();
        SaveUpgradeLevel(type);
        ApplyUpgradeEffects(type);
    }

    public void AddUpgradePoints(int points)
    {
        upgradePoints += points;
        SaveUpgradePoints();
    }

    public int GetUpgradePoints() => upgradePoints;

    public int GetUpgradeLevel(UpgradeType type) => upgradeData.ContainsKey(type) ? upgradeData[type].CurrentLevel : 0;

    private void ApplyUpgradeEffects(UpgradeType type)
    {
        // Effects are applied through GameModifiers
        GameModifiers.Instance.UpdateModifiers();
    }
}

public enum UpgradeType
{
    ConveyorCount,
    EggIncome,
    EggExperience,
    DoubleIncomeChance,
    DoubleExperienceChance,
    EggCostReduction,
    DoubleEggSpawnChance,
    DoubleEggPurchaseChance,
    DoubleFruitSpeedChance
}

[System.Serializable]
public class UpgradeData
{
    public int CurrentLevel;
    public int MaxLevel;
    public int CostPerLevel;
}