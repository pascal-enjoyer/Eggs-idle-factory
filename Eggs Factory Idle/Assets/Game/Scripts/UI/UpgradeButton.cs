using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage; // ������ ��������
    [SerializeField] private Text levelText; // ����� ������
    [SerializeField] private Text costText; // ����� ���������
    [SerializeField] private Button button; // ������ ��������

    private UpgradeType upgradeType;
    private Action<UpgradeType> onSelectCallback;

    public void Initialize(UpgradeType type, Action<UpgradeType> onSelect)
    {
        upgradeType = type;
        onSelectCallback = onSelect;
        var config = UpgradeSystem.Instance.GetUpgradeConfig(type);
        iconImage.sprite = config.Icon;
        button.onClick.AddListener(OnButtonClick);
        UpdateUI();
        UpgradeSystem.Instance.AddListener(UpdateUI);
    }

    private void OnEnable()
    {
        UpgradeSystem.Instance.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        UpgradeSystem.Instance.RemoveListener(UpdateUI);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        onSelectCallback?.Invoke(upgradeType);
    }

    private void UpdateUI()
    {
        var level = UpgradeSystem.Instance.GetUpgradeLevel(upgradeType);
        var config = UpgradeSystem.Instance.GetUpgradeConfig(upgradeType);
        levelText.text = $"Lv.{level}";
        costText.text = level < config.MaxLevel ? $"{config.CostPerLevel}" : "Max";
        button.interactable = UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeType);
    }
}