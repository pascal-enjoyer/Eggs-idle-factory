using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class Egg : MonoBehaviour, IInitializableEgg, IMoveable, ICollectable
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2D;
    private EggData eggData;

    private int currentFloorIndex = -1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        gameObject.layer = LayerMask.NameToLayer("Egg");
    }

    public void Initialize(EggData eggData)
    {
        this.eggData = eggData;
        spriteRenderer.sprite = eggData.EggSprite;
    }

    public void Move(Vector2 direction, float speed)
    {
        rb2D.AddForce(direction.normalized * speed, ForceMode2D.Force);
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    public int GetCoinReward()
    {
        return eggData.CurrentIncome;
    }

    public void OnNewFloorEnter(int floorIndex)
    {
        // ѕровер€ем, что это новый этаж, а не тот же самый
        if (floorIndex > currentFloorIndex)
        {
            currentFloorIndex = floorIndex;

            gameObject.layer = LayerMask.NameToLayer("Egg");
            // ѕоловинки остаютс€ на слое EggHalf, слой не мен€ем
        }
    }

    public bool CanBeSplitOnFloor()
    {
        return gameObject.layer == LayerMask.NameToLayer("Egg");
    }

    public void Split()
    {
        if (!CanBeSplitOnFloor())
            return;

        gameObject.layer = LayerMask.NameToLayer("EggHalf");
        CreateHalf(Vector2.up * 0.2f, Quaternion.Euler(0, 0, 45));
        CreateHalf(Vector2.down * 0.2f, Quaternion.Euler(0, 0, -45));
        Destroy(gameObject);
    }

    private void CreateHalf(Vector2 offset, Quaternion rotation)
    {
        GameObject half = Instantiate(gameObject, transform.position + (Vector3)offset, rotation);

        half.transform.localScale = new Vector3(0.75f* half.transform.localScale.x, 0.75f* half.transform.localScale.y, 1f);


        CircleCollider2D halfCollider = half.GetComponent<CircleCollider2D>();
        halfCollider.radius *= 0.75f;


        Egg halfScript = half.GetComponent<Egg>();

        half.layer = LayerMask.NameToLayer("EggHalf");
        halfScript.Initialize(eggData);
        halfScript.SetFloorState(currentFloorIndex, true); // ѕоловинки не распиливаютс€ на этом этаже
    }

    public void SetFloorState(int floorIndex, bool wasSplitted)
    {
        currentFloorIndex = floorIndex;
        gameObject.layer = wasSplitted ? LayerMask.NameToLayer("EggHalf") : LayerMask.NameToLayer("Egg");
    }
}