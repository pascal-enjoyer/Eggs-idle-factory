using UnityEngine;

public static class EffectManager
{
    private static int activeEffectsCount = 0;
    private static float lastEffectTime = -Mathf.Infinity;
    private static readonly float baseEffectInterval = 0.1f; // Базовый интервал между эффектами (0.1с)
    private static readonly int effectThreshold = 100; // Порог активных эффектов
    private static readonly float intervalProgressionStep = 0.001f; // Шаг арифметической прогрессии (0.05с)

    public static int ActiveEffectsCount => activeEffectsCount;

    public static bool CanPlayEffect()
    {
        float currentTime = Time.time;
        float interval = GetCurrentInterval();

        if (currentTime - lastEffectTime >= interval)
        {
            lastEffectTime = currentTime;
            return true;
        }
        return false;
    }

    public static void RegisterEffect()
    {
        activeEffectsCount++;
        //Debug.Log($"EffectManager: Активных эффектов: {activeEffectsCount}");
    }

    public static void UnregisterEffect()
    {
        activeEffectsCount = Mathf.Max(0, activeEffectsCount - 1);
        //Debug.Log($"EffectManager: Активных эффектов: {activeEffectsCount}");
    }

    private static float GetCurrentInterval()
    {
        if (activeEffectsCount <= effectThreshold)
        {
            return baseEffectInterval;
        }

        // Арифметическая прогрессия: baseInterval + step * (activeEffectsCount - threshold)
        return baseEffectInterval + intervalProgressionStep * (activeEffectsCount - effectThreshold);
    }
}