using UnityEngine;
using System.Collections.Generic;

public class EggSpawner : MonoBehaviour, IEggSpawner
{
    [SerializeField] private List<EggData> eggDataList;
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private Transform spawnLineStart;
    [SerializeField] private Transform spawnLineEnd;

    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnEgg();
            timer = spawnInterval;
        }
    }

    public void SpawnEgg()
    {
        if (eggDataList.Count == 0 || eggPrefab == null) return;

        float t = Random.Range(0f, 1f);
        Vector2 spawnPosition = Vector2.Lerp(spawnLineStart.position, spawnLineEnd.position, t);

        GameObject eggObj = Instantiate(eggPrefab, spawnPosition, Quaternion.identity);
        IInitializableEgg egg = eggObj.GetComponent<IInitializableEgg>();
        EggData randomData = eggDataList[Random.Range(0, eggDataList.Count)];
        egg?.Initialize(randomData);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnLineStart.position, spawnLineEnd.position);
    }
}