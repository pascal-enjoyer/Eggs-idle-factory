using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class EggHalf : MonoBehaviour, ICollectable, IMoveable, IInitializableEgg
{
    private EggData eggData;
    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        gameObject.layer = LayerMask.NameToLayer("EggHalf");
    }

    public void Initialize(EggData eggData)
    {
        this.eggData = eggData;
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    public float GetCoinReward()
    {
        return eggData.CurrentIncome;
    }

    public void Move(Vector2 direction, float speed)
    {
        rigidbody2D.AddForce(direction.normalized * speed, ForceMode2D.Force);
    }
}