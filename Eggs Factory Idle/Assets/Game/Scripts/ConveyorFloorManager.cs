using UnityEngine;
using System.Collections.Generic;

public class ConveyorFloorManager : MonoBehaviour
{
    [SerializeField] private List<ConveyorFloor> floors = new List<ConveyorFloor>();
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private float verticalSpacing = 2f; // Расстояние между этажами
    [SerializeField] private Transform initialPosition;
    [SerializeField] private int maxFloorsCount = 3;
    private void Awake()
    {
        if (floors.Count == 0)
        {
            AddFloor();
        }
        UpdateFloorStates();
    }

    public void AddFloor()
    {
        if (floorPrefab == null)
        {
            Debug.LogError("ConveyorFloorManager: floorPrefab not assigned!");
            return;
        }
        if (floors.Count >= maxFloorsCount)
        {
            return;
        }

        // Спавн нового этажа с вертикальным смещением
        Vector3 spawnPosition = initialPosition.position + new Vector3(0f, -verticalSpacing * floors.Count, 0f);
        GameObject newFloor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity, transform);
        ConveyorFloor newConveyorFloor = newFloor.GetComponent<ConveyorFloor>();
        if (newConveyorFloor == null)
        {
            Debug.LogError("ConveyorFloorManager: floorPrefab missing ConveyorFloor component!");
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
        //Debug.Log($"ConveyorFloorManager: Added floor at position={spawnPosition}, total floors={floors.Count}, mirrored={floors.Count % 2 == 0}");
    }

    private void UpdateFloorStates()
    {
        for (int i = 0; i < floors.Count; i++)
        {
            bool isLast = i == floors.Count - 1;
            floors[i].SetLastFloor(isLast);
            floors[i].SetFloorIndex(i);
            //Debug.Log($"ConveyorFloorManager: Floor {i} at position={floors[i].transform.position}, set to isLastFloor={isLast}");
        }
    }
}