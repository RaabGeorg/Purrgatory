using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private Transform playerTransform;

    private string currentLevelName;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void SwitchScene(string targetLevelName, string sourceLevelName)
    {
        if (isTransitioning || currentLevelName == targetLevelName) return;
        StartCoroutine(TransitionRoutine(targetLevelName, sourceLevelName));
    }

    private IEnumerator TransitionRoutine(string targetLevelName, string sourceLevelName)
    {
        isTransitioning = true;

        // 1. Unload the current level. 
        // This instantly destroys all GameObjects and purges the DOTS SubScene entities.
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            Scene oldScene = SceneManager.GetSceneByName(currentLevelName);
            if (oldScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(currentLevelName);
            }
        }

        // 2. Load the target level additively.
        yield return SceneManager.LoadSceneAsync(targetLevelName, LoadSceneMode.Additive);
        
        Scene newScene = SceneManager.GetSceneByName(targetLevelName);
        SceneManager.SetActiveScene(newScene);
        currentLevelName = targetLevelName;

        // 3. Teleport the Player to the designated entry point
        TeleportPlayer(sourceLevelName);

        isTransitioning = false;
    }

    private void TeleportPlayer(string sourceLevelName)
    {
        LevelEntryPoint[] entryPoints = Object.FindObjectsByType<LevelEntryPoint>(FindObjectsSortMode.None);
        foreach (var entry in entryPoints)
        {
            if (entry.SourceSceneName == sourceLevelName)
            {
                // Temporarily disable physics/CharacterController if applicable before moving
                playerTransform.SetPositionAndRotation(entry.transform.position, entry.transform.rotation);
                Physics.SyncTransforms(); 
                return;
            }
        }
        Debug.LogWarning($"No entry point found expecting source: {sourceLevelName}");
    }
}