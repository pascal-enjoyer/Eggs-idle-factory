using UnityEngine;

public interface IConveyor
{
    void SetLastConveyor(bool isLast);
    void MoveObject(Rigidbody2D rb);
    void MirrorDirection();
    Transform GetShortenPoint();
}