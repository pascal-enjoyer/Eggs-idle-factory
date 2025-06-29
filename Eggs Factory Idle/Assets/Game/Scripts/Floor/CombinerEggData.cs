using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class CombinedEggData : MonoBehaviour, ICollectable, IMoveable, IInitializableEgg
{
    private float coinReward;
    private float experienceReward = 1f;
    private SpriteRenderer spriteRenderer;

    private EggData eggData;
    private Rigidbody2D rb2D;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.layer = LayerMask.NameToLayer("Egg");
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.gravityScale = 1f;
    }

    public void Initialize(float coins, Sprite sprite)
    {
        coinReward = coins;
        spriteRenderer.sprite = sprite;
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    public float GetCoinReward()
    {
        return coinReward;
    }

    public void Move(Vector2 direction, float speed)
    {
        rb2D.AddForce(direction.normalized * speed, ForceMode2D.Force);
    }

    public void Initialize(EggData eggData)
    {
        this.eggData = eggData;
        spriteRenderer.sprite = eggData.EggSprite;
    }
}