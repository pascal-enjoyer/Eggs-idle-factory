using UnityEngine;
using System.Collections.Generic;

public class ConveyorFloorManager : MonoBehaviour
{
    [SerializeField] private List<ConveyorFloor> floors = new List<ConveyorFloor>();
    [SerializeField] private GameObject floorPrefab; // Префаб этажа
    [SerializeField] private float verticalSpacing = 2f; // Расстояние между этажами
    [SerializeField] private Transform initialPosition; // Начальная позиция первого этажа
    [SerializeField] private int maxFloorsCount = 5; // Максимальное число этажей (ограничиваем апгрейдом)

    private void Awake()
    {
        if (floors.Count == 0)
        {
            AddFloor();
        }
        UpdateConveyorCount();
        // Подписываемся на изменение апгрейдов
        UpgradeSystem.Instance.OnUpgradeChanged += UpdateConveyorCount;
    }

    private void OnDestroy()
    {
        UpgradeSystem.Instance.OnUpgradeChanged -= UpdateConveyorCount;
    }

    public void AddFloor()
    {
        if (floorPrefab == null)
        {
            Debug.LogError("ConveyorFloorManager: floorPrefab не назначен!");
            return;
        }
        if (floors.Count >= maxFloorsCount)
        {
            Debug.LogWarning("ConveyorFloorManager: Достигнуто максимальное число этажей!");
            return;
        }

        // Спавн нового этажа с вертикальным смещением
        Vector3 spawnPosition = initialPosition.position + new Vector3(0f, -verticalSpacing * floors.Count, 0f);
        GameObject newFloor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity, transform);
        ConveyorFloor newConveyorFloor = newFloor.GetComponent<ConveyorFloor>();
        if (newConveyorFloor == null)
        {
            Debug.LogError("ConveyorFloorManager: floorPrefab не содержит компонент ConveyorFloor!");
            Destroy(newFloor);
            return;
        }

        floors.Add(newConveyorFloor);

        // Зеркалирование для чередующихся этажей
        if (floors.Count % 2 == 0)
        {
            newConveyorFloor.Mirror();
        }

        UpdateFloorStates();
    }

    private void UpdateFloorStates()
    {
        for (int i = 0; i < floors.Count; i++)
        {
            bool isLast = i == floors.Count - 1;
            floors[i].SetLastFloor(isLast);
            floors[i].SetFloorIndex(i);
        }
    }

    public void UpdateConveyorCount()
    {
        int targetCount = GameModifiers.Instance.GetConveyorCount();
        while (floors.Count < targetCount)
        {
            AddFloor();
        }
    }
}