using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class EggButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _eggIcon;
    [SerializeField] private Text _upgradeCostText;
    [SerializeField] private Text _incomeText;
    [SerializeField] private Text _headerText1; // Первый текстовый заголовок
    [SerializeField] private Text _headerText2; // Второй текстовый заголовок
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage; // Изображение кнопки

    

    [SerializeField] private EggData _eggData;
    private Color _originalIconColor;
    private Color _originalTextColor;
    private bool isSubscribed;
    private Dictionary<Image, Color> _originalImageColors = new Dictionary<Image, Color>();

    public UnityEvent<EggData> UnlockNextEgg;
    public static event Action<EggData> OnEggPurchased;

    private void Awake()
    {
        _originalIconColor = _eggIcon.color;
        _originalTextColor = _incomeText.color;
        _button.onClick.AddListener(OnButtonClick);

        // Сохраняем исходные цвета всех компонентов Image в кнопке
        Image[] images = GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            _originalImageColors[image] = image.color;
        }

        // Сохраняем цвет изображения кнопки, если оно задано
        if (_buttonImage != null)
        {
            _originalImageColors[_buttonImage] = _buttonImage.color;
        }
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
            // Устанавливаем черный цвет для всех Image
            foreach (var image in _originalImageColors.Keys)
            {
                image.color = Color.black;
            }
            // Устанавливаем черный цвет для всех текстовых элементов
            if (_incomeText != null) _incomeText.color = Color.black;
            if (_upgradeCostText != null) _upgradeCostText.color = Color.black;
            if (_headerText1 != null) _headerText1.color = Color.black;
            if (_headerText2 != null) _headerText2.color = Color.black;
            _button.interactable = false;
        }
        else
        {
            // Восстанавливаем исходные цвета для всех Image
            foreach (var pair in _originalImageColors)
            {
                pair.Key.color = pair.Value;
            }
            // Восстанавливаем исходные цвета для всех текстовых элементов
            if (_incomeText != null) _incomeText.color = _originalTextColor;
            if (_upgradeCostText != null) _upgradeCostText.color = _originalTextColor;
            if (_headerText1 != null) _headerText1.color = _originalTextColor;
            if (_headerText2 != null) _headerText2.color = _originalTextColor;
            _button.interactable = CanUpgrade();
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

        if (!_eggData.IsUnlocked)
        {
            _incomeText.text = "0";
            _upgradeCostText.text = _eggData.IsFirstInList ? "Free" : "Locked";
            _button.interactable = false;
        }
        else if (_eggData.UpgradeLevel == 0)
        {
            _incomeText.text = "0";
            _upgradeCostText.text = _eggData.IsFirstInList ? "Free" : Mathf.FloorToInt(cost).ToString();
            _button.interactable = CanUpgrade();
        }
        else
        {
            _incomeText.text = Mathf.FloorToInt(_eggData.CurrentIncome).ToString();
            _upgradeCostText.text = Mathf.FloorToInt(cost).ToString();
            _button.interactable = CanUpgrade();
        }
    }

    private bool CanUpgrade()
    {
        if (!_eggData.IsUnlocked) return false;
        float cost = _eggData.CurrentUpgradeCost;
        float reduction = GameModifiers.Instance != null ? GameModifiers.Instance.GetEggCostReduction() : 1f;
        cost *= reduction;
        return _eggData.IsFirstInList || PlayerEconomy.Instance.HaveEnoughCoinsToBuy(cost);
    }

    private void SaveEggData()
    {
        PlayerPrefs.SetInt($"Egg_{_eggData.EggName}_IsUnlocked", _eggData.IsUnlocked ? 1 : 0);
        PlayerPrefs.SetInt($"Egg_{_eggData.EggName}_UpgradeLevel", _eggData.UpgradeLevel);
        PlayerPrefs.Save();
    }
}