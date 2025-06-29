using UnityEngine;
using System;

public class EggCollector : MonoBehaviour, ICollector
{
    [SerializeField] private ConveyorFloor conveyorFloor;
    [SerializeField] private GameObject coinTextPrefab;
    [SerializeField] private float bufferInterval = 0.2f;

    private float coinBuffer = 0f;
    private float bufferTimer = 0f;
    public static event Action OnEggCollected;

    private void Update()
    {
        if (coinBuffer > 0f)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                SpawnCoinText();
                bufferTimer = bufferInterval;
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
        }

        coins *= GameModifiers.Instance != null ? GameModifiers.Instance.GetEggIncomeMultiplier() : 1f;

        float doubleExperienceChance = GameModifiers.Instance != null ? GameModifiers.Instance.GetDoubleExperienceChance() : 0f;
        if (UnityEngine.Random.value < doubleExperienceChance)
        {
            experience *= 2f;
        }

        experience *= GameModifiers.Instance != null ? GameModifiers.Instance.GetEggExperienceMultiplier() : 1f;

        PlayerEconomy.Instance.AddCoins(coins);
        PlayerEconomy.Instance.AddExperience(experience);
        collectible.Collect();
        OnEggCollected?.Invoke();

        coinBuffer += coins;
        
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
            return;
        }

        Canvas canvas = UICanvasRef.Instance?.Canvas;
        if (canvas == null)
        {
            return;
        }

        GameObject textInstance = Instantiate(coinTextPrefab);
        CoinTextEffect coinText = textInstance.GetComponent<CoinTextEffect>();
        if (coinText != null)
        {
            coinText.Initialize(coinBuffer, transform.position, canvas);
            coinBuffer = 0f;
        }
        else
        {
            Destroy(textInstance);
        }
    }
}