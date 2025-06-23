using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    [SerializeField] private GameObject normalConveyor;
    [SerializeField] private GameObject shortenedConveyor;
    [SerializeField] private ConveyorShadow shadow;
    [SerializeField] private bool isLastConveyor = true;

    private IConveyor activeConveyor;

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
            activeConveyor = isLast ? normalConveyor.GetComponent<IConveyor>() : shortenedConveyor.GetComponent<IConveyor>();
            Debug.Log($"ConveyorManager {gameObject.name} set to {(isLast ? "normal" : "shortened")} conveyor, normalActive={normalConveyor.activeSelf}, shortenedActive={shortenedConveyor.activeSelf}");
        }

        if (shadow != null)
        {
            shadow.SetLastConveyor(isLastConveyor);
        }
    }

    public void MirrorDirection()
    {
        if (activeConveyor != null)
        {
            activeConveyor.MirrorDirection();
        }
    }

    public Transform GetShortenPoint()
    {
        return null;
    }
}