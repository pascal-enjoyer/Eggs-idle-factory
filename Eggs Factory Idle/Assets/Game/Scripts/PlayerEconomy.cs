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
                Debug.LogWarning("PlayerEconomy.Instance вызван во время выхода приложения.");
                return null;
            }

            if (instance == null)
            {
                if (Application.isPlaying)
                {
                    GameObject go = new GameObject("PlayerEconomy");
                    instance = go.AddComponent<PlayerEconomy>();
                    DontDestroyOnLoad(go);
                    Debug.Log("PlayerEconomy: Создан новый синглтон");
                }
                else
                {
                    Debug.LogWarning("PlayerEconomy: Попытка создания синглтона вне игрового режима.");
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
            //Debug.LogWarning($"PlayerEconomy: Обнаружен дубликат синглтона на {gameObject.name}. Уничтожаем этот экземпляр.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    private void Start()
    {
        // Запускаем музыку с ID "0" и настраиваем цикл
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayMusic("0");
            StartCoroutine(CycleMusic(audioManager));
        }
        else
        {
            //Debug.LogWarning("PlayerEconomy: AudioManager не найден в сцене!");
        }
    }

    private System.Collections.IEnumerator CycleMusic(AudioManager audioManager)
    {
        while (true)
        {
            // Ожидаем окончания текущего трека (примерно, точная длительность обрабатывается в AudioManager)
            yield return new WaitForSeconds(60f); // Замените на реальную длительность трека или используйте clip.length
            string nextMusicId = audioManager.GetMusicVolume() == 0 ? "0" : (audioManager.GetMusicVolume() == 1 ? "0" : "1");
            audioManager.PlayMusic(nextMusicId);
        }
    }

    private void OnDestroy()
    {
        if (instance == this && !applicationIsQuitting)
        {
            //Debug.LogWarning("PlayerEconomy: Основной экземпляр уничтожается. Сбрасываем instance.");
            instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(LevelLoader.mainMenuName);
    }

    public void AddCoins(float amount)
    {
        if (amount <= 0)
        {
            //Debug.LogWarning($"AddCoins вызван с неположительным значением: {amount}. Используйте SpendCoins для списания.");
            return;
        }

        coins += amount;
        SaveCoins();
        CoinsChanged?.Invoke();
        OnCoinsAdded?.Invoke(amount);
        //Debug.Log($"Добавлено монет: {amount}, итого: {coins}");
    }

    public void SpendCoins(float amount)
    {
        if (amount <= 0)
        {
            //Debug.LogWarning($"SpendCoins вызван с неположительным значением: {amount}");
            return;
        }

        if (HaveEnoughCoinsToBuy(amount))
        {
            coins -= amount;
            SaveCoins();
            CoinsChanged?.Invoke();
            //Debug.Log($"Списано монет: {amount}, итого: {coins}");
        }
        else
        {
            //Debug.LogWarning($"Недостаточно монет для списания: требуется {amount}, есть {coins}");
        }
    }

    public bool HaveEnoughCoinsToBuy(float amount)
    {
        if (amount < 0)
        {
            //Debug.LogWarning($"HaveEnoughCoinsToBuy вызван с отрицательным значением: {amount}");
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
            //Debug.LogWarning($"AddExperience вызван с неположительным значением: {amount}");
            return;
        }

        experience += amount;
        OnExperienceAdded?.Invoke(amount);
        int previousLevel = level;
        while (experience >= GetExperienceForNextLevel(level))
        {
            experience -= GetExperienceForNextLevel(level);
            level++;
            UpgradeSystem.Instance?.AddUpgradePoints(5);
            LevelChanged?.Invoke();
            OnLevelUp?.Invoke(level);
            // Воспроизведение звука повышения уровня (ID "0")
            var audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.PlaySound("0", Vector3.zero);
                //Debug.Log($"PlayerEconomy: Played level up sound for level {level}");
            }
            else
            {
                //Debug.LogWarning("PlayerEconomy: AudioManager не найден для воспроизведения звука уровня!");
            }
        }
        SaveExperienceAndLevel();
        ExperienceChanged?.Invoke();
        //Debug.Log($"Добавлено опыта: {amount}, итого: {experience}");
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