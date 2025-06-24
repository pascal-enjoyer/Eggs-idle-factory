using UnityEngine;

public interface IConveyor
{
    void MoveObject(Rigidbody2D rb);
    void MirrorDirection();
}