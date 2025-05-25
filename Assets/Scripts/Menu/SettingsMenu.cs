using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown graphicsQuality;
    public Slider fieldOfView;
    public Slider sens;
    public Slider sound;
    public AudioMixer audioMixer;

    private void Start()
    {
        graphicsQuality.value = (int)Settings.graphicsQuality;
        fieldOfView.value = Settings.fov;
        sens.value = Settings.sens;
        sound.value = Settings.sound;
        ApplyChanges();
    }

    private void Update()
    {
        Settings.graphicsQuality = (Settings.GraphicsQuality)graphicsQuality.value;
        Settings.fov = fieldOfView.value;
        Settings.sens = sens.value;
        Settings.sound = sound.value;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ApplyChanges()
    {
        string qualityName = graphicsQuality.options[graphicsQuality.value].text;

        int levelIndex = System.Array.IndexOf(QualitySettings.names, qualityName);

        if (levelIndex != -1)
        {
            QualitySettings.SetQualityLevel(levelIndex, true); // true = aplica inmediatamente
            Debug.Log("Calidad cambiada a: " + qualityName);
        }
        else
        {
            Debug.LogWarning("No se encontr� el nivel de calidad: " + qualityName);
        }
    }

    public void SoundControll(float slider)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(slider) * 20);
    }
}
