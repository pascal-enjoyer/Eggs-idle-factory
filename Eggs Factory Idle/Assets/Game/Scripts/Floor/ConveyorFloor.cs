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
        // Поворачиваем на 180 градусов по оси Y, вместо изменения масштаба
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        float newYRotation = Mathf.Approximately(currentRotation.y, 180f) ? 0f : 180f;
        transform.localRotation = Quaternion.Euler(0f, newYRotation, 0f);
        if (converyorManager != null)
        {
            converyorManager.MirrorDirection();
        }
        Debug.Log($"ConveyorFloor: Зеркалирование на {gameObject.name}, rotation.y: {newYRotation}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Egg>(out Egg collidedEgg))
        {
            collidedEgg.OnNewFloorEnter(floorIndex);
        }
    }
}