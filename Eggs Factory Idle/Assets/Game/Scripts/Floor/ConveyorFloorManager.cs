using UnityEngine;
using System.Collections.Generic;

public class ConveyorFloorManager : MonoBehaviour
{
    [SerializeField] private List<ConveyorFloor> floors = new List<ConveyorFloor>();
    [SerializeField] private List<GameObject> floorPrefabs = new List<GameObject>(); // ������ �������� ������
    [SerializeField] private float verticalSpacing = 2f; // ���������� ����� �������
    [SerializeField] private Transform initialPosition; // ��������� ������� ������� �����
    [SerializeField] private int maxFloorsCount = 5; // ������������ ����� ������

    private bool isSubscribed;
    private int currentPrefabIndex = 0; // ������� ������ �������

    private void Awake()
    {
        if (floorPrefabs == null || floorPrefabs.Count == 0 || initialPosition == null)
        {
            //Debug.LogError("ConveyorFloorManager: floorPrefabs �� ��������� ��� ������ ����, ���� initialPosition �� ���������!");
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
            //Debug.Log("ConveyorFloorManager: �������� �� UpgradeSystem ���������");
        }
        else
        {
            //Debug.LogError("ConveyorFloorManager: UpgradeSystem.Instance �� ���������������!");
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
            //Debug.LogError("ConveyorFloorManager: ������ floorPrefabs ����!");
            return;
        }
        if (floors.Count >= maxFloorsCount)
        {
           //Debug.LogWarning("ConveyorFloorManager: ���������� ������������ ����� ������!");
            return;
        }

        // ����� ������� �� �������� �������
        GameObject selectedPrefab = floorPrefabs[currentPrefabIndex];

        // ����� ������ ����� � ������������ ���������
        Vector3 spawnPosition = initialPosition.position + new Vector3(0f, -verticalSpacing * floors.Count, 0f);
        GameObject newFloor = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);
        ConveyorFloor newConveyorFloor = newFloor.GetComponent<ConveyorFloor>();
        if (newConveyorFloor == null)
        {
            //Debug.LogError("ConveyorFloorManager: ��������� ������ �� �������� ��������� ConveyorFloor!");
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

        // ���������� ������� ������� (����������� �������)
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