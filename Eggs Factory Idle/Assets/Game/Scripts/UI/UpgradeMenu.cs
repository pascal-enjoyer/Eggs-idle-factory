using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeMenu : MonoBehaviour
{
    [Header("Левая панель")]
    [SerializeField] private UpgradeButtonsManager buttonsManager; // Ссылка на UpgradeButtonsManager
    [SerializeField] private UpgradeConfigSO upgradeConfig; // Конфигурация апгрейдов

    [Header("Правая панель")]
    [SerializeField] private Text descriptionText; // Текст описания
    [SerializeField] private Text levelText; // Текст уровня апгрейда
    [SerializeField] private Text effectText; // Текст суммарного эффекта
    [SerializeField] private Button buyButton; // Кнопка покупки
    [SerializeField] private Text buyCostText; // Текст стоимости покупки

    [Header("Информация игрока")]
    [SerializeField] private Text playerLevelText; // Текст уровня игрока
    [SerializeField] private Text upgradePointsText; // Текст очков апгрейда

    private UpgradeType selectedUpgrade; // Текущий выбранный апгрейд
    private bool isUpgradeSelected; // Флаг выбора апгрейда

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
        PlayerEconomy.Instance.CoinsChanged += UpdatePlayerInfo; // На случай, если очки зависят от монет
        buyButton.onClick.AddListener(OnBuyButtonClick);

        // Инициализация UI
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
        descriptionText.text = upgradeInfo.Description;
        levelText.text = $"Level {currentLevel}/{upgradeInfo.MaxLevel}";
        effectText.text = $"{GetEffectText(upgradeInfo.Type, currentLevel)}";
        buyCostText.text = currentLevel < upgradeInfo.MaxLevel ? $"Buy {upgradeInfo.CostPerLevel}" : "Max";
        buyButton.interactable = UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeInfo.Type);
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