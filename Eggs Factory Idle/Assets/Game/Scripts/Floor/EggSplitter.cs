using UnityEngine;

public abstract class EggSplitter : MonoBehaviour
{
    [SerializeField] protected float activeTime = 1f; // ����� ����������
    [SerializeField] protected float inactiveTime = 3f; // ����� ����� �����������

    protected float timer;
    protected bool isActive;

    protected virtual void Start()
    {
        timer = inactiveTime;
    }

    protected virtual void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (isActive)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }
    }

    protected virtual void Activate()
    {
        isActive = true;
        timer = activeTime;
    }

    protected virtual void Deactivate()
    {
        isActive = false;
        timer = inactiveTime;
    }

    protected virtual void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        // ���������� �� ���������, ����� ���� ��������������
    }
}