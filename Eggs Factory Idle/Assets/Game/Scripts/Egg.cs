using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class Egg : MonoBehaviour, IInitializableEgg, IMoveable, ICollectable
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private EggData eggData;
    private bool isSplit;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        gameObject.layer = LayerMask.NameToLayer("Egg");
    }

    public void Initialize(EggData eggData)
    {
        this.eggData = eggData;
        spriteRenderer.sprite = eggData.EggSprite;
    }

    public void Move(Vector2 direction, float speed)
    {
        rigidbody2D.AddForce(direction.normalized * speed, ForceMode2D.Force);
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    public int GetCoinReward()
    {
        return eggData.Income;
    }

    public bool CanBeSplit()
    {
        return !isSplit;
    }

    public void Split()
    {
        if (isSplit) return;
        isSplit = true;

        CreateHalf(Vector2.up * 0.2f, Quaternion.Euler(0, 0, 45));
        CreateHalf(Vector2.down * 0.2f, Quaternion.Euler(0, 0, -45));
        Destroy(gameObject);
    }

    private void CreateHalf(Vector2 offset, Quaternion rotation)
    {
        GameObject half = new GameObject($"{eggData.EggName}_Half");
        half.transform.position = transform.position + (Vector3)offset;
        half.transform.rotation = rotation;

        SpriteRenderer halfRenderer = half.AddComponent<SpriteRenderer>();
        halfRenderer.sprite = eggData.EggSprite;
        halfRenderer.sortingOrder = spriteRenderer.sortingOrder;
        half.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Rigidbody2D halfRb = half.AddComponent<Rigidbody2D>();
        halfRb.gravityScale = rigidbody2D.gravityScale;
        halfRb.constraints = rigidbody2D.constraints;

        CircleCollider2D halfCollider = half.AddComponent<CircleCollider2D>();
        halfCollider.radius = 0.25f;

        half.layer = LayerMask.NameToLayer("EggHalf");

        EggHalf halfScript = half.AddComponent<EggHalf>();
        halfScript.Initialize(eggData);
    }
}