using UnityEngine;

public interface IFloorElement
{
    bool IsActive { get; }
    void Activate();
    void Deactivate();
    void ProcessEggCollision(Egg egg, Vector3 collisionPosition);
    void UpdateElement(float deltaTime);
    void SetFloorIndex(int index);
}