using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Hammer : EggSplitter
{
    [SerializeField] private GameObject effectPrefab; // ������ ������� ����������
    [SerializeField] private float inactiveAngle = 45f; // ���� ����������� ����� (�������)
    [SerializeField] private float activeAngle = -45f; // ���� �����
    [SerializeField, Tooltip("������������ ������� � �������� (��������� ��� ���������)")]
    private float chargeDuration = 1f; // ������������ �������� �������
    [SerializeField, Tooltip("������������ ����� � �������� (��������� ��� ���������)")]
    private float hitDuration = 0.5f; // ������������ �������� �����
    [SerializeField, Tooltip("������ ��� ������� (��������� ��� ��������)")]
    private AnimationCurve chargeCurve = AnimationCurve.Linear(0, 0, 1, 1); // ������ �������� �������
    [SerializeField, Tooltip("������ ��� ����� (��������� ��� ��������)")]
    private AnimationCurve hitCurve = AnimationCurve.Linear(0, 0, 1, 1); // ������ �������� �����
    [SerializeField, Tooltip("����� ���������� ���������� �� ����� �����")]
    private float colliderActiveTime = 0.2f; // ����� ���������� ���������� �� ����� �����
    [SerializeField, Tooltip("����������� ������������ �� ����� ������� (0 = ��������� ����������, 1 = ������������)")]
    private float minChargeAlpha = 0.5f; // ����������� ������������ ��� �������

    private BoxCollider2D hammerCollider;
    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private bool isAnimating;
    private float colliderTimer;
    private bool isColliderActive;
    private float startAngle;
    private float targetAngle;
    private bool isCharging; // true ��� �������, false ��� �����

    protected override void Start()
    {
        base.Start();
        hammerCollider = GetComponent<BoxCollider2D>();
        if (hammerCollider == null)
        {
           // Debug.LogError($"Hammer: BoxCollider2D ����������� �� {gameObject.name}!");
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            //Debug.LogError($"Hammer: SpriteRenderer ����������� �� {gameObject.name}!");
            return;
        }
        hammerCollider.isTrigger = true;
        hammerCollider.enabled = false; // ��������� ���������� ��������
        startAngle = inactiveAngle;
        targetAngle = inactiveAngle;
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
        isCharging = true;
        SetAlpha(1f); // ������ �������������� ��� ������
        //Debug.Log($"Hammer: ������������� �� {gameObject.name}, ��������� ����: {startAngle}, chargeDuration: {chargeDuration}, hitDuration: {hitDuration}, minChargeAlpha: {minChargeAlpha}, parent rotationY: {(transform.parent != null ? transform.parent.localRotation.eulerAngles.y : 0f)}, �����: {Time.time}");
    }

    protected override void Update()
    {
        base.Update();
        if (isAnimating)
        {
            PerformAnimation();
        }
    }

    private void PerformAnimation()
    {
        float duration = isCharging ? chargeDuration : hitDuration;
        AnimationCurve curve = isCharging ? chargeCurve : hitCurve;
        animationTimer += Time.deltaTime / duration;
        float t = Mathf.Clamp01(curve.Evaluate(animationTimer));
        float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t);
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

        // ���������� ������������� �� ����� �������
        if (isCharging)
        {
            float alpha = Mathf.Lerp(1f, minChargeAlpha, t);
            SetAlpha(alpha);
            //Debug.Log($"Hammer: ������� �� {gameObject.name}, alpha: {alpha}, t: {t}, �����: {Time.time}");
        }
        else
        {
            SetAlpha(1f); // ������ �������������� �� ����� �����
        }

        // ���������� ����������� �� ����� �����
        if (!isCharging)
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer <= colliderActiveTime && !isColliderActive)
            {
                hammerCollider.enabled = true;
                isColliderActive = true;
                //Debug.Log($"Hammer: ��������� ������� �� {gameObject.name}, �����: {Time.time}");
            }
            else if (colliderTimer > colliderActiveTime && isColliderActive)
            {
                hammerCollider.enabled = false;
                isColliderActive = false;
                //Debug.Log($"Hammer: ��������� �������� �� {gameObject.name}, �����: {Time.time}");
            }
        }

        if (animationTimer >= 1f)
        {
            isAnimating = false;
            startAngle = targetAngle; // ��������� ��������� ���� ��� ��������� ��������
            //Debug.Log($"Hammer: �������� {(isCharging ? "�������" : "�����")} ��������� �� {gameObject.name}, ������� ����: {currentAngle}, alpha: {spriteRenderer.color.a}, ������������: {duration}, �����: {Time.time}");
        }
    }

    protected override void Activate()
    {
        isActive = true;
        timer = activeTime;
        startAngle = transform.localRotation.eulerAngles.z;
        targetAngle = activeAngle;
        isCharging = false;
        isAnimating = true;
        animationTimer = 0f;
        colliderTimer = 0f;
        SetAlpha(1f); // ������ �������������� ��� �����
        //Debug.Log($"Hammer: ��������� �� {gameObject.name}, ��������� ����: {startAngle}, ������� ����: {targetAngle}, hitDuration: {hitDuration}, �����: {Time.time}");
    }

    protected override void Deactivate()
    {
        isActive = false;
        timer = inactiveTime;
        startAngle = transform.localRotation.eulerAngles.z;
        targetAngle = inactiveAngle;
        isCharging = true;
        isAnimating = true;
        animationTimer = 0f;
        colliderTimer = 0f;
        hammerCollider.enabled = false;
        isColliderActive = false;
        //Debug.Log($"Hammer: ����������� �� {gameObject.name}, ��������� ����: {startAngle}, ������� ����: {targetAngle}, chargeDuration: {chargeDuration}, �����: {Time.time}");
    }

    protected override void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        if (isActive && isColliderActive && egg != null && egg.CanBeSplitOnFloor())
        {
            egg.Split();
            PlaySplitEffect(collisionPosition);
            //Debug.Log($"Hammer: ���� ������� �� {gameObject.name} � ������� {collisionPosition}, �����: {Time.time}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Egg"))
        {
            Egg egg = other.GetComponent<Egg>();
            ProcessEggCollision(egg, other.transform.position);
        }
    }

    private void PlaySplitEffect(Vector3 position)
    {
        if (effectPrefab == null)
        {
            //Debug.LogWarning($"Hammer: effectPrefab �� �������� �� {gameObject.name}!");
            return;
        }

        if (!IsPositionInCameraView(position))
        {
            //Debug.Log($"Hammer: ������ ��������, ������� {position} ��� ������, �����: {Time.time}");
            return;
        }

        if (EffectManager.CanPlayEffect())
        {
            GameObject effectInstance = Instantiate(effectPrefab, position, Quaternion.identity, transform.parent);
            EffectController controller = effectInstance.GetComponent<EffectController>();
            if (controller == null)
            {
                controller = effectInstance.AddComponent<EffectController>();
            }
            //Debug.Log($"Hammer: ������������ ������ ���������� �� ������� {position}, �����: {Time.time}");
        }
        else
        {
            //Debug.Log($"Hammer: ������ �������� ��-�� ����������� (�������� ��������: {EffectManager.ActiveEffectsCount}), �����: {Time.time}");
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            //Debug.LogWarning($"Hammer: Camera.main �� �������, �����: {Time.time}");
            return false;
        }

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
               viewportPoint.z >= 0f;
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}