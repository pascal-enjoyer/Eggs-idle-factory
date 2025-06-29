using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;

public class PlayerEconomy : MonoBehaviour
{
    private static PlayerEconomy instance;
    private static bool applicationIsQuitting = false;

    public static PlayerEconomy Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            if (instance == null)
            {
                if (Application.isPlaying)
                {
                    GameObject go = new GameObject("PlayerEconomy");
                    instance = go.AddComponent<PlayerEconomy>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private float coins;
    private float experience;
    private int level;
    private const string COINS_KEY = "PlayerCoins";
    private const string EXP_KEY = "PlayerExperience";
    private const string LEVEL_KEY = "PlayerLevel";

    public static event Action<float> OnCoinsAdded;
    public static event Action<float> OnExperienceAdded;
    public static event Action<int> OnLevelUp;
    public event Action CoinsChanged;
    public event Action ExperienceChanged;
    public event Action LevelChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
    }


    private void OnDestroy()
    {
        if (instance == this && !applicationIsQuitting)
        {
            instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    public void DeleteAll()
    {
        coins = 0;
        experience = 0;
        level = 0;
        UpgradeSystem.Instance.upgradePoints = 0;

        UpgradeSystem.Instance.ClearUpgradeData();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        CoinsChanged?.Invoke();
        ExperienceChanged?.Invoke();
        LevelChanged?.Invoke();

        SceneManager.LoadScene(LevelLoader.mainMenuName);
    }

    public void AddCoins(float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound("4", Vector3.zero);
        }
        coins += amount;
        SaveCoins();
        CoinsChanged?.Invoke();
        OnCoinsAdded?.Invoke(amount);
    }

    public void SpendCoins(float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (HaveEnoughCoinsToBuy(amount))
        {
            coins -= amount;
            SaveCoins();
            CoinsChanged?.Invoke();
        }
    }

    public bool HaveEnoughCoinsToBuy(float amount)
    {
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }

        return coins >= amount;
    }

    public int GetCoins()
    {
        return Mathf.FloorToInt(coins);
    }

    public float GetCoinsFloat()
    {
        return coins;
    }

    public void AddExperience(float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        experience += amount;
        OnExperienceAdded?.Invoke(amount);
        int previousLevel = level;
        while (experience >= GetExperienceForNextLevel(level))
        {
            experience -= GetExperienceForNextLevel(level);
            level++;
            UpgradeSystem.Instance?.AddUpgradePoints(level / 5 + 2);
            LevelChanged?.Invoke();
            OnLevelUp?.Invoke(level);

            var audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.PlaySound("0", Vector3.zero);
            }
        }
        SaveExperienceAndLevel();
        ExperienceChanged?.Invoke();
    }

    public int GetExperience()
    {
        return Mathf.FloorToInt(experience);
    }

    public float GetExperienceFloat()
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
        coins = PlayerPrefs.GetFloat(COINS_KEY, 0f);
        experience = PlayerPrefs.GetFloat(EXP_KEY, 0f);
        level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        CoinsChanged?.Invoke();
        ExperienceChanged?.Invoke();
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetFloat(COINS_KEY, coins);
        PlayerPrefs.Save();
    }

    private void SaveExperienceAndLevel()
    {
        PlayerPrefs.SetFloat(EXP_KEY, experience);
        PlayerPrefs.SetInt(LEVEL_KEY, level);
        PlayerPrefs.Save();
    }
}