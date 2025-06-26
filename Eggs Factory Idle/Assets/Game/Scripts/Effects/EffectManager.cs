using UnityEngine;

public static class EffectManager
{
    private static int activeEffectsCount = 0;
    private static float lastEffectTime = -Mathf.Infinity;
    private static readonly float baseEffectInterval = 0.1f; // ������� �������� ����� ��������� (0.1�)
    private static readonly int effectThreshold = 100; // ����� �������� ��������
    private static readonly float intervalProgressionStep = 0.001f; // ��� �������������� ���������� (0.05�)

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
        //Debug.Log($"EffectManager: �������� ��������: {activeEffectsCount}");
    }

    public static void UnregisterEffect()
    {
        activeEffectsCount = Mathf.Max(0, activeEffectsCount - 1);
        //Debug.Log($"EffectManager: �������� ��������: {activeEffectsCount}");
    }

    private static float GetCurrentInterval()
    {
        if (activeEffectsCount <= effectThreshold)
        {
            return baseEffectInterval;
        }

        // �������������� ����������: baseInterval + step * (activeEffectsCount - threshold)
        return baseEffectInterval + intervalProgressionStep * (activeEffectsCount - effectThreshold);
    }
}