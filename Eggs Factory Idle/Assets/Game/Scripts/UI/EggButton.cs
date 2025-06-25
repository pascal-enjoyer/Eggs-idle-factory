using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

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
    private bool isSubscribed;

    public UnityEvent<EggData> UnlockNextEgg;
    public static event Action<EggData> OnEggPurchased;

    private void Awake()
    {
        _originalIconColor = _eggIcon.color;
        _originalTextColor = _incomeText.color;
        _button.onClick.AddListener(OnButtonClick);
    }

    private void Start()
    {
        if (PlayerEconomy.Instance != null)
        {
            PlayerEconomy.Instance.CoinsChanged += UpdateUI;
            isSubscribed = true;
            UpdateUI();
            Debug.Log($"EggButton: Подписка на PlayerEconomy для {_eggData?.EggName} выполнена");
        }
        else
        {
            Debug.LogError("EggButton: PlayerEconomy.Instance не инициализирован!");
        }
    }

    private void OnDisable()
    {
        if (PlayerEconomy.Instance != null && isSubscribed)
        {
            PlayerEconomy.Instance.CoinsChanged -= UpdateUI;
        }
    }

    public void Initialize(EggData eggData)
    {
        _eggData = eggData;
        _eggIcon.sprite = _eggData.EggSprite;
        UpdateUI();

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
            float cost = _eggData.CurrentUpgradeCost;
            float reduction = GameModifiers.Instance != null ? GameModifiers.Instance.GetEggCostReduction() : 1f;
            cost *= reduction;

            PlayerEconomy.Instance.SpendCoins(cost);
            _eggData.UpgradeLevel++;
            SaveEggData();
            OnEggPurchased?.Invoke(_eggData);

            if (_eggData.UpgradeLevel == 1)
            {
                EggSpawnSystem.Instance.AddEgg(_eggData);
                UnlockNextEgg?.Invoke(_eggData);
            }

            float doublePurchaseChance = GameModifiers.Instance != null ? GameModifiers.Instance.GetDoubleEggPurchaseChance() : 0f;
            if (UnityEngine.Random.value < doublePurchaseChance)
            {
                _eggData.UpgradeLevel++;
                SaveEggData();
                OnEggPurchased?.Invoke(_eggData);
                if (_eggData.UpgradeLevel == 1)
                {
                    EggSpawnSystem.Instance.AddEgg(_eggData);
                    UnlockNextEgg?.Invoke(_eggData);
                }
                Debug.Log($"DoubleEggPurchaseChance сработал, куплено второе яйцо бесплатно для {_eggData.EggName}");
            }

            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        float cost = _eggData.CurrentUpgradeCost;
        float reduction = GameModifiers.Instance != null ? GameModifiers.Instance.GetEggCostReduction() : 1f;
        cost *= reduction;

        if (!_eggData.IsUnlocked || _eggData.UpgradeLevel == 0)
        {
            _incomeText.text = "0";
            _upgradeCostText.text = _eggData.IsFirstInList ? "Free" : Mathf.FloorToInt(cost).ToString();
        }
        else
        {
            _incomeText.text = Mathf.FloorToInt(_eggData.CurrentIncome).ToString();
            _upgradeCostText.text = Mathf.FloorToInt(cost).ToString();
        }

        _button.interactable = CanUpgrade();
    }

    private bool CanUpgrade()
    {
        if (!_eggData.IsUnlocked) return false;
        float cost = _eggData.CurrentUpgradeCost;
        float reduction = GameModifiers.Instance != null ? GameModifiers.Instance.GetEggCostReduction() : 1f;
        cost *= reduction;
        return PlayerEconomy.Instance.HaveEnoughCoinsToBuy(cost);
    }

    private void SaveEggData()
    {
        PlayerPrefs.SetInt($"Egg_{_eggData.EggName}_IsUnlocked", _eggData.IsUnlocked ? 1 : 0);
        PlayerPrefs.SetInt($"Egg_{_eggData.EggName}_UpgradeLevel", _eggData.UpgradeLevel);
        PlayerPrefs.Save();
    }
}