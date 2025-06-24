using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;
    private UpgradeType upgradeType;

    public void Initialize(UpgradeType type)
    {
        upgradeType = type;
        UpdateUI();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
    }


    private void OnEnable()
    {
        //UpgradeSystem.Instance.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        //UpgradeSystem.Instance.RemoveListener(UpdateUI);
    }

    private void OnUpgradeButtonClick()
    {
        if (UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeType))
        {
            UpgradeSystem.Instance.PurchaseUpgrade(upgradeType);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        var level = UpgradeSystem.Instance.GetUpgradeLevel(upgradeType);
        //var config = UpgradeSystem.Instance.GetUpgradeConfig(upgradeType);
        upgradeNameText.text = upgradeType.ToString();
        //levelText.text = $"Level: {level}/{config.MaxLevel}";
        //costText.text = $"Cost: {config.CostPerLevel} points";
        upgradeButton.interactable = UpgradeSystem.Instance.CanPurchaseUpgrade(upgradeType);
    }
}