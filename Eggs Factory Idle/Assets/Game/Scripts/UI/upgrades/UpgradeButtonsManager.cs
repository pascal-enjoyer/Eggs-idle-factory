using UnityEngine;
using System.Collections.Generic;
using System;

public class UpgradeButtonsManager : MonoBehaviour
{
    [SerializeField] private UpgradeConfigSO _upgradeConfig; // Конфигурация апгрейдов
    [SerializeField] private UpgradeButton _upgradeButtonPrefab; // Префаб кнопки апгрейда
    [SerializeField] private Transform _buttonsParent; // Родительский объект для кнопок

    private List<UpgradeButton> _spawnedButtons = new List<UpgradeButton>();
    public event Action<UpgradeType> OnUpgradeSelected; // Событие выбора апгрейда

    private void Start()
    {
        CreateButtons();
    }

    private void CreateButtons()
    {
        if (_upgradeConfig == null || _upgradeButtonPrefab == null || _buttonsParent == null)
        {
            Debug.LogError("UpgradeButtonsManager: Не заданы _upgradeConfig, _upgradeButtonPrefab или _buttonsParent!");
            return;
        }

        foreach (var upgrade in _upgradeConfig.Upgrades)
        {
            var button = Instantiate(_upgradeButtonPrefab, _buttonsParent);
            button.Initialize(upgrade.Type, HandleUpgradeSelected);
            _spawnedButtons.Add(button);
        }
    }

    private void HandleUpgradeSelected(UpgradeType type)
    {
        OnUpgradeSelected?.Invoke(type);
    }
}