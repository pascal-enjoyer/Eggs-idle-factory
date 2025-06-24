using UnityEngine;

public class EggTimer
{
    private EggData _eggData;
    private float _currentTimer;
    private bool _isActive;

    public EggData EggData => _eggData;
    public float Progress => 1f - (_currentTimer / _eggData.CurrentSpawnInterval);
    public bool IsActive => _isActive;

    public EggTimer(EggData eggData)
    {
        _eggData = eggData;
        _currentTimer = eggData.CurrentSpawnInterval;
        _isActive = true;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!_isActive) return;

        _currentTimer -= deltaTime;
        if (_currentTimer <= 0)
        {
            _currentTimer = _eggData.CurrentSpawnInterval;
            OnTimerCompleted?.Invoke(_eggData);
        }
    }

    public void ResetTimer()
    {
        _currentTimer = _eggData.CurrentSpawnInterval;
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        if (active) ResetTimer();
    }

    public event System.Action<EggData> OnTimerCompleted;
}