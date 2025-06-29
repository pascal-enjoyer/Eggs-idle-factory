using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Hammer : EggSplitter
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float inactiveAngle = 45f;
    [SerializeField] private float activeAngle = -45f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float hitDuration = 0.5f;
    [SerializeField] private AnimationCurve chargeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve hitCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float colliderActiveTime = 0.2f;
    [SerializeField] private float minChargeAlpha = 0.5f;

    private BoxCollider2D hammerCollider;
    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private bool isAnimating;
    private float colliderTimer;
    private bool isColliderActive;
    private float startAngle;
    private float targetAngle;
    private bool isCharging;

    protected override void Start()
    {
        base.Start();
        hammerCollider = GetComponent<BoxCollider2D>();
        if (hammerCollider == null)
        {
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }
        hammerCollider.isTrigger = true;
        hammerCollider.enabled = false;
        startAngle = inactiveAngle;
        targetAngle = inactiveAngle;
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
        isCharging = true;
        SetAlpha(1f);
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

        if (isCharging)
        {
            float alpha = Mathf.Lerp(1f, minChargeAlpha, t);
            SetAlpha(alpha);
            
        }
        else
        {
            SetAlpha(1f);
        }

        if (!isCharging)
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer <= colliderActiveTime && !isColliderActive)
            {
                hammerCollider.enabled = true;
                isColliderActive = true;
            }
            else if (colliderTimer > colliderActiveTime && isColliderActive)
            {
                hammerCollider.enabled = false;
                isColliderActive = false;
            }
        }

        if (animationTimer >= 1f)
        {
            isAnimating = false;
            startAngle = targetAngle;
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

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}