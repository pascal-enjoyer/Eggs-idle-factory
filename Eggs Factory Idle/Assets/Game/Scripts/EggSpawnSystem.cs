using System.Collections.Generic;
using UnityEngine;

public class EggSpawnSystem : MonoBehaviour
{
    public static EggSpawnSystem Instance { get; private set; }

    [SerializeField] private EggSpawner _spawner;
    private Dictionary<EggData, EggTimer> _eggTimers = new Dictionary<EggData, EggTimer>();

    public event System.Action<EggData> EggLevelUpgraded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        if (_spawner == null)
        {
            _spawner = GetComponent<EggSpawner>();
            if (_spawner == null)
            {
                Debug.LogError("EggSpawnSystem: �� ������ EggSpawner!");
            }
        }
    }

    private void Update()
    {
        foreach (var timer in _eggTimers.Values)
        {
            // ��������� ���� �������� �������� ������
            float doubleSpeedChance = GameModifiers.Instance.GetDoubleFruitSpeedChance();
            float speedMultiplier = UnityEngine.Random.value < doubleSpeedChance ? 2f : 1f;
            timer.UpdateTimer(Time.deltaTime * speedMultiplier);
        }
    }

    public void AddEgg(EggData eggData)
    {
        if (eggData == null)
        {
            Debug.LogError("EggSpawnSystem: ������� �������� null EggData!");
            return;
        }

        if (eggData.IsUnlocked && eggData.UpgradeLevel > 0)
        {
            if (!_eggTimers.ContainsKey(eggData))
            {
                var timer = new EggTimer(eggData);
                timer.OnTimerCompleted += HandleTimerCompleted;
                _eggTimers.Add(eggData, timer);
                Debug.Log($"EggSpawnSystem: �������� ������ ��� ���� {eggData.EggName}, ��������={eggData.CurrentSpawnInterval}");
            }
            else
            {
                Debug.Log($"EggSpawnSystem: ������ ��� ���� {eggData.EggName} ��� ����������");
            }
        }
        else
        {
            Debug.LogWarning($"EggSpawnSystem: ���� {eggData.EggName} �� ���������: IsUnlocked={eggData.IsUnlocked}, UpgradeLevel={eggData.UpgradeLevel}");
        }
    }

    public float GetEggProgress(EggData eggData)
    {
        return _eggTimers.TryGetValue(eggData, out var timer) ? timer.Progress : 0f;
    }

    private void HandleTimerCompleted(EggData eggData)
    {
        if (_spawner != null)
        {
            _spawner.SpawnEgg(eggData);
            Debug.Log($"EggSpawnSystem: ����� ���� {eggData.EggName}");
        }
        else
        {
            Debug.LogError("EggSpawnSystem: _spawner �� ��������!");
        }
    }

    public void NotifyEggUpgraded(EggData eggData)
    {
        EggLevelUpgraded?.Invoke(eggData);
    }
}