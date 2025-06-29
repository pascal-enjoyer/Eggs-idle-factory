using UnityEngine;

public interface IConveyorFloorElement
{
    bool CanBeUsed();
    void OnTriggerEnter2D();
}
