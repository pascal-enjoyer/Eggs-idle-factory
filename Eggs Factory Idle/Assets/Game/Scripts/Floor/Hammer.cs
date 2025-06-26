using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Hammer : EggSplitter
{
    [SerializeField] private GameObject effectPrefab; // Префаб эффекта разделения
    [SerializeField] private float inactiveAngle = 45f; // Угол оттягивания назад (зарядка)
    [SerializeField] private float activeAngle = -45f; // Угол удара
    [SerializeField, Tooltip("Длительность зарядки в секундах (уменьшите для ускорения)")]
    private float chargeDuration = 1f; // Длительность анимации зарядки
    [SerializeField, Tooltip("Длительность удара в секундах (уменьшите для ускорения)")]
    private float hitDuration = 0.5f; // Длительность анимации удара
    [SerializeField, Tooltip("Кривая для зарядки (настройте для резкости)")]
    private AnimationCurve chargeCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая анимации зарядки
    [SerializeField, Tooltip("Кривая для удара (настройте для резкости)")]
    private AnimationCurve hitCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая анимации удара
    [SerializeField, Tooltip("Время активности коллайдера во время удара")]
    private float colliderActiveTime = 0.2f; // Время активности коллайдера во время удара
    [SerializeField, Tooltip("Минимальная прозрачность во время зарядки (0 = полностью прозрачный, 1 = непрозрачный)")]
    private float minChargeAlpha = 0.5f; // Минимальная прозрачность при зарядке

    private BoxCollider2D hammerCollider;
    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private bool isAnimating;
    private float colliderTimer;
    private bool isColliderActive;
    private float startAngle;
    private float targetAngle;
    private bool isCharging; // true для зарядки, false для удара

    protected override void Start()
    {
        base.Start();
        hammerCollider = GetComponent<BoxCollider2D>();
        if (hammerCollider == null)
        {
           // Debug.LogError($"Hammer: BoxCollider2D отсутствует на {gameObject.name}!");
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            //Debug.LogError($"Hammer: SpriteRenderer отсутствует на {gameObject.name}!");
            return;
        }
        hammerCollider.isTrigger = true;
        hammerCollider.enabled = false; // Коллайдер изначально выключен
        startAngle = inactiveAngle;
        targetAngle = inactiveAngle;
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
        isCharging = true;
        SetAlpha(1f); // Полная непрозрачность при старте
        //Debug.Log($"Hammer: Инициализация на {gameObject.name}, стартовый угол: {startAngle}, chargeDuration: {chargeDuration}, hitDuration: {hitDuration}, minChargeAlpha: {minChargeAlpha}, parent rotationY: {(transform.parent != null ? transform.parent.localRotation.eulerAngles.y : 0f)}, время: {Time.time}");
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

        // Управление прозрачностью во время зарядки
        if (isCharging)
        {
            float alpha = Mathf.Lerp(1f, minChargeAlpha, t);
            SetAlpha(alpha);
            //Debug.Log($"Hammer: Зарядка на {gameObject.name}, alpha: {alpha}, t: {t}, время: {Time.time}");
        }
        else
        {
            SetAlpha(1f); // Полная непрозрачность во время удара
        }

        // Управление коллайдером во время удара
        if (!isCharging)
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer <= colliderActiveTime && !isColliderActive)
            {
                hammerCollider.enabled = true;
                isColliderActive = true;
                //Debug.Log($"Hammer: Коллайдер включён на {gameObject.name}, время: {Time.time}");
            }
            else if (colliderTimer > colliderActiveTime && isColliderActive)
            {
                hammerCollider.enabled = false;
                isColliderActive = false;
                //Debug.Log($"Hammer: Коллайдер выключен на {gameObject.name}, время: {Time.time}");
            }
        }

        if (animationTimer >= 1f)
        {
            isAnimating = false;
            startAngle = targetAngle; // Обновляем стартовый угол для следующей анимации
            //Debug.Log($"Hammer: Анимация {(isCharging ? "зарядки" : "удара")} завершена на {gameObject.name}, текущий угол: {currentAngle}, alpha: {spriteRenderer.color.a}, длительность: {duration}, время: {Time.time}");
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
        SetAlpha(1f); // Полная непрозрачность при ударе
        //Debug.Log($"Hammer: Активация на {gameObject.name}, стартовый угол: {startAngle}, целевой угол: {targetAngle}, hitDuration: {hitDuration}, время: {Time.time}");
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
        //Debug.Log($"Hammer: Деактивация на {gameObject.name}, стартовый угол: {startAngle}, целевой угол: {targetAngle}, chargeDuration: {chargeDuration}, время: {Time.time}");
    }

    protected override void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        if (isActive && isColliderActive && egg != null && egg.CanBeSplitOnFloor())
        {
            egg.Split();
            PlaySplitEffect(collisionPosition);
            //Debug.Log($"Hammer: Яйцо разбито на {gameObject.name} в позиции {collisionPosition}, время: {Time.time}");
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
            //Debug.LogWarning($"Hammer: effectPrefab не назначен на {gameObject.name}!");
            return;
        }

        if (!IsPositionInCameraView(position))
        {
            //Debug.Log($"Hammer: Эффект пропущен, позиция {position} вне камеры, время: {Time.time}");
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
            //Debug.Log($"Hammer: Воспроизведён эффект разбивания на позиции {position}, время: {Time.time}");
        }
        else
        {
            //Debug.Log($"Hammer: Эффект пропущен из-за ограничений (активных эффектов: {EffectManager.ActiveEffectsCount}), время: {Time.time}");
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            //Debug.LogWarning($"Hammer: Camera.main не найдена, время: {Time.time}");
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