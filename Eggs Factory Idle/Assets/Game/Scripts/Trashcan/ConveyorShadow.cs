using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorShadow : MonoBehaviour, IConveyor
{
    private float conveyorSpeed = 10f;
    private Vector2 moveDirection = Vector2.right;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("ConveyorShadow");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
    }

    public void SetConveyorSpeed(float speed, Vector2 direction)
    {
        conveyorSpeed = speed;
        moveDirection = direction;
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
            Debug.Log($"ConveyorShadow collision with {collision.gameObject.name}");
            MoveObject(rb);
        }
    }
}