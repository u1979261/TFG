using UnityEngine;

public class Bed : MonoBehaviour
{
    // Offset opcional para que el jugador no spawnee dentro de la cama
    public Vector3 respawnOffset = Vector3.up;

    public void SaveBed()
    {
        // Calculamos el punto de respawn
        Vector3 spawnPos = transform.position + respawnOffset;
        // Guardamos en PlayerPrefs
        PlayerPrefs.SetFloat("SpawnX", spawnPos.x);
        PlayerPrefs.SetFloat("SpawnY", spawnPos.y);
        PlayerPrefs.SetFloat("SpawnZ", spawnPos.z);
        PlayerPrefs.Save();

        Debug.Log("Nuevo punto de respawn guardado en: " + spawnPos);
        // Aquí podrías reproducir una animación de “acostarse” o sonido
    }
}
