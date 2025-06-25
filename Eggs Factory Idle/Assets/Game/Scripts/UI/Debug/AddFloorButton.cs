using UnityEngine;
using UnityEngine.UI;

public class AddFloorButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private ConveyorFloorManager floorManager;

    private void Awake()
    {
        if (button != null && floorManager != null)
        {
            button.onClick.AddListener(floorManager.AddFloor);
        }
    }
}