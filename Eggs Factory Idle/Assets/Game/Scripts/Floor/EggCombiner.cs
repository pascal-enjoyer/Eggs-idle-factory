using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class EggCombiner : MonoBehaviour, ICollector
{
    [SerializeField] private GameObject bigEggPrefab; // Префаб большого яйца
    [SerializeField] private GameObject effectPrefab; // Префаб эффекта спавна
    [SerializeField, Tooltip("Количество яиц для создания большого яйца")]
    private int eggThreshold = 3; // Сколько яиц нужно собрать
    [SerializeField, Tooltip("Фиксированный интервал спавна текста монет (секунды)")]
    private float bufferInterval = 0.2f; // Интервал спавна текста
    [SerializeField, Tooltip("Длительность активности (секунды)")]
    private float activeTime = 0.5f; // Время активного состояния
    [SerializeField, Tooltip("Длительность неактивности (секунды)")]
    private float inactiveTime = 1f; // Время неактивного состояния
    [SerializeField, Tooltip("Амплитуда вибрации (вдоль локальной оси X)")]
    private float vibrationAmplitude = 0.1f; // Амплитуда вибрации
    [SerializeField, Tooltip("Скорость вибрации (циклов в секунду)")]
    private float vibrationFrequency = 4f; // Частота вибрации
    [SerializeField, Tooltip("Длительность анимации прозрачности (секунды)")]
    private float fadeDuration = 0.3f; // Длительность изменения прозрачности
    [SerializeField, Tooltip("Кривая анимации прозрачности")]
    private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая прозрачности
    [SerializeField, Tooltip("Минимальная прозрачность в неактивном состоянии")]
    private float minAlpha = 0.5f; // Минимальная прозрачность

    private float coinBuffer = 0f; // Буфер для монет
    private float bufferTimer = 0f; // Таймер буфера
    private int collectedEggs = 0; // Счётчик собранных яиц
    private float totalCoinReward = 0f; // Сумма монет от собранных яиц
    private EggData lastEggData; // Данные последнего собранного яйца
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool isActive = false; // Состояние активности
    private float timer; // Таймер для активации/деактивации
    private float fadeTimer; // Таймер для анимации прозрачности
    private bool isFading; // Идёт ли анимация прозрачности
    private float startAlpha; // Начальная прозрачность
    private float targetAlpha; // Целевая прозрачность
    private Vector3 basePosition; // Базовая позиция для вибрации
    public static event Action OnEggCollected; // Событие сбора яйца

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError($"EggCombiner: BoxCollider2D отсутствует на {gameObject.name}!");
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"EggCombiner: SpriteRenderer отсутствует на {gameObject.name}!");
            return;
        }
        boxCollider.isTrigger = true;
        basePosition = transform.localPosition;
        timer = inactiveTime;
        isActive = false;
        SetAlpha(0.5f); // Начальная прозрачность
        Debug.Log($"EggCombiner: Инициализация на {gameObject.name}, базовая позиция: {basePosition}, minAlpha: {minAlpha}, parent rotationY: {(transform.parent != null ? transform.parent.localRotation.eulerAngles.y : 0f)}, время: {Time.time}");
    }

    private void Update()
    {
        // Управление буфером текста монет
        if (coinBuffer > 0f)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                bufferTimer = bufferInterval;
            }
        }

        // Управление состоянием активации/деактивации
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

        // Вибрация при активном состоянии
        if (isActive)
        {
            PerformVibration();
        }

        // Анимация прозрачности
        if (isFading)
        {
            PerformFade();
        }
    }

    private void PerformVibration()
    {
        float offset = Mathf.Sin(Time.time * vibrationFrequency * 2 * Mathf.PI) * vibrationAmplitude;
        transform.localPosition = basePosition + transform.right * offset;
        //Debug.Log($"EggCombiner: Вибрация на {gameObject.name}, offset: {offset}, позиция: {transform.localPosition}, время: {Time.time}");
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
            //Debug.Log($"EggCombiner: Анимация прозрачности завершена на {gameObject.name}, alpha: {currentAlpha}, время: {Time.time}");
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
        //Debug.Log($"EggCombiner: Активация на {gameObject.name}, целевая прозрачность: {targetAlpha}, время: {Time.time}");
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
        //Debug.Log($"EggCombiner: Деактивация на {gameObject.name}, целевая прозрачность: {targetAlpha}, время: {Time.time}");
    }

    public void ProcessCollectible(ICollectable collectible)
    {
        if (collectible == null || !isActive)
        {
            //Debug.LogWarning($"EggCombiner: Получен null collectible или неактивное состояние на {gameObject.name}");
            return;
        }

        // Собираем монеты
        float coins = collectible.GetCoinReward();
        coinBuffer += coins;
        totalCoinReward += coins;
        collectedEggs++;
        lastEggData = (collectible as Egg)?.EggData; // Сохраняем EggData для спрайта
        collectible.Collect();
        OnEggCollected?.Invoke();

        //Debug.Log($"EggCombiner: Собрано яйцо на {gameObject.name}, монеты: {coins}, общее количество: {collectedEggs}/{eggThreshold}, totalCoinReward: {totalCoinReward}, позиция: {transform.position}, время: {Time.time}");

        // Проверяем, собрано ли достаточно яиц
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
            //Debug.LogWarning($"EggCombiner: bigEggPrefab не назначен на {gameObject.name}!");
            return;
        }

        if (lastEggData == null)
        {
            //Debug.LogWarning($"EggCombiner: EggData не сохранён, невозможно создать большое яйцо на {gameObject.name}!");
            return;
        }

        // Спавним большое яйцо
        GameObject bigEgg = Instantiate(bigEggPrefab, transform.position, Quaternion.identity, transform.parent);
        CombinedEggData eggScript = bigEgg.GetComponent<CombinedEggData>();
        if (eggScript == null)
        {
            //Debug.LogWarning($"EggCombiner: bigEggPrefab не содержит компонент CombinedEggData на {gameObject.name}!");
            Destroy(bigEgg);
            return;
        }

        // Воспроизведение звука повышения уровня (ID "0")
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound("3", Vector3.zero);
            //Debug.Log($"PlayerEconomy: Played level up sound for level {level}");
        }
        else
        {
            //Debug.LogWarning("PlayerEconomy: AudioManager не найден для воспроизведения звука уровня!");
        }

        // Воспроизведение эффекта спавна
        PlaySpawnEffect(transform.position);

        // Инициализируем большое яйцо
        eggScript.Initialize(totalCoinReward / 2f, lastEggData.EggSprite);

        //Debug.Log($"EggCombiner: Создано большое яйцо на {gameObject.name}, монеты: {totalCoinReward / 2f}, позиция: {bigEgg.transform.position}, время: {Time.time}");

        // Сбрасываем счётчик и сумму
        collectedEggs = 0;
        totalCoinReward = 0f;
    }

    private void PlaySpawnEffect(Vector3 position)
    {
        if (effectPrefab == null)
        {
            //Debug.LogWarning($"EggCombiner: effectPrefab не назначен на {gameObject.name}!");
            return;
        }

        if (!IsPositionInCameraView(position))
        {
            //Debug.Log($"EggCombiner: Эффект пропущен, позиция {position} вне камеры");
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
            //Debug.Log($"EggCombiner: Воспроизведён эффект спавна на позиции {position}");
        }
        else
        {
            //Debug.Log($"EggCombiner: Эффект пропущен из-за ограничений (активных эффектов: {EffectManager.ActiveEffectsCount})");
        }
    }

    private bool IsPositionInCameraView(Vector3 worldPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            //Debug.LogWarning("EggCombiner: Camera.main не найдена!");
            return false;
        }

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f && viewportPoint.y <= 1f &&
               viewportPoint.z >= 0f; // Проверяем, что точка перед камерой
    }
}