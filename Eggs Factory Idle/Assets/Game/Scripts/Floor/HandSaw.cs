using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HandSaw : EggSplitter
{
    [SerializeField] private GameObject effectPrefab; // Префаб эффекта разделения
    [SerializeField] private Transform activationPoint; // Точка активации (угол и позиция)
    [SerializeField] private Transform deactivationPoint; // Точка деактивации (угол и позиция)
    [SerializeField, Tooltip("Амплитуда движения распила (вдоль локальной оси X)")]
    private float sawAmplitude = 0.5f; // Амплитуда движения вперёд-назад
    [SerializeField, Tooltip("Скорость движения распила (циклов в секунду)")]
    private float sawFrequency = 2f; // Частота движения распила
    [SerializeField, Tooltip("Время активности коллайдера во время удара")]
    private float colliderActiveTime = 0.2f; // Время активности коллайдера
    [SerializeField, Tooltip("Длительность поворота при активации/деактивации")]
    private float rotationDuration = 0.3f; // Длительность поворота
    [SerializeField, Tooltip("Кривая анимации поворота")]
    private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая поворота

    private BoxCollider2D sawCollider;
    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private bool isAnimating;
    private float colliderTimer;
    private bool isColliderActive;
    private float startAngle;
    private float targetAngle;
    private Vector3 startPosition;
    private Vector3 basePosition; // Базовая позиция для движения распила

    protected override void Start()
    {
        base.Start();
        sawCollider = GetComponent<BoxCollider2D>();
        if (sawCollider == null)
        {
            Debug.LogError($"HandSaw: BoxCollider2D отсутствует на {gameObject.name}!");
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"HandSaw: SpriteRenderer отсутствует на {gameObject.name}!");
            return;
        }
        if (activationPoint == null || deactivationPoint == null)
        {
            Debug.LogError($"HandSaw: Точки активации или деактивации не назначены на {gameObject.name}!");
            return;
        }
        sawCollider.isTrigger = true;
        sawCollider.enabled = false; // Коллайдер изначально выключен
        startAngle = deactivationPoint.localRotation.eulerAngles.z;
        targetAngle = startAngle;
        basePosition = transform.localPosition;
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
        transform.localPosition = deactivationPoint.localPosition;
        Debug.Log($"HandSaw: Инициализация на {gameObject.name}, стартовый угол: {startAngle}, базовая позиция: {basePosition}, parent rotationY: {(transform.parent != null ? transform.parent.localRotation.eulerAngles.y : 0f)}, время: {Time.time}");
    }

    protected override void Update()
    {
        base.Update();
        if (isAnimating)
        {
            PerformAnimation();
        }
        if (isActive)
        {
            PerformSawMotion();
        }
    }

    private void PerformAnimation()
    {
        animationTimer += Time.deltaTime / rotationDuration;
        float t = Mathf.Clamp01(rotationCurve.Evaluate(animationTimer));
        float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t);
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

        // Линейная интерполяция позиции между начальной и целевой
        Vector3 targetPosition = isActive ? activationPoint.localPosition : deactivationPoint.localPosition;
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

        // Управление коллайдером во время активации
        if (isActive)
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer <= colliderActiveTime && !isColliderActive)
            {
                sawCollider.enabled = true;
                isColliderActive = true;
                Debug.Log($"HandSaw: Коллайдер включён на {gameObject.name}, время: {Time.time}");
            }
            else if (colliderTimer > colliderActiveTime && isColliderActive)
            {
                sawCollider.enabled = false;
                isColliderActive = false;
                Debug.Log($"HandSaw: Коллайдер выключен на {gameObject.name}, время: {Time.time}");
            }
        }

        if (animationTimer >= 1f)
        {
            isAnimating = false;
            startAngle = targetAngle;
            startPosition = transform.localPosition;
            Debug.Log($"HandSaw: Анимация поворота завершена на {gameObject.name}, текущий угол: {currentAngle}, позиция: {transform.localPosition}, время: {Time.time}");
        }
    }

    private void PerformSawMotion()
    {
        // Движение вперёд-назад вдоль локальной оси X
        float offset = Mathf.Sin(Time.time * sawFrequency * 2 * Mathf.PI) * sawAmplitude;
        transform.localPosition = basePosition + transform.right * offset;
        Debug.Log($"HandSaw: Движение распила на {gameObject.name}, offset: {offset}, позиция: {transform.localPosition}, время: {Time.time}");
    }

    protected override void Activate()
    {
        isActive = true;
        timer = activeTime;
        startAngle = transform.localRotation.eulerAngles.z;
        targetAngle = activationPoint.localRotation.eulerAngles.z;
        startPosition = transform.localPosition;
        basePosition = activationPoint.localPosition; // Базовая позиция для движения распила
        isAnimating = true;
        animationTimer = 0f;
        colliderTimer = 0f;
        Debug.Log($"HandSaw: Активация на {gameObject.name}, стартовый угол: {startAngle}, целевой угол: {targetAngle}, базовая позиция: {basePosition}, время: {Time.time}");
    }

    protected override void Deactivate()
    {
        isActive = false;
        timer = inactiveTime;
        startAngle = transform.localRotation.eulerAngles.z;
        targetAngle = deactivationPoint.localRotation.eulerAngles.z;
        startPosition = transform.localPosition;
        basePosition = deactivationPoint.localPosition;
        isAnimating = true;
        animationTimer = 0f;
        colliderTimer = 0f;
        sawCollider.enabled = false;
        isColliderActive = false;
        Debug.Log($"HandSaw: Деактивация на {gameObject.name}, стартовый угол: {startAngle}, целевой угол: {targetAngle}, базовая позиция: {basePosition}, время: {Time.time}");
    }

    protected override void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        if (isActive && isColliderActive && egg != null && egg.CanBeSplitOnFloor())
        {
            egg.Split();
            PlaySplitEffect(collisionPosition);
            Debug.Log($"HandSaw: Яйцо разбито на {gameObject.name} в позиции {collisionPosition}, время: {Time.time}");
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
            Debug.LogWarning($"HandSaw: effectPrefab не назначен на {gameObject.name}!");
            return;
        }

        if (!IsPositionInCameraView(position))
        {
            Debug.Log($"HandSaw: Эффект пропущен, позиция {position} вне камеры, время: {Time.time}");
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
            Debug.Log($"HandSaw: Воспроизведён эффект разбивания на позиции {position}, время: {Time.time}");
        }
        else
        {
            Debug.Log($"HandSaw: Эффект пропущен из-за ограничений (активных эффектов: {EffectManager.ActiveEffectsCount}), время: {Time.time}");
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning($"HandSaw: Camera.main не найдена, время: {Time.time}");
            return false;
        }

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
               viewportPoint.z >= 0f;
    }
}