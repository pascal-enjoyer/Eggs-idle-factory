using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    [SerializeField] private GameObject normalConveyor;
    [SerializeField] private GameObject shortenedConveyor;
    [SerializeField] private bool isLastConveyor = true;

    private void Awake()
    {
        if (normalConveyor == null || shortenedConveyor == null)
        {
            Debug.LogError($"ConveyorManager {gameObject.name} missing normalConveyor or shortenedConveyor reference!");
            return;
        }

        SetLastConveyor(isLastConveyor);
    }

    public void SetLastConveyor(bool isLast)
    {
        isLastConveyor = isLast;

        if (normalConveyor != null && shortenedConveyor != null)
        {
            normalConveyor.SetActive(isLast);
            shortenedConveyor.SetActive(!isLast);
            if (isLast)
            {
                normalConveyor.SetActive(isLast);
                shortenedConveyor.SetActive(!isLast);
            }
           
        }

    }

    public void MirrorDirection()
    {
        if (normalConveyor != null && shortenedConveyor != null)
        {
            normalConveyor.GetComponent<IConveyor>().MirrorDirection();
            shortenedConveyor.GetComponent<IConveyor>().MirrorDirection();
        }
    }

}