using UnityEngine;
using System.Collections.Generic;

public class ConveyorFloorManager : MonoBehaviour
{
    [SerializeField] private List<ConveyorFloor> floors = new List<ConveyorFloor>();
    [SerializeField] private List<GameObject> floorPrefabs = new List<GameObject>(); // Список префабов этажей
    [SerializeField] private float verticalSpacing = 2f; // Расстояние между этажами
    [SerializeField] private Transform initialPosition; // Начальная позиция первого этажа
    [SerializeField] private int maxFloorsCount = 5; // Максимальное число этажей

    private bool isSubscribed;
    private int currentPrefabIndex = 0; // Текущий индекс префаба

    private void Awake()
    {
        if (floorPrefabs == null || floorPrefabs.Count == 0 || initialPosition == null)
        {
            //Debug.LogError("ConveyorFloorManager: floorPrefabs не назначены или список пуст, либо initialPosition не назначена!");
            return;
        }
        if (floors.Count == 0)
        {
            AddFloor();
        }
    }

    private void Start()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.OnUpgradeChanged += UpdateConveyorCount;
            isSubscribed = true;
            UpdateConveyorCount();
            //Debug.Log("ConveyorFloorManager: Подписка на UpgradeSystem выполнена");
        }
        else
        {
            //Debug.LogError("ConveyorFloorManager: UpgradeSystem.Instance не инициализирован!");
        }
    }

    private void OnDestroy()
    {
        if (UpgradeSystem.Instance != null && isSubscribed)
        {
            UpgradeSystem.OnUpgradeChanged -= UpdateConveyorCount;
        }
    }

    public void AddFloor()
    {
        if (floorPrefabs.Count == 0)
        {
            //Debug.LogError("ConveyorFloorManager: Список floorPrefabs пуст!");
            return;
        }
        if (floors.Count >= maxFloorsCount)
        {
           //Debug.LogWarning("ConveyorFloorManager: Достигнуто максимальное число этажей!");
            return;
        }

        // Выбор префаба по текущему индексу
        GameObject selectedPrefab = floorPrefabs[currentPrefabIndex];

        // Спавн нового этажа с вертикальным смещением
        Vector3 spawnPosition = initialPosition.position + new Vector3(0f, -verticalSpacing * floors.Count, 0f);
        GameObject newFloor = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);
        ConveyorFloor newConveyorFloor = newFloor.GetComponent<ConveyorFloor>();
        if (newConveyorFloor == null)
        {
            //Debug.LogError("ConveyorFloorManager: Выбранный префаб не содержит компонент ConveyorFloor!");
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

        // Обновление индекса префаба (циклический переход)
        currentPrefabIndex = (currentPrefabIndex + 1) % floorPrefabs.Count;
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