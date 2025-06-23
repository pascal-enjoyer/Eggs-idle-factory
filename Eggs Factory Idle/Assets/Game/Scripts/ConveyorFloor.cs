using UnityEngine;

public class ConveyorFloor : MonoBehaviour, IConveyorable
{
    [SerializeField] private ConveyorManager converyorManager;
    [SerializeField] private EggCollector collector;
    [SerializeField] private CircularSaw saw;
    [SerializeField] private bool isLastConveyor = true;

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
}