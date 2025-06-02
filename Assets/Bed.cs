using UnityEngine;

public class Bed : MonoBehaviour
{
    // Offset
    public Vector3 respawnOffset = Vector3.up;

    public void SaveBed()
    {
        // Punto de respawn
        Vector3 spawnPos = transform.position + respawnOffset;

        PlayerPrefs.SetFloat("SpawnX", spawnPos.x);
        PlayerPrefs.SetFloat("SpawnY", spawnPos.y);
        PlayerPrefs.SetFloat("SpawnZ", spawnPos.z);
        PlayerPrefs.Save();
    }
}
