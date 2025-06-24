using UnityEngine;
using System.Collections.Generic;

public class ConveyorFloorManager : MonoBehaviour
{
    [SerializeField] private List<ConveyorFloor> floors = new List<ConveyorFloor>();
    [SerializeField] private GameObject floorPrefab; // ������ �����
    [SerializeField] private float verticalSpacing = 2f; // ���������� ����� �������
    [SerializeField] private Transform initialPosition; // ��������� ������� ������� �����
    [SerializeField] private int maxFloorsCount = 5; // ������������ ����� ������ (������������ ���������)

    private void Awake()
    {
        if (floors.Count == 0)
        {
            AddFloor();
        }
        UpdateConveyorCount();
        // ������������� �� ��������� ���������
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
            Debug.LogError("ConveyorFloorManager: floorPrefab �� ��������!");
            return;
        }
        if (floors.Count >= maxFloorsCount)
        {
            Debug.LogWarning("ConveyorFloorManager: ���������� ������������ ����� ������!");
            return;
        }

        // ����� ������ ����� � ������������ ���������
        Vector3 spawnPosition = initialPosition.position + new Vector3(0f, -verticalSpacing * floors.Count, 0f);
        GameObject newFloor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity, transform);
        ConveyorFloor newConveyorFloor = newFloor.GetComponent<ConveyorFloor>();
        if (newConveyorFloor == null)
        {
            Debug.LogError("ConveyorFloorManager: floorPrefab �� �������� ��������� ConveyorFloor!");
            Destroy(newFloor);
            return;
        }

        floors.Add(newConveyorFloor);

        // �������������� ��� ������������ ������
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