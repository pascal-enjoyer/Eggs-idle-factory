using UnityEngine;
using System;

public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }
    private int coins;
    private int experience;
    private int level;
    private const string COINS_KEY = "PlayerCoins";
    private const string EXP_KEY = "PlayerExperience";
    private const string LEVEL_KEY = "PlayerLevel";

    public event Action CoinsChanged;
    public event Action ExperienceChanged;
    public event Action LevelChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

/*    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }*/

    public void AddCoins(int amount)
    {
        bool adding = true;
        if (amount < 0 && HaveEnoughCoinsToBuy(amount))
            adding = true;
        else if (amount < 0)
            adding = false;
        if (adding)
        {
            float doubleChance = GameModifiers.Instance.GetDoubleIncomeChance();
            if (UnityEngine.Random.value < doubleChance)
                amount *= 2;
            amount = Mathf.RoundToInt(amount * GameModifiers.Instance.GetEggIncomeMultiplier());
            coins += amount;
            SaveCoins();
            CoinsChanged?.Invoke();
        }
    }



    public bool HaveEnoughCoinsToBuy(int amount)
    {
        float reduction = GameModifiers.Instance.GetEggCostReduction();
        amount = Mathf.RoundToInt(amount * reduction);
        return coins - Math.Abs(amount) >= 0;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void AddExperience(int amount)
    {
        float doubleChance = GameModifiers.Instance.GetDoubleExperienceChance();
        if (UnityEngine.Random.value < doubleChance)
            amount *= 2;
        amount = Mathf.RoundToInt(amount * GameModifiers.Instance.GetEggExperienceMultiplier());
        experience += amount;
        int previousLevel = level;
        while (experience >= GetExperienceForNextLevel(level))
        {
            experience -= GetExperienceForNextLevel(level);
            level++;
            UpgradeSystem.Instance.AddUpgradePoints(5); // Placeholder: 5 points per level
            LevelChanged?.Invoke();
        }
        SaveExperienceAndLevel();
        ExperienceChanged?.Invoke();
    }

    public int GetExperience()
    {
        return experience;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetExperienceForNextLevel()
    {
        return GetExperienceForNextLevel(level);
    }

    private int GetExperienceForNextLevel(int currentLevel)
    {
        return (int)(5 + 16.5f * currentLevel + 3.5f * currentLevel * currentLevel);
    }

    private void LoadData()
    {
        coins = PlayerPrefs.GetInt(COINS_KEY, 0);
        experience = PlayerPrefs.GetInt(EXP_KEY, 0);
        level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        CoinsChanged?.Invoke();
        ExperienceChanged?.Invoke();
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_KEY, coins);
        PlayerPrefs.Save();
    }

    private void SaveExperienceAndLevel()
    {
        PlayerPrefs.SetInt(EXP_KEY, experience);
        PlayerPrefs.SetInt(LEVEL_KEY, level);
        PlayerPrefs.Save();
    }
}