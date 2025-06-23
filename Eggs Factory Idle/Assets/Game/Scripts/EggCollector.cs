using UnityEngine;

public class EggCollector : MonoBehaviour, ICollector
{
    public void ProcessCollectible(ICollectable collectible)
    {
        if (collectible == null)
            return;

        int coins = collectible.GetCoinReward();
        PlayerEconomy.Instance.AddCoins(coins);
        PlayerEconomy.Instance.AddExperience(1);
        collectible.Collect();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ICollectable collectible = other.GetComponent<ICollectable>();
        if (collectible != null)
        {
            ProcessCollectible(collectible);
        }
    }
}