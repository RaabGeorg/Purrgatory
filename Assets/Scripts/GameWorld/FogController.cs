using UnityEngine;

public class FogController : MonoBehaviour
{
    public Color fogColor = Color.gray;
    public float fogDensity = 0.05f;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
}
