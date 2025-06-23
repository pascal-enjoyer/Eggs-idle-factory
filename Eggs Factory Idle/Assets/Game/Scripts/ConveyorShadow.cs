using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorShadow : MonoBehaviour, IConveyor
{
    private float conveyorSpeed;
    private Vector2 moveDirection;

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

    public void SetLastConveyor(bool isLast)
    {
        gameObject.SetActive(!isLast);
    }

    public Transform GetShortenPoint()
    {
        return null;
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