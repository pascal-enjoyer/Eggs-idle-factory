using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeMenu : MonoBehaviour
{
    [Header("����� ������")]
    [SerializeField] private UpgradeButtonsManager buttonsManager; // ������ �� UpgradeButtonsManager
    [SerializeField] private UpgradeConfigSO upgradeConfig; // ������������ ���������

    [Header("������ ������")]
    [SerializeField] private Text descriptionText; // ����� ��������
    [SerializeField] private Text levelText; // ����� ������ ��������
    [SerializeField] private Text effectText; // ����� ���������� �������
    [SerializeField] private Button buyButton; // ������ �������
    [SerializeField] private Text buyCostText; // ����� ��������� �������

    [Header("���������� ������")]
    [SerializeField] private Text playerLevelText; // ����� ������ ������
    [SerializeField] private Text upgradePointsText; // ����� ����� ��������

    private UpgradeType selectedUpgrade; // ������� ��������� �������
    private bool isUpgradeSelected; // ���� ������ ��������

    private void Start()
    {
        if (buttonsManager == null || upgradeConfig == null)
        {
            Debug.LogError("UpgradeMenu: �� ������ buttonsManager ��� upgradeConfig!");
            return;
        }
        if (playerLevelText == null || upgradePointsText == null)
        {
            Debug.LogError("UpgradeMenu: �� ������ playerLevelText ��� upgradePointsText!");
            return;
        }

        buttonsManager.OnUpgradeSelected += OnUpgradeSelected;
        UpgradeSystem.Instance.OnUpgradeChanged += UpdateUI;
        PlayerEconomy.Instance.LevelChanged += UpdatePlayerInfo;
        PlayerEconomy.Instance.CoinsChanged += UpdatePlayerInfo; // �� ������, ���� ���� ������� �� �����
        buyButton.onClick.AddListener(OnBuyButtonClick);

        // ������������� UI
        UpdatePlayerInfo();
        UpdateRightPanel(null);
    }

    private void OnDestroy()
    {
        if (buttonsManager != null)
            buttonsManager.OnUpgradeSelected -= OnUpgradeSelected;
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.OnUpgradeChanged -= UpdateUI;
        if (PlayerEconomy.Instance != null)
        {
            PlayerEconomy.Instance.LevelChanged -= UpdatePlayerInfo;
            PlayerEconomy.Instance.CoinsChanged -= UpdatePlayerInfo;
        }
        buyButton.onClick.RemoveListener(OnBuyButtonClick);
    }

    private void OnUpgradeSelected(UpgradeType type)
    {
        selectedUpgrade = type;
        isUpgradeSelected = true;
        UpdateRightPanel(upgradeConfig.Upgrades.Find(u => u.Type == type));
    }

    private void UpdateRightPanel(UpgradeConfigSO.UpgradeInfo upgradeInfo)
    {
        if (upgradeInfo == null || !isUpgradeSelected)
        {
            descriptionText.text = "�������� �������";
            levelText.text = "";
            effectText.text = "";
            buyCostText.text = "";
            buyButton.interactable = false;
            return;
        }

        int currentLevel = UpgradeSystem.Instance.GetUpgradeLevel(upgradeInfo.Type);
        descriptionText.text = upgradeInfo.Description;
        levelText.text = $"�������: {currentLevel}/{upgradeInfo.MaxLevel}";
        effectText.text = $"������: {GetEffectText(upgradeInfo.Type, currentLevel)}";
        buyCostText.text = currentLevel < upgradeInfo.MaxLevel ? $"������: {upgradeInfo.CostPerLevel} �����" : "����. �������";
        buyButton.interactable = UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeInfo.Type);
    }

    private string GetEffectText(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.ConveyorCount:
                return $"+{level} ������";
            case UpgradeType.EggIncome:
                return $"+{level * 10}% ������";
            case UpgradeType.EggExperience:
                return $"+{level * 10}% �����";
            case UpgradeType.DoubleIncomeChance:
                return $"+{level * 3}% �����";
            case UpgradeType.DoubleExperienceChance:
                return $"+{level * 3}% �����";
            case UpgradeType.EggCostReduction:
                return $"-{level * 5}% ���������";
            case UpgradeType.DoubleEggSpawnChance:
                return $"+{level * 3}% �����";
            case UpgradeType.DoubleEggPurchaseChance:
                return $"+{level * 3}% �����";
            case UpgradeType.DoubleFruitSpeedChance:
                return $"+{level * 3}% �����";
            default:
                return "����������� ������";
        }
    }

    private void OnBuyButtonClick()
    {
        if (isUpgradeSelected && UpgradeSystem.Instance.CanPurchaseUpgrade(selectedUpgrade))
        {
            UpgradeSystem.Instance.PurchaseUpgrade(selectedUpgrade);
            UpdateRightPanel(upgradeConfig.Upgrades.Find(u => u.Type == selectedUpgrade));
        }
    }

    private void UpdateUI()
    {
        if (isUpgradeSelected)
        {
            UpdateRightPanel(upgradeConfig.Upgrades.Find(u => u.Type == selectedUpgrade));
        }
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        playerLevelText.text = $"�������: {PlayerEconomy.Instance.GetLevel()}";
        upgradePointsText.text = $"����: {UpgradeSystem.Instance.GetUpgradePoints()}";
    }
}