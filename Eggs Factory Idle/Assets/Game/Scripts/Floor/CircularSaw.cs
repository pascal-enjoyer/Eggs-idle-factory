using UnityEngine;

public class CircularSaw : EggSplitter
{
    [SerializeField] private Transform activePosition;
    [SerializeField] private Transform inactivePosition;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private GameObject effectPrefab;

    private bool isMoving;
    private float moveTimer;
    private Transform targetPosition;

    protected override void Start()
    {
        base.Start();
        if (activePosition == null || inactivePosition == null)
        {
            return;
        }
        transform.position = inactivePosition.position;
        targetPosition = inactivePosition;
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
        }
    }

    protected override void Activate()
    {
        isActive = true;
        timer = activeTime;
        targetPosition = activePosition;
        isMoving = true;
        moveTimer = 0f;
    }

    protected override void Deactivate()
    {
        isActive = false;
        timer = inactiveTime;
        targetPosition = inactivePosition;
        isMoving = true;
        moveTimer = 0f;
    }

    protected override void ProcessEggCollision(Egg egg, Vector3 collisionPosition)
    {
        if (isActive && egg != null && egg.CanBeSplitOnFloor())
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
            GameObject effectInstance = Instantiate(effectPrefab, position, Quaternion.identity, transform);
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