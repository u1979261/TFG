using UnityEngine;
using System.Collections;
public class PlayerRespawn : MonoBehaviour
{
    // Punto por defecto si no hay cama guardada
    public Vector3 defaultSpawnPoint = new Vector3(0, 1, 0);

    void Start()
    {
        // Al iniciar, colocamos al jugador en el punto guardado (o el por defecto)
        Vector3 spawnPos = LoadSpawnPoint();
        transform.position = spawnPos;
    }

    // Llama a este método cuando el jugador “muere”
    public void Die()
    {
        // Podrías reproducir animación, pantalla negra, delay, etc.
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        // Espera un segundo (ajusta a tu gusto)
        yield return new WaitForSeconds(1f);

        // Teletransporta al jugador al punto de respawn
        Vector3 spawnPos = LoadSpawnPoint();
        transform.position = spawnPos;

        // Aquí podrías restaurar vida, reproducir animación de “despertar”, etc.
    }

    private Vector3 LoadSpawnPoint()
    {
        if (PlayerPrefs.HasKey("SpawnX"))
        {
            float x = PlayerPrefs.GetFloat("SpawnX");
            float y = PlayerPrefs.GetFloat("SpawnY");
            float z = PlayerPrefs.GetFloat("SpawnZ");
            return new Vector3(x, y, z);
        }
        else
        {
            // Si nunca hizo click en una cama, usamos el punto por defecto
            return defaultSpawnPoint;
        }
    }
}
