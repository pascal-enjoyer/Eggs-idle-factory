using UnityEngine;

public class CircularSaw : MonoBehaviour
{
    [SerializeField] private float activeTime = 1f; // ����� ���������� ����
    [SerializeField] private float inactiveTime = 3f; // ����� ����� �����������
    [SerializeField] private Transform activePosition; // �������, ����� ���� �������
    [SerializeField] private Transform inactivePosition; // �������, ����� ���� ���������
    [SerializeField] private float moveDuration = 0.5f; // ������������ ��������
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1); // ������ ��������
    [SerializeField] private GameObject effectPrefab; // ������ ������� ������������

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
        if (isActive && other.gameObject.layer == LayerMask.NameToLayer("Egg"))
        {
            Egg egg = other.GetComponent<Egg>();
            if (egg != null && egg.CanBeSplitOnFloor())
            {
                egg.Split();
                PlaySplitEffect(other.transform.position);
            }
        }
    }

    private void PlaySplitEffect(Vector3 position)
    {
        if (effectPrefab == null)
        {
            Debug.LogWarning("CircularSaw: effectPrefab �� ��������!");
            return;
        }

        if (!IsPositionInCameraView(position))
        {
            Debug.Log($"CircularSaw: ������ ��������, ������� {position} ��� ������");
            return;
        }

        if (EffectManager.CanPlayEffect())
        {
            GameObject effectInstance = Instantiate(effectPrefab, position, Quaternion.identity, transform);
            EffectController controller = effectInstance.GetComponent<EffectController>();
            if (controller == null)
            {
                controller = effectInstance.AddComponent<EffectController>();
            }
            Debug.Log($"CircularSaw: ������������� ������ ������������ �� ������� {position}");
        }
        else
        {
            Debug.Log($"CircularSaw: ������ �������� ��-�� ����������� (�������� ��������: {EffectManager.ActiveEffectsCount})");
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("CircularSaw: Camera.main �� �������!");
            return false;
        }

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
               viewportPoint.z >= 0f; // ���������, ��� ����� ����� �������
    }
}