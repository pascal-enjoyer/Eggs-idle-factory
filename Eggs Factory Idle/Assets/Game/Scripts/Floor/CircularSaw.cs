using UnityEngine;

public class CircularSaw : EggSplitter
{
    [SerializeField] private Transform activePosition; // Позиция, когда пила активна
    [SerializeField] private Transform inactivePosition; // Позиция, когда пила неактивна
    [SerializeField] private float moveDuration = 0.5f; // Длительность движения
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая анимации
    [SerializeField] private GameObject effectPrefab; // Префаб эффекта распиливания

    private bool isMoving;
    private float moveTimer;
    private Transform targetPosition;

    protected override void Start()
    {
        base.Start();
        if (activePosition == null || inactivePosition == null)
        {
            Debug.LogError($"CircularSaw: activePosition или inactivePosition не назначены на {gameObject.name}!");
            return;
        }
        transform.position = inactivePosition.position;
        targetPosition = inactivePosition;
        Debug.Log($"CircularSaw: Инициализация на {gameObject.name}, стартовая позиция: {transform.position}");
    }

    protected override void Update()
    {
        base.Update();
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        moveTimer += Time.deltaTime / moveDuration;
        float t = moveCurve.Evaluate(moveTimer);
        transform.position = Vector3.Lerp(transform.position, targetPosition.position, t);
        if (moveTimer >= 1f)
        {
            isMoving = false;
            Debug.Log($"CircularSaw: Движение завершено на {gameObject.name}, текущая позиция: {transform.position}");
        }
    }

    protected override void Activate()
    {
        isActive = true;
        timer = activeTime;
        targetPosition = activePosition;
        isMoving = true;
        moveTimer = 0f;
        Debug.Log($"CircularSaw: Активация пилы на {gameObject.name}, целевая позиция: {targetPosition.position}");
    }

    protected override void Deactivate()
    {
        isActive = false;
        timer = inactiveTime;
        targetPosition = inactivePosition;
        isMoving = true;
        moveTimer = 0f;
        Debug.Log($"CircularSaw: Деактивация пилы на {gameObject.name}, целевая позиция: {targetPosition.position}");
    }

    protected override void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        if (isActive && egg != null && egg.CanBeSplitOnFloor())
        {
            egg.Split();
            PlaySplitEffect(collisionPosition);
            Debug.Log($"CircularSaw: Яйцо разрезано на {gameObject.name} в позиции {collisionPosition}");
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
            Debug.LogWarning($"CircularSaw: effectPrefab не назначен на {gameObject.name}!");
            return;
        }

        if (!IsPositionInCameraView(position))
        {
            Debug.Log($"CircularSaw: Эффект пропущен, позиция {position} вне камеры");
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
            Debug.Log($"CircularSaw: Воспроизведён эффект распиливания на позиции {position}");
        }
        else
        {
            Debug.Log($"CircularSaw: Эффект пропущен из-за ограничений (активных эффектов: {EffectManager.ActiveEffectsCount})");
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("CircularSaw: Camera.main не найдена!");
            return false;
        }

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
               viewportPoint.z >= 0f; // Проверяем, что точка перед камерой
    }
}