using UnityEngine;

public class DayNightCicle : MonoBehaviour
{
    public Transform sun;
    public float daySpeed = 1f;

    [Header("Cycle Settings")]
    public float timeOfDay = 1350f;
    public float cycleDuration = 3000f;
    public float dayStartTime = 750f;
    public float dayEndTime = 2150f;

    [Space]
    public float cycleSpeed = 1f;

    [Header("Lighting Settings")]
    public float dayTimeSunIntensity = 1f;
    public float nightTimeSunIntensity = 0f;

    [Space]
    public float dayTimeAmbientIntensity = 1f;
    public float nightTimeAmbientIntensity = 0.15f;

    [Space]
    public float intensitySpeed = 1f;

    [HideInInspector] public bool isNightTime;
    public Material skyboxMaterial;
    public Color dayTimeSkyColor;
    public Color nightTimeColor;

    private void Start()
    {
        if (!isNightTime)
        {
            sun.GetComponentInChildren<Light>().intensity = dayTimeSunIntensity;
        }
        else
        {
            sun.GetComponentInChildren<Light>().intensity = nightTimeSunIntensity;
        }
    }

    private void Update()
    {
        if (!isNightTime)
        {
            sun.GetComponentInChildren<Light>().intensity = Mathf.Lerp(sun.GetComponentInChildren<Light>().intensity, dayTimeSunIntensity, intensitySpeed * Time.deltaTime);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, dayTimeAmbientIntensity, intensitySpeed * Time.deltaTime);

            if (skyboxMaterial != null && RenderSettings.skybox.HasProperty("_SkyTint"))
            {
                Color currentSkyTint = RenderSettings.skybox.GetColor("_SkyTint");
                RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(currentSkyTint, dayTimeSkyColor, intensitySpeed * Time.deltaTime));
            }
        }
        else
        {
            sun.GetComponentInChildren<Light>().intensity = Mathf.Lerp(sun.GetComponentInChildren<Light>().intensity, nightTimeSunIntensity, intensitySpeed * Time.deltaTime);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, nightTimeAmbientIntensity, intensitySpeed * Time.deltaTime);

            if (skyboxMaterial != null && RenderSettings.skybox.HasProperty("_SkyTint"))
            {
                Color currentSkyTint = RenderSettings.skybox.GetColor("_SkyTint");
                RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(currentSkyTint, nightTimeColor, intensitySpeed * Time.deltaTime));
            }
        }

        if (timeOfDay > cycleDuration)
        {
            timeOfDay = 0;
        }
        if (timeOfDay > dayStartTime && timeOfDay < dayEndTime)
        {
            timeOfDay += cycleSpeed * Time.deltaTime;
        }
        else
        {
            timeOfDay += (cycleSpeed * 2) * Time.deltaTime;
        }

        UpdateLighting();
    }

    public void UpdateLighting()
    {
        sun.localRotation = Quaternion.Euler((timeOfDay * 360 / cycleDuration), 0, 0);
        isNightTime = timeOfDay < dayStartTime || timeOfDay > dayEndTime;

        if (isNightTime)
        {
            RenderSettings.ambientLight = Color.black;
        }
        else
        {
            RenderSettings.ambientLight = Color.white;
        }
    }
}
