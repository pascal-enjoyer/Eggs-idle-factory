using UnityEngine;
using System;

public class EggCollector : MonoBehaviour, ICollector
{
    [SerializeField] private ConveyorFloor conveyorFloor;
    [SerializeField] private GameObject coinTextPrefab; // Префаб текста монет
    [SerializeField] private float bufferInterval = 0.2f; // Фиксированный интервал спавна текста (0.2с)

    private float coinBuffer = 0f; // Буфер для монет
    private float bufferTimer = 0f; // Таймер буфера
    public static event Action OnEggCollected;

    private void Update()
    {
        if (coinBuffer > 0f)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                SpawnCoinText();
                bufferTimer = bufferInterval; // Устанавливаем фиксированный интервал
            }
        }
    }

    public void ProcessCollectible(ICollectable collectible)
    {
        if (collectible == null)
            return;

        float coins = collectible.GetCoinReward();
        float experience = 1f;

        float doubleIncomeChance = GameModifiers.Instance != null ? GameModifiers.Instance.GetDoubleIncomeChance() : 0f;
        if (UnityEngine.Random.value < doubleIncomeChance)
        {
            coins *= 2f;
            Debug.Log($"DoubleIncomeChance сработал, доход удвоен: {coins}");
        }

        coins *= GameModifiers.Instance != null ? GameModifiers.Instance.GetEggIncomeMultiplier() : 1f;

        float doubleExperienceChance = GameModifiers.Instance != null ? GameModifiers.Instance.GetDoubleExperienceChance() : 0f;
        if (UnityEngine.Random.value < doubleExperienceChance)
        {
            experience *= 2f;
            Debug.Log($"DoubleExperienceChance сработал, опыт удвоен: {experience}");
        }

        experience *= GameModifiers.Instance != null ? GameModifiers.Instance.GetEggExperienceMultiplier() : 1f;

        PlayerEconomy.Instance.AddCoins(coins);
        PlayerEconomy.Instance.AddExperience(experience);
        collectible.Collect();
        OnEggCollected?.Invoke();

        // Добавляем монеты в буфер без сброса таймера
        coinBuffer += coins;
        Debug.Log($"EggCollector: Добавлено {coins} монет в буфер, итого: {coinBuffer}, Позиция коллектора: {transform.position}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EggHalf") && conveyorFloor != null && !conveyorFloor.IsLastConveyor)
        {
            return;
        }

        ICollectable collectible = other.GetComponent<ICollectable>();
        if (collectible != null)
        {
            ProcessCollectible(collectible);
        }
    }

    private void SpawnCoinText()
    {
        if (coinTextPrefab == null)
        {
            Debug.LogWarning("EggCollector: coinTextPrefab не назначен!");
            return;
        }

        Canvas canvas = UICanvasRef.Instance?.Canvas;
        if (canvas == null)
        {
            Debug.LogWarning("EggCollector: UICanvasRef.Instance или Canvas не найдены!");
            return;
        }

        GameObject textInstance = Instantiate(coinTextPrefab);
        CoinTextEffect coinText = textInstance.GetComponent<CoinTextEffect>();
        if (coinText != null)
        {
            coinText.Initialize(coinBuffer, transform.position, canvas);
            Debug.Log($"EggCollector: Спавн текста: +{Mathf.FloorToInt(coinBuffer)} монет");
            coinBuffer = 0f; // Сбрасываем буфер после спавна
        }
        else
        {
            Debug.LogWarning("EggCollector: coinTextPrefab не содержит CoinText!");
            Destroy(textInstance);
        }
    }
}