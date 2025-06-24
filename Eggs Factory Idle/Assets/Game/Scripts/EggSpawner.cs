using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _eggPrefab;
    [SerializeField] private Transform _spawnLineStart;
    [SerializeField] private Transform _spawnLineEnd;

    public void SpawnEgg(EggData eggData)
    {
        if (_eggPrefab == null) return;

        float t = Random.Range(0f, 1f);
        Vector2 spawnPosition = Vector2.Lerp(_spawnLineStart.position, _spawnLineEnd.position, t);

        GameObject eggObj = Instantiate(_eggPrefab, spawnPosition, Quaternion.identity);
        if (eggObj.TryGetComponent(out IInitializableEgg egg))
        {
            egg.Initialize(eggData);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_spawnLineStart.position, _spawnLineEnd.position);
    }
}