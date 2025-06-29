using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Conveyor : MonoBehaviour, IConveyor
{
    [SerializeField] private float conveyorSpeed = 10f;
    [SerializeField] private Vector2 moveDirection = Vector2.right;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = false;

    }

    public void MoveObject(Rigidbody2D rb)
    {
        IMoveable moveable = rb.GetComponent<IMoveable>();
        if (moveable != null)
        {
            moveable.Move(moveDirection, conveyorSpeed);
        }
    }

    public void MirrorDirection()
    {
        moveDirection = new Vector2(-moveDirection.x, moveDirection.y);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            MoveObject(rb);
        }
    }

    

}