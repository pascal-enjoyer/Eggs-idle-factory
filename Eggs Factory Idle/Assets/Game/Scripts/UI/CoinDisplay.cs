using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CoinDisplay : MonoBehaviour
{
    private Text coinsText;
    private bool isSubscribed;

    private void Awake()
    {
        coinsText = GetComponent<Text>();
    }

    private void Start()
    {
        TrySubscribe();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (PlayerEconomy.Instance != null && isSubscribed)
        {
            PlayerEconomy.Instance.CoinsChanged -= UpdateCoinDisplay;
            isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (!isSubscribed && PlayerEconomy.Instance != null)
        {
            PlayerEconomy.Instance.CoinsChanged += UpdateCoinDisplay;
            isSubscribed = true;
            UpdateCoinDisplay();
        }
    }

    private void UpdateCoinDisplay()
    {
        if (PlayerEconomy.Instance != null)
        {
            coinsText.text = PlayerEconomy.Instance.GetCoins().ToString();
        }
    }
}