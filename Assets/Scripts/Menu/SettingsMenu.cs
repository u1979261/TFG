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
        Debug.Log((int)Settings.graphicsQuality);
        QualitySettings.SetQualityLevel((int)Settings.graphicsQuality);
    }

    public void SoundControll(float slider)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(slider) * 20);
    }
}
