using UnityEngine;

public class GameModifiers : MonoBehaviour
{
    public static GameModifiers Instance { get; private set; }

    [SerializeField] private UpgradeConfigSO upgradeConfig;
    private int baseConveyorCount = 1; // Default conveyor count

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateModifiers()
    {
        // Notify systems that need to react to upgrades
    }

    public int GetConveyorCount()
    {
        return baseConveyorCount + UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.ConveyorCount);
    }

    public float GetEggIncomeMultiplier()
    {
        return 1f + (UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.EggIncome) * 0.1f);
    }

    public float GetEggExperienceMultiplier()
    {
        return 1f + (UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.EggExperience) * 0.1f);
    }

    public float GetDoubleIncomeChance()
    {
        return UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleIncomeChance) * 0.03f;
    }

    public float GetDoubleExperienceChance()
    {
        return UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleExperienceChance) * 0.03f;
    }

    public float GetEggCostReduction()
    {
        return 1f - (UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.EggCostReduction) * 0.05f);
    }

    public float GetDoubleEggSpawnChance()
    {
        return UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleEggSpawnChance) * 0.03f;
    }

    public float GetDoubleEggPurchaseChance()
    {
        return UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleEggPurchaseChance) * 0.03f;
    }

    public float GetDoubleFruitSpeedChance()
    {
        return UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleFruitSpeedChance) * 0.03f;
    }
}