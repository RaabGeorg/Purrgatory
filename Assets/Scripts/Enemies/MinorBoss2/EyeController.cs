using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EyeController : MonoBehaviour
{
    [Header("Fog Settings")]
    public Color fogColor = new Color(0.1f, 0f, 0.15f, 1f);
    public FogMode fogMode = FogMode.ExponentialSquared;
    public float fogDensity = 0.04f;

    [Header("Fade")]
    public float fadeInDuration  = 2f;
    public float fadeOutDuration = 3f;
    
    private bool isDying = false;

  
    void OnEnable()
    {
       
        SceneManager.activeSceneChanged += OnSceneActivated;
        
        ApplyFog();
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneActivated;
        RenderSettings.fog = false;
    }
    
    void OnSceneActivated(Scene old, Scene next)
    {
        SceneManager.activeSceneChanged -= OnSceneActivated;
        ApplyFog();
    }
    
    public void ApplyFog()
    {
        StopAllCoroutines();

        RenderSettings.fog      = true;
        RenderSettings.fogMode  = fogMode;
        RenderSettings.fogColor = new Color(fogColor.r, fogColor.g, fogColor.b, 0f);
        RenderSettings.fogDensity = 0f;

        StartCoroutine(FadeFogDensity(0f, fogDensity, fadeInDuration));
    }
    
    public void Die()
    {
        if (isDying) return;
        isDying = true;
        StartCoroutine(FadeOutAndDestroy());
    }
    
    IEnumerator FadeFogDensity(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            RenderSettings.fogDensity = Mathf.Lerp(from, to, t);
            RenderSettings.fogColor   = Color.Lerp(
                new Color(fogColor.r, fogColor.g, fogColor.b, 0f), fogColor, t);
            yield return null;
        }
        RenderSettings.fogDensity = to;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float startDensity = RenderSettings.fogDensity;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, 0f, t);
            RenderSettings.fogColor   = Color.Lerp(fogColor,
                new Color(fogColor.r, fogColor.g, fogColor.b, 0f), t);
            yield return null;
        }

        RenderSettings.fog = false;
        Destroy(gameObject);
    }
}
