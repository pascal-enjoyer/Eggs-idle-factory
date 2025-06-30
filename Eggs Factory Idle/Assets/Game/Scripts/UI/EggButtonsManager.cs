using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class EggButtonsManager : MonoBehaviour
{
    [SerializeField] private List<EggData> _allEggData;
    [SerializeField] private EggButton _eggButtonPrefab;
    [SerializeField] private Transform _buttonsParent;
    public UnityEvent EggButtonsInitialized;
    private List<EggButton> _spawnedButtons = new List<EggButton>();

    private void Start()
    {
        LoadEggsData();
        InitializeEggs();
        InitializeEggTimers();
        CreateButtons();
    }

    private void LoadEggsData()
    {
        for (int i = 0; i < _allEggData.Count; i++)
        {
            var egg = _allEggData[i];
            egg.IsUnlocked = (i == 0) || PlayerPrefs.GetInt($"Egg_{egg.EggName}_IsUnlocked", 0) == 1;
            egg.UpgradeLevel = PlayerPrefs.GetInt($"Egg_{egg.EggName}_UpgradeLevel", 0);
        }
    }

    private void SaveEggData(EggData eggData)
    {
        PlayerPrefs.SetInt($"Egg_{eggData.EggName}_IsUnlocked", eggData.IsUnlocked ? 1 : 0);
        PlayerPrefs.SetInt($"Egg_{eggData.EggName}_UpgradeLevel", eggData.UpgradeLevel);
        PlayerPrefs.Save();
    }

    private void InitializeEggs()
    {
        for (int i = 0; i < _allEggData.Count; i++)
        {
            _allEggData[i].IsFirstInList = (i == 0);

        }
        
    }

    private void InitializeEggTimers()
    {
        foreach (var eggData in _allEggData)
        {
            if (eggData.IsUnlocked && eggData.UpgradeLevel > 0)
            {
                EggSpawnSystem.Instance.AddEgg(eggData);
            }
        }
    }

    private void CreateButtons()
    {
        if (_eggButtonPrefab == null || _buttonsParent == null)
        {
            return;
        }

        for (int i = 0; i < _allEggData.Count; i++)
        {
            EggData eggData = _allEggData[i];
            var button = Instantiate(_eggButtonPrefab, _buttonsParent);
            button.Initialize(eggData);
            _spawnedButtons.Add(button);
            button.UnlockNextEgg.AddListener(UnlockNextEgg);
            if (!eggData.IsUnlocked)
            {
                button.SetDarkStyle(true);
            }
        }
        EggButtonsInitialized?.Invoke();
    }

    public void UnlockNextEgg(EggData currentEgg)
    {
        int index = _allEggData.IndexOf(currentEgg);
        if (index >= 0 && index < _allEggData.Count - 1)
        {
            _allEggData[index + 1].IsUnlocked = true;
            SaveEggData(_allEggData[index + 1]);
            _spawnedButtons[index + 1].SetDarkStyle(false);
            _spawnedButtons[index + 1].UpdateUI();
        }
    }
}