using UnityEditor.SceneManagement;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorFloor : MonoBehaviour, IConveyorable
{
    [SerializeField] private ConveyorManager converyorManager;
    [SerializeField] private EggCollector collector;
    [SerializeField] private CircularSaw saw;
    [SerializeField] private bool isLastConveyor = true;
    public bool IsLastConveyor => isLastConveyor;
    private int floorIndex = 0;
    private void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void SetFloorIndex(int index)
    {
        floorIndex = index;
    }

    public void SetLastFloor(bool enable)
    {
        isLastConveyor = enable;
        if (converyorManager != null)
        {
            converyorManager.SetLastConveyor(isLastConveyor);
        }
    }

    public void Mirror()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        if (converyorManager != null)
        {
            converyorManager.MirrorDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Egg>(out Egg collidedEgg))
        {
            collidedEgg.OnNewFloorEnter(floorIndex);
        }
    }
}