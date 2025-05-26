using UnityEngine;

public class Settings : MonoBehaviour
{
    public enum GraphicsQuality { Low, Medium, High}

    public static GraphicsQuality graphicsQuality = GraphicsQuality.Medium;

    public static float fov = 60f;
    public static float sens = 150f;
    public static float sound = 0.01f;
}
