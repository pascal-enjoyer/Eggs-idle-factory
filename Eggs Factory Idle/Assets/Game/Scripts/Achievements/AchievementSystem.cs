using UnityEngine;
using System;
using System.Collections.Generic;

public class AchievementSystem : MonoBehaviour
{
    private static AchievementSystem instance;
    public static AchievementSystem Instance
    {
        get
        {
            if (instance == null && !applicationIsQuitting)
            {
                GameObject go = new GameObject("AchievementSystem");
                instance = go.AddComponent<AchievementSystem>();
                DontDestroyOnLoad(go);
                instance.Initialize();
            }
            return instance;
        }
    }


    [SerializeField] private List<AchievementData> achievements = new List<AchievementData>();
    public event Action<AchievementData> OnAchievementUpdated;
    public event Action<AchievementData> OnAchievementLevelUp;

    private static bool applicationIsQuitting = false;
    private bool isInitialized = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        LoadAchievements();
        SubscribeToEvents();
        isInitialized = true;
    }
    private void SubscribeToEvents()
    {
        PlayerEconomy.OnCoinsAdded += HandleCoinsAdded;
        PlayerEconomy.OnExperienceAdded += HandleExperienceAdded;
        PlayerEconomy.OnLevelUp += HandleLevelUp;
        EggButton.OnEggPurchased += HandleEggPurchased;
        EggCollector.OnEggCollected += HandleEggCollected;
        UpgradeSystem.OnUpgradePurchased += HandleUpgradePurchased;
    }
    private void UnsubscribeFromEvents()
    {
        PlayerEconomy.OnCoinsAdded -= HandleCoinsAdded;
        PlayerEconomy.OnExperienceAdded -= HandleExperienceAdded;
        PlayerEconomy.OnLevelUp -= HandleLevelUp;
        EggButton.OnEggPurchased -= HandleEggPurchased;
        EggCollector.OnEggCollected -= HandleEggCollected;
        UpgradeSystem.OnUpgradePurchased -= HandleUpgradePurchased;
    }
    private void Start()
    {
        SubscribeToEvents();
        //Debug.Log("AchievementSystem: ѕодписка на событи€ выполнена");
    }

    private void OnDestroy()
    {
        if (instance == this && !applicationIsQuitting)
        {
            UnsubscribeFromEvents();
        }
    }
    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
    private void LoadAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.Load();
        }
    }


    public List<AchievementData> GetAchievements()
    {
        return achievements;
    }


    private void HandleCoinsAdded(float amount)
    {
        UpdateAchievementProgress(AchievementType.EarnCoins, amount);
    }

    private void HandleExperienceAdded(float amount)
    {
        UpdateAchievementProgress(AchievementType.GainExperience, amount);
    }

    private void HandleLevelUp(int level)
    {
        UpdateAchievementProgress(AchievementType.FactoryLevel, level);
    }

    private void HandleEggPurchased(EggData eggData)
    {
        UpdateAchievementProgress(AchievementType.BuyEggs, 1);
    }

    private void HandleEggCollected()
    {
        UpdateAchievementProgress(AchievementType.CollectEggs, 1);
    }

    private void HandleUpgradePurchased(UpgradeType type)
    {
        UpdateAchievementProgress(AchievementType.BuyUpgrades, 1);
    }

    private void UpdateAchievementProgress(AchievementType type, float amount)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == type)
            {
                if (type == AchievementType.FactoryLevel)
                {
                    // ƒл€ FactoryLevel прогресс равен текущему уровню игрока
                    achievement.CurrentProgress = amount;
                }
                else
                {
                    achievement.CurrentProgress += amount;
                }

                while (achievement.CurrentProgress >= achievement.GetGoalForLevel(achievement.CurrentLevel))
                {
                    achievement.CurrentProgress -= achievement.GetGoalForLevel(achievement.CurrentLevel);
                    achievement.CurrentLevel++;
                    OnAchievementLevelUp.Invoke(achievement);
                    //Debug.Log($"AchievementSystem: ƒостижение {achievement.AchievementName} достигло уровн€ {achievement.CurrentLevel}");
                    // ¬оспроизведение звука достижени€ (ID "1")
                    var audioManager = FindObjectOfType<AudioManager>();
                    if (audioManager != null)
                    {
                        audioManager.PlaySound("1", Vector3.zero);
                        //Debug.Log($"AchievementSystem: Played achievement sound for {achievement.AchievementName}");
                    }
                    else
                    {
                        //Debug.LogWarning("AchievementSystem: AudioManager не найден дл€ воспроизведени€ звука достижени€!");
                    }
                }
                achievement.Save();
                OnAchievementUpdated?.Invoke(achievement);
            }
        }
    }
}