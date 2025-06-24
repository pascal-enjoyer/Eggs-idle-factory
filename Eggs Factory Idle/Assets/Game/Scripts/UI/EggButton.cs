using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class EggButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _eggIcon;
    [SerializeField] private Text _upgradeCostText;
    [SerializeField] private Text _incomeText;
    [SerializeField] private Button _button;

    private EggData _eggData;
    private Color _originalIconColor;
    private Color _originalTextColor;

    public UnityEvent<EggData> UnlockNextEgg;

    private void Awake()
    {
        _originalIconColor = _eggIcon.color;
        _originalTextColor = _incomeText.color;
        _button.onClick.AddListener(OnButtonClick);
    }

    private void Start()
    {
        PlayerEconomy.Instance.CoinsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        PlayerEconomy.Instance.CoinsChanged -= UpdateUI;
    }

    public void Initialize(EggData eggData)
    {
        _eggData = eggData;
        _eggIcon.sprite = _eggData.EggSprite;
        UpdateUI();

        // Инициализируем progress bar
        if (_eggIcon != null)
        {
            _eggIcon.fillAmount = 0f;
        }
    }

    public void SetDarkStyle(bool dark)
    {
        if (dark)
        {
            _eggIcon.color = Color.black;
            _incomeText.color = Color.gray;
            _upgradeCostText.color = Color.gray;
            _button.interactable = false;
        }
        else
        {
            _eggIcon.color = _originalIconColor;
            _incomeText.color = _originalTextColor;
            _upgradeCostText.color = _originalTextColor;
            _button.interactable = true;
        }
    }

    private void Update()
    {
        if (_eggData != null && _eggData.IsUnlocked && _eggIcon != null)
        {
            _eggIcon.fillAmount = EggSpawnSystem.Instance.GetEggProgress(_eggData);
        }
    }


    private void OnButtonClick()
    {
        if (CanUpgrade())
        {
            PlayerEconomy.Instance.AddCoins(-_eggData.CurrentUpgradeCost);
            _eggData.UpgradeLevel++;

            if (_eggData.UpgradeLevel == 1)
            {
                EggSpawnSystem.Instance.AddEgg(_eggData);
                UnlockNextEgg?.Invoke(_eggData);
            }

            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        // Для неразблокированных или некупленных
        if (!_eggData.IsUnlocked || _eggData.UpgradeLevel == 0)
        {
            _incomeText.text = "0";
            _upgradeCostText.text = _eggData.IsFirstInList ? "Free" : _eggData.BaseUpgradeCost.ToString();
        }
        else
        {
            _incomeText.text = _eggData.CurrentIncome.ToString();
            _upgradeCostText.text = _eggData.CurrentUpgradeCost.ToString();
        }

        // Обновляем состояние кнопки
        _button.interactable = CanUpgrade();
    }

    private bool CanUpgrade()
    {
        if (!_eggData.IsUnlocked) return false;
        return 
               PlayerEconomy.Instance.HaveEnoughCoinsToBuy(_eggData.CurrentUpgradeCost);
    }
}