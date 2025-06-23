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
        gameObject.layer = LayerMask.NameToLayer("Conveyor");
        Debug.Log($"Conveyor {gameObject.name} initialized, colliderBounds={boxCollider.bounds}");
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
        // Управление активацией через ConveyorManager
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
            Debug.Log($"Conveyor {gameObject.name} collision with {collision.gameObject.name}");
            MoveObject(rb);
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = gameObject.name.Contains("Shortened") ? Color.red : Color.green;
            Vector3 center = transform.TransformPoint(boxCollider.offset);
            Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, 1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}