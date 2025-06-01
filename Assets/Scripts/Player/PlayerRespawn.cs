using UnityEngine;
using System.Collections;
public class PlayerRespawn : MonoBehaviour
{
    public GameObject deathScreenUI;
    // Punto por defecto si no hay cama guardada
    public Vector3 defaultSpawnPoint = new Vector3(768.87323f, 23.6694355f, 372.422333f);

    void Start()
    {
        Vector3 spawnPos = LoadSpawnPoint();
        transform.position = spawnPos;
    }
    public void Die()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        deathScreenUI.SetActive(true);
        // Teletransporta al jugador al punto de respawn
        yield return new WaitForSeconds(0.5f);
        Vector3 spawnPos = LoadSpawnPoint();
        transform.position = spawnPos;
        yield return new WaitForSeconds(2.5f);
        deathScreenUI.SetActive(false);
    }

    private Vector3 LoadSpawnPoint()
    {
        if (PlayerPrefs.HasKey("SpawnX"))
        {
            float x = PlayerPrefs.GetFloat("SpawnX");
            float y = PlayerPrefs.GetFloat("SpawnY");
            float z = PlayerPrefs.GetFloat("SpawnZ");
            return new Vector3(768.87323f, 23.6694355f, 372.422333f);
        }
        else
        {
            return defaultSpawnPoint;
        }
    }
}
