using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HandSaw : EggSplitter
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform activationPoint;
    [SerializeField] private Transform deactivationPoint;
    [SerializeField] private float sawAmplitude = 0.5f;
    [SerializeField] private float sawFrequency = 2f;
    [SerializeField] private float colliderActiveTime = 0.2f;
    [SerializeField] private float rotationDuration = 0.3f;
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private BoxCollider2D sawCollider;
    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private bool isAnimating;
    private float colliderTimer;
    private bool isColliderActive;
    private float startAngle;
    private float targetAngle;
    private Vector3 startPosition;
    private Vector3 basePosition;

    protected override void Start()
    {
        base.Start();
        sawCollider = GetComponent<BoxCollider2D>();
        if (sawCollider == null)
        {
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }
        if (activationPoint == null || deactivationPoint == null)
        {
            return;
        }
        sawCollider.isTrigger = true;
        sawCollider.enabled = false;
        startAngle = deactivationPoint.localRotation.eulerAngles.z;
        targetAngle = startAngle;
        basePosition = transform.localPosition;
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
        transform.localPosition = deactivationPoint.localPosition;
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

        Vector3 targetPosition = isActive ? activationPoint.localPosition : deactivationPoint.localPosition;
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

        if (isActive)
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer <= colliderActiveTime && !isColliderActive)
            {
                sawCollider.enabled = true;
                isColliderActive = true;
            }
            else if (colliderTimer > colliderActiveTime && isColliderActive)
            {
                sawCollider.enabled = false;
                isColliderActive = false;
            }
        }

        if (animationTimer >= 1f)
        {
            isAnimating = false;
            startAngle = targetAngle;
            startPosition = transform.localPosition;
        }
    }

    private void PerformSawMotion()
    {
        float offset = Mathf.Sin(Time.time * sawFrequency * 2 * Mathf.PI) * sawAmplitude;
        transform.localPosition = basePosition + transform.right * offset;
    }

    protected override void Activate()
    {
        isActive = true;
        timer = activeTime;
        startAngle = transform.localRotation.eulerAngles.z;
        targetAngle = activationPoint.localRotation.eulerAngles.z;
        startPosition = transform.localPosition;
        basePosition = activationPoint.localPosition;
        isAnimating = true;
        animationTimer = 0f;
        colliderTimer = 0f;
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
    }

    protected override void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        if (isActive && isColliderActive && egg != null && egg.CanBeSplitOnFloor())
        {
            egg.Split();
            PlaySplitEffect(collisionPosition);
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
            return;
        }

        if (!IsPositionInCameraView(position))
        {
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
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return false;
        }

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
               viewportPoint.z >= 0f;
    }
}