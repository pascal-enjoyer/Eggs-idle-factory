using UnityEngine;
using System.Collections.Generic;

public class EggButtonsManager : MonoBehaviour
{
    [SerializeField] private List<EggData> _allEggData;
    [SerializeField] private EggButton _eggButtonPrefab;
    [SerializeField] private Transform _buttonsParent;

    private List<EggButton> _spawnedButtons = new List<EggButton>();

    private void Start()
    {
        InitializeEggs();
        CreateButtons();
    }

    private void InitializeEggs()
    {
        // Разблокируем только первое яйцо
        for (int i = 0; i < _allEggData.Count; i++)
        {
            _allEggData[i].IsUnlocked = (i == 0);
        }
    }

    private void CreateButtons()
    {
        for(int i = 0; i < _allEggData.Count;i++)
        {
            EggData eggData = _allEggData[i];
            if (i == 0) eggData.IsFirstInList = true;
            var button = Instantiate(_eggButtonPrefab, _buttonsParent);
            button.Initialize(eggData);
            
            _spawnedButtons.Add(button);
            button.UnlockNextEgg.AddListener(UnlockNextEgg);
            // Затемняем если не разблокировано
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
            _spawnedButtons[index + 1].SetDarkStyle(false);
            _spawnedButtons[index + 1].UpdateUI();
        }
    }
}