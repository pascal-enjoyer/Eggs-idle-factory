using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeMenu : MonoBehaviour
{
    [Header("Левая панель")]
    [SerializeField] private UpgradeButtonsManager buttonsManager;
    [SerializeField] private UpgradeConfigSO upgradeConfig;

    [Header("Правая панель")]
    [SerializeField] private Text descriptionText; // Предполагается, что это Text
    [SerializeField] private Text levelText;
    [SerializeField] private Text effectText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text buyCostText;

    [Header("Информация игрока")]
    [SerializeField] private Text playerLevelText;
    [SerializeField] private Text upgradePointsText;

    private UpgradeType selectedUpgrade;
    private bool isUpgradeSelected;

    private void Start()
    {
        if (buttonsManager == null || upgradeConfig == null)
        {
            Debug.LogError("UpgradeMenu: Не заданы buttonsManager или upgradeConfig!");
            return;
        }
        if (playerLevelText == null || upgradePointsText == null)
        {
            Debug.LogError("UpgradeMenu: Не заданы playerLevelText или upgradePointsText!");
            return;
        }

        buttonsManager.OnUpgradeSelected += OnUpgradeSelected;
        UpgradeSystem.OnUpgradeChanged += UpdateUI;
        PlayerEconomy.Instance.LevelChanged += UpdatePlayerInfo;
        PlayerEconomy.Instance.CoinsChanged += UpdatePlayerInfo;
        buyButton.onClick.AddListener(OnBuyButtonClick);

        UpdatePlayerInfo();
        UpdateRightPanel(null);
    }

    private void OnDestroy()
    {
        if (buttonsManager != null)
            buttonsManager.OnUpgradeSelected -= OnUpgradeSelected;
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.OnUpgradeChanged -= UpdateUI;
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
            descriptionText.text = "Choose upgrade";
            levelText.text = "";
            effectText.text = "";
            buyCostText.text = "";
            buyButton.interactable = false;
            return;
        }

        int currentLevel = UpgradeSystem.Instance.GetUpgradeLevel(upgradeInfo.Type);
        bool isUnlocked = UpgradeSystem.Instance.IsUpgradeUnlocked(upgradeInfo.Type);
        descriptionText.text = isUnlocked ? upgradeInfo.Description : $"{upgradeInfo.Description}\n(Unlocks at level {(int)upgradeInfo.Type + 1})";
        levelText.text = isUnlocked ? $"Level {currentLevel}/{upgradeInfo.MaxLevel}" : "Locked";
        effectText.text = isUnlocked ? $"{GetEffectText(upgradeInfo.Type, currentLevel)}" : "Locked";
        buyCostText.text = isUnlocked && currentLevel < upgradeInfo.MaxLevel ? $"Buy {upgradeInfo.CostPerLevel}" : isUnlocked ? "Max" : "Locked";
        buyButton.interactable = isUnlocked && UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeInfo.Type);
    }

    private string GetEffectText(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.ConveyorCount:
                return $"+{level} floors";
            case UpgradeType.EggIncome:
                return $"+{level * 10}% income";
            case UpgradeType.EggExperience:
                return $"+{level * 10}% exp";
            case UpgradeType.DoubleIncomeChance:
                return $"+{level * 3}% chance";
            case UpgradeType.DoubleExperienceChance:
                return $"+{level * 3}% chance";
            case UpgradeType.EggCostReduction:
                return $"-{level * 5}% cost";
            case UpgradeType.DoubleEggSpawnChance:
                return $"+{level * 3}% chance";
            case UpgradeType.DoubleEggPurchaseChance:
                return $"+{level * 3}% chance";
            case UpgradeType.DoubleFruitSpeedChance:
                return $"+{level * 3}% chance";
            default:
                return "Unknown effect";
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
        playerLevelText.text = $"Level {PlayerEconomy.Instance.GetLevel()}";
        upgradePointsText.text = $"Points {UpgradeSystem.Instance.GetUpgradePoints()}";
    }
}