using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs a spawnear")]
    public GameObject[] prefabs;

    [Header("Cantidad máxima por prefab (debe coincidir con el orden en 'prefabs')")]
    public int[] maxCounts;

    [Header("Parámetros de spawn")]
    public float minDistanceBetweenObjects = 3f;

    [Header("Zona de spawn (circular)")]
    public Vector3 spawnCenter = Vector3.zero;
    public float spawnRadius = 50f;

    [Header("Capa de colisión (opcional)")]
    public LayerMask collisionMask;

    private Dictionary<GameObject, List<GameObject>> activeInstances = new();

    void Start()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            int prefabMax = maxCounts[i];

            activeInstances[prefab] = new List<GameObject>();
            SpawnUntilLimit(prefab, prefabMax);
        }
    }

    void Update()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            int prefabMax = maxCounts[i];

            activeInstances[prefab].RemoveAll(item => item == null);

            if (activeInstances[prefab].Count < prefabMax)
            {
                SpawnUntilLimit(prefab, prefabMax);
            }
        }
    }

    void SpawnUntilLimit(GameObject prefab, int max)
    {
        int attempts = 0;
        int maxAttempts = 1000;

        while (activeInstances[prefab].Count < max && attempts < maxAttempts)
        {
            Vector3 position = GetRandomCircularPosition();

            if (IsPositionValid(position, prefab))
            {
                GameObject newObj = Instantiate(prefab, position, Quaternion.identity);
                activeInstances[prefab].Add(newObj);
            }

            attempts++;
        }
    }

    Vector3 GetRandomCircularPosition()
    {
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        float x = spawnCenter.x + circle.x;
        float z = spawnCenter.z + circle.y;
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
        return new Vector3(x, y, z);
    }

    bool IsPositionValid(Vector3 pos, GameObject prefab)
    {
        foreach (var other in activeInstances[prefab])
        {
            if (other != null && Vector3.Distance(pos, other.transform.position) < minDistanceBetweenObjects)
                return false;
        }

        if (Physics.OverlapSphere(pos, minDistanceBetweenObjects, collisionMask).Length > 0)
            return false;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnCenter, spawnRadius);
    }
}
