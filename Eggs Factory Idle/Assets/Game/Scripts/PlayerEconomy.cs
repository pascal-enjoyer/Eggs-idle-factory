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
        coins += amount;
        SaveCoins();
        CoinsChanged?.Invoke();
    }

    public int GetCoins()
    {
        return coins;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        while (experience >= GetExperienceForNextLevel(level))
        {
            experience -= GetExperienceForNextLevel(level);
            level++;
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