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

        float doubleSpawnChance = GameModifiers.Instance.GetDoubleEggSpawnChance();
        int eggCount = UnityEngine.Random.value < doubleSpawnChance ? 2 : 1;

        for (int i = 0; i < eggCount; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            GameObject eggObj = Instantiate(_eggPrefab, spawnPosition + offset, Quaternion.identity, transform);
            if (eggObj.TryGetComponent(out IInitializableEgg egg))
            {
                egg.Initialize(eggData);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_spawnLineStart.position, _spawnLineEnd.position);
    }
}