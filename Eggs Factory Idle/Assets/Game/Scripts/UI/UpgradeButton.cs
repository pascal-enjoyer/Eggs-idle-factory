using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text levelText; // Предполагается, что это Text, а не TextMeshProUGUI
    [SerializeField] private Text costText;
    [SerializeField] private Button button;

    private UpgradeType upgradeType;
    private Action<UpgradeType> onSelectCallback;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
    }

    public void Initialize(UpgradeType type, Action<UpgradeType> onSelect)
    {
        upgradeType = type;
        onSelectCallback = onSelect;

        if (UpgradeSystem.Instance == null)
        {
            Debug.LogError("UpgradeSystem instance is null!");
            return;
        }

        var config = UpgradeSystem.Instance.GetUpgradeConfig(type);
        if (config == null)
        {
            Debug.LogError($"Config for {type} is null!");
            return;
        }

        if (iconImage != null) iconImage.sprite = config.Icon;
        button.onClick.AddListener(OnButtonClick);
        UpdateUI();
    }

    private void OnEnable()
    {
        UpgradeSystem.OnUpgradeChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        UpgradeSystem.OnUpgradeChanged -= UpdateUI;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
        UpgradeSystem.OnUpgradeChanged -= UpdateUI;
    }

    private void OnButtonClick()
    {
        onSelectCallback?.Invoke(upgradeType);
    }

    public void UpdateUI()
    {
        if (UpgradeSystem.Instance == null) return;

        var level = UpgradeSystem.Instance.GetUpgradeLevel(upgradeType);
        var config = UpgradeSystem.Instance.GetUpgradeConfig(upgradeType);
        bool isUnlocked = UpgradeSystem.Instance.IsUpgradeUnlocked(upgradeType);
        bool canPurchase = UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeType);

        if (levelText != null)
            levelText.text = isUnlocked ? $"Lv.{level}" : "Locked";

        if (costText != null)
            costText.text = isUnlocked && level < config.MaxLevel ? $"{config.CostPerLevel}" : isUnlocked ? "Max" : "Locked";

        if (iconImage != null)
        {
            iconImage.color = isUnlocked ? Color.white : Color.black;
            iconImage.sprite = config.Icon; // Убедимся, что иконка всегда правильная
        }

        if (levelText != null)
            levelText.color = isUnlocked ? Color.white : Color.black;

        if (costText != null)
            costText.color = isUnlocked ? Color.white : Color.black;

        // Кнопка всегда кликабельна, чтобы показывать описание
        if (button != null)
            button.interactable = true; // Всегда кликабельна
    }
}