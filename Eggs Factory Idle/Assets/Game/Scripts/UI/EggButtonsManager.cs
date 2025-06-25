using UnityEngine;
using System.Collections.Generic;

public class EggButtonsManager : MonoBehaviour
{
    [SerializeField] private List<EggData> _allEggData; // Список всех яиц
    [SerializeField] private EggButton _eggButtonPrefab; // Префаб кнопки яйца
    [SerializeField] private Transform _buttonsParent; // Родительский объект для кнопок

    private List<EggButton> _spawnedButtons = new List<EggButton>();

    private void Start()
    {
        LoadEggsData(); // Загружаем сохраненные данные
        InitializeEggs();
        InitializeEggTimers(); // Инициализируем таймеры для прокачанных яиц
        CreateButtons();
    }

    private void LoadEggsData()
    {
        for (int i = 0; i < _allEggData.Count; i++)
        {
            var egg = _allEggData[i];
            // Загружаем данные только для первого яйца или если оно разблокировано
            egg.IsUnlocked = (i == 0) || PlayerPrefs.GetInt($"Egg_{egg.EggName}_IsUnlocked", 0) == 1;
            egg.UpgradeLevel = PlayerPrefs.GetInt($"Egg_{egg.EggName}_UpgradeLevel", 0);
            Debug.Log($"Загружено яйцо {egg.EggName}: IsUnlocked={egg.IsUnlocked}, UpgradeLevel={egg.UpgradeLevel}");
        }
    }

    private void SaveEggData(EggData eggData)
    {
        PlayerPrefs.SetInt($"Egg_{eggData.EggName}_IsUnlocked", eggData.IsUnlocked ? 1 : 0);
        PlayerPrefs.SetInt($"Egg_{eggData.EggName}_UpgradeLevel", eggData.UpgradeLevel);
        PlayerPrefs.Save();
        Debug.Log($"Сохранено яйцо {eggData.EggName}: IsUnlocked={eggData.IsUnlocked}, UpgradeLevel={eggData.UpgradeLevel}");
    }

    private void InitializeEggs()
    {
        // Устанавливаем первое яйцо как разблокированное, если не загружено иное
        for (int i = 0; i < _allEggData.Count; i++)
        {
            _allEggData[i].IsFirstInList = (i == 0);
        }
    }

    private void InitializeEggTimers()
    {
        // Добавляем прокачанные яйца в EggSpawnSystem
        foreach (var eggData in _allEggData)
        {
            if (eggData.IsUnlocked && eggData.UpgradeLevel > 0)
            {
                EggSpawnSystem.Instance.AddEgg(eggData);
                Debug.Log($"Добавлен таймер для яйца {eggData.EggName} с уровнем {eggData.UpgradeLevel}");
            }
        }
    }

    private void CreateButtons()
    {
        if (_eggButtonPrefab == null || _buttonsParent == null)
        {
            Debug.LogError("EggButtonsManager: Не заданы _eggButtonPrefab или _buttonsParent!");
            return;
        }

        for (int i = 0; i < _allEggData.Count; i++)
        {
            EggData eggData = _allEggData[i];
            var button = Instantiate(_eggButtonPrefab, _buttonsParent);
            button.Initialize(eggData);
            _spawnedButtons.Add(button);
            button.UnlockNextEgg.AddListener(UnlockNextEgg);
            // Затемняем, если не разблокировано
            if (!eggData.IsUnlocked)
            {
                button.SetDarkStyle(true);
            }
        }
    }

    public void UnlockNextEgg(EggData currentEgg)
    {
        int index = _allEggData.IndexOf(currentEgg);
        if (index >= 0 && index < _allEggData.Count - 1)
        {
            _allEggData[index + 1].IsUnlocked = true;
            SaveEggData(_allEggData[index + 1]); // Сохраняем разблокировку
            _spawnedButtons[index + 1].SetDarkStyle(false);
            _spawnedButtons[index + 1].UpdateUI();
            Debug.Log($"Разблокировано следующее яйцо: {_allEggData[index + 1].EggName}");
        }
    }
}