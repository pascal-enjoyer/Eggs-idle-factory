using UnityEngine;
using System.Collections.Generic;
using System;

public class UpgradeButtonsManager : MonoBehaviour
{
    [SerializeField] private UpgradeConfigSO _upgradeConfig;
    [SerializeField] private UpgradeButton _upgradeButtonPrefab;
    [SerializeField] private Transform _buttonsParent;

    private List<UpgradeButton> _spawnedButtons = new List<UpgradeButton>();
    public event Action<UpgradeType> OnUpgradeSelected;

    private void Start()
    {
        CreateButtons();
    }

    private void CreateButtons()
    {
        if (_upgradeConfig == null || _upgradeButtonPrefab == null || _buttonsParent == null)
        {
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