using UnityEngine;

public class GameModifiers : MonoBehaviour
{
    public static GameModifiers Instance { get; private set; }

    [SerializeField] private UpgradeConfigSO upgradeConfig;

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
    }

    public int GetConveyorCount()
    {
        return 1 + UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.ConveyorCount);
    }

    public float GetEggIncomeMultiplier()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.EggIncome);
        return 1f + level * 0.1f;
    }

    public float GetEggExperienceMultiplier()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.EggExperience);
        return 1f + level * 0.1f;
    }

    public float GetDoubleIncomeChance()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleIncomeChance);
        return level * 0.03f;
    }

    public float GetDoubleExperienceChance()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleExperienceChance);
        return level * 0.03f;
    }

    public float GetEggCostReduction()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.EggCostReduction);
        return 1f - level * 0.05f;
    }

    public float GetDoubleEggSpawnChance()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleEggSpawnChance);
        return level * 0.03f;
    }

    public float GetDoubleEggPurchaseChance()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleEggPurchaseChance);
        return level * 0.03f;
    }

    public float GetDoubleFruitSpeedChance()
    {
        int level = UpgradeSystem.Instance.GetUpgradeLevel(UpgradeType.DoubleFruitSpeedChance);
        return level * 0.03f;
    }
}