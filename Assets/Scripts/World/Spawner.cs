using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs a spawnear")]
    public GameObject[] prefabs;

    [Header("Parámetros de spawn")]
    public int maxCount = 50;
    public float minDistanceBetweenObjects = 3f;

    [Header("Zona de spawn (circular)")]
    public Vector3 spawnCenter = Vector3.zero;
    public float spawnRadius = 50f;

    [Header("Capa de colisión (opcional)")]
    public LayerMask collisionMask;

    private Dictionary<GameObject, List<GameObject>> activeInstances = new();

    void Start()
    {
        foreach (GameObject prefab in prefabs)
        {
            activeInstances[prefab] = new List<GameObject>();
            SpawnUntilLimit(prefab);
        }
    }

    void Update()
    {
        foreach (GameObject prefab in prefabs)
        {
            activeInstances[prefab].RemoveAll(item => item == null);

            if (activeInstances[prefab].Count < maxCount)
            {
                SpawnUntilLimit(prefab);
            }
        }
    }

    void SpawnUntilLimit(GameObject prefab)
    {
        int attempts = 0;
        int maxAttempts = 1000;

        while (activeInstances[prefab].Count < maxCount && attempts < maxAttempts)
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
        // 1. Verifica que no esté demasiado cerca de otros del mismo tipo
        foreach (var other in activeInstances[prefab])
        {
            if (other != null && Vector3.Distance(pos, other.transform.position) < minDistanceBetweenObjects)
                return false;
        }

        // 2. Verifica que no haya colisión con otros objetos
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
