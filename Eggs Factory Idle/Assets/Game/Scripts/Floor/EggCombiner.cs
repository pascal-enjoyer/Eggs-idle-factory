using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class EggCombiner : MonoBehaviour, ICollector
{
    [SerializeField] private GameObject bigEggPrefab;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private int eggThreshold = 3;
    [SerializeField] private float bufferInterval = 0.2f;
    [SerializeField] private float activeTime = 0.5f;
    [SerializeField] private float inactiveTime = 1f;
    [SerializeField] private float vibrationAmplitude = 0.1f;
    [SerializeField] private float vibrationFrequency = 4f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float minAlpha = 0.5f;

    private float coinBuffer = 0f;
    private float bufferTimer = 0f;
    private int collectedEggs = 0;
    private float totalCoinReward = 0f;
    private EggData lastEggData;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool isActive = false;
    private float timer;
    private float fadeTimer;
    private bool isFading;
    private float startAlpha;
    private float targetAlpha;
    private Vector3 basePosition;
    public static event Action OnEggCollected;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }
        boxCollider.isTrigger = true;
        basePosition = transform.localPosition;
        timer = inactiveTime;
        isActive = false;
        SetAlpha(0.5f);
        
    }

    private void Update()
    {
        if (coinBuffer > 0f)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                bufferTimer = bufferInterval;
            }
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
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

        if (isActive)
        {
            PerformVibration();
        }

        if (isFading)
        {
            PerformFade();
        }
    }

    private void PerformVibration()
    {
        float offset = Mathf.Sin(Time.time * vibrationFrequency * 2 * Mathf.PI) * vibrationAmplitude;
        transform.localPosition = basePosition + transform.right * offset;
       
    }

    private void PerformFade()
    {
        fadeTimer += Time.deltaTime / fadeDuration;
        float t = Mathf.Clamp01(fadeCurve.Evaluate(fadeTimer));
        float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
        SetAlpha(currentAlpha);

        if (fadeTimer >= 1f)
        {
            isFading = false;
            
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    private void Activate()
    {
        isActive = true;
        timer = activeTime;
        startAlpha = spriteRenderer.color.a;
        targetAlpha = 1f;
        isFading = true;
        fadeTimer = 0f;
        boxCollider.enabled = true;
        
    }

    private void Deactivate()
    {
        isActive = false;
        timer = inactiveTime;
        startAlpha = spriteRenderer.color.a;
        targetAlpha = minAlpha;
        isFading = true;
        fadeTimer = 0f;
        boxCollider.enabled = false;
        
    }

    public void ProcessCollectible(ICollectable collectible)
    {
        if (collectible == null || !isActive)
        {
            return;
        }

        float coins = collectible.GetCoinReward();
        coinBuffer += coins;
        totalCoinReward += coins;
        collectedEggs++;
        lastEggData = (collectible as Egg)?.EggData;
        collectible.Collect();
        OnEggCollected?.Invoke();

        if (collectedEggs >= eggThreshold)
        {
            SpawnBigEgg();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.layer == LayerMask.NameToLayer("Egg") || other.gameObject.layer == LayerMask.NameToLayer("EggHalf")) &&
            other.GetComponent<CombinedEggData>() == null)
        {
            ICollectable collectible = other.GetComponent<ICollectable>();
            if (collectible != null)
            {
                ProcessCollectible(collectible);
            }
        }
    }

    private void SpawnBigEgg()
    {
        if (bigEggPrefab == null)
        {
            
            return;
        }

        if (lastEggData == null)
        {
            
            return;
        }

        GameObject bigEgg = Instantiate(bigEggPrefab, transform.position, Quaternion.identity, transform.parent);
        CombinedEggData eggScript = bigEgg.GetComponent<CombinedEggData>();
        if (eggScript == null)
        {
           
            Destroy(bigEgg);
            return;
        }

        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound("3", Vector3.zero);
        }
        PlaySpawnEffect(transform.position);

        eggScript.Initialize(totalCoinReward / 2f, lastEggData.EggSprite);

        collectedEggs = 0;
        totalCoinReward = 0f;
    }

    private void PlaySpawnEffect(Vector3 position)
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