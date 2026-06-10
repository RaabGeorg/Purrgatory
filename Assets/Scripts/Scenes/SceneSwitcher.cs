using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitchController : MonoBehaviour
{
    [Header("Scenes")]
    public string coreSceneName = "Core_Scene";
    public string hellSceneName = "Level_Shell";
    public string heavenSceneName = "Level_Heaven";
    
    [Header("Player Reference")]
    public GameObject player;
    public Transform playerStartPosition;
    
    [Header("Level Entry Points")]
    public Transform hellEntryPoint;
    public Transform heavenEntryPoint;
    
    private Scene hellScene;
    private Scene heavenScene;
    private bool isHellActive = true; // Start with Hell active
    
    void Start()
    {
        // Get scene references
        hellScene = SceneManager.GetSceneByName(hellSceneName);
        heavenScene = SceneManager.GetSceneByName(heavenSceneName);
        
        // Initially activate Hell, deactivate Heaven
        SetSceneActive(hellScene, true);
        SetSceneActive(heavenScene, false);
        
        // Position player at Hell entry
        if (hellEntryPoint != null)
            player.transform.position = hellEntryPoint.position;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchLevels();
        }
    }
    
    void SwitchLevels()
    {
        if (isHellActive)
            SwitchToHeaven();
        else
            SwitchToHell();
    }
    
    void SwitchToHeaven()
    {
        StartCoroutine(SmoothSwitch(heavenScene, hellScene, heavenEntryPoint, false));
        isHellActive = false;
    }
    
    void SwitchToHell()
    {
        StartCoroutine(SmoothSwitch(hellScene, heavenScene, hellEntryPoint, true));
        isHellActive = true;
    }
    
    IEnumerator SmoothSwitch(Scene activateScene, Scene deactivateScene, Transform entryPoint, bool isHell)
    {
        // Optional: Add fade effect
        yield return StartCoroutine(FadeOut());
        
        // Deactivate current level
        SetSceneActive(deactivateScene, false);
        
        // Activate new level
        SetSceneActive(activateScene, true);
        
        // Move player to entry point
        if (entryPoint != null)
        {
            player.transform.position = entryPoint.position;
            
            // Also reset velocity if player has Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = Vector3.zero;
        }
        
        // Optional: Trigger any level-specific setup
        OnLevelChanged(isHell);
        
        // Optional: Fade in
        yield return StartCoroutine(FadeIn());
    }
    
    void SetSceneActive(Scene scene, bool active)
    {
        if (!scene.isLoaded)
        {
            Debug.LogWarning($"Scene {scene.name} not loaded!");
            return;
        }
        
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(active);
        }
    }
    
    void OnLevelChanged(bool isHell)
    {
        // Update UI, camera settings, etc.
        Debug.Log($"Switched to {(isHell ? "Hell" : "Heaven")}");
        
        // Example: Change background music
        // AudioManager.Instance.PlayLevelMusic(isHell);
        
        // Example: Update UI elements
        // UIManager.Instance.UpdateLevelIndicator(isHell);
    }
    
    IEnumerator FadeOut()
    {
        // Simple fade - you can implement CanvasGroup fade here
        yield return null;
    }
    
    IEnumerator FadeIn()
    {
        yield return null;
    }
}