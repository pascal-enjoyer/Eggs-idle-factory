// CircularSaw.cs
using UnityEngine;

public class CircularSaw : MonoBehaviour
{
    [SerializeField] private float activeTime = 1f; // ����� ���������� ����
    [SerializeField] private float inactiveTime = 3f; // ����� ����� �����������
    [SerializeField] private Transform activePosition; // �������, ����� ���� �������
    [SerializeField] private Transform inactivePosition; // �������, ����� ���� ���������
    [SerializeField] private float moveDuration = 0.5f; // ������������ ��������
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1); // ������ ��������

    private float timer;
    private bool isActive;
    private bool isMoving;
    private float moveTimer;
    private Transform targetPosition;

    private void Start()
    {
        timer = inactiveTime;
        transform.position = inactivePosition.position;
        targetPosition = inactivePosition;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && !isMoving)
        {
            isActive = !isActive;
            timer = isActive ? activeTime : inactiveTime;
            targetPosition = isActive ? activePosition : inactivePosition;
            isMoving = true;
            moveTimer = 0f;
        }

        if (isMoving)
        {
            moveTimer += Time.deltaTime / moveDuration;
            float t = moveCurve.Evaluate(moveTimer);
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, t);
            if (moveTimer >= 1f)
            {
                isMoving = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive)
        {
            Egg egg = other.GetComponent<Egg>();
            if (egg != null && egg.CanBeSplit())
            {
                egg.Split();
            }
        }
    }
}