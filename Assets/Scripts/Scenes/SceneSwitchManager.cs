using System.Collections;
using Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneSwitchManager : MonoBehaviour
{
    public static SceneSwitchManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string coreScene = "Core_Scene";
    [SerializeField] private string hellScene = "Level_Hell";
    [SerializeField] private string heavenScene = "Level_Heaven";

    [Header("Input Configuration")]
    [SerializeField] private InputAction toggleLevelAction;

    private string currentLevel = "";
     bool isTransitioning = false;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        toggleLevelAction.Enable();
        toggleLevelAction.performed += OnToggleLevel;
    }

    private void OnDisable()
    {
        toggleLevelAction.Disable();
        toggleLevelAction.performed -= OnToggleLevel;
    }
    
    public void StartGame()
    {
        if (!gameObject.activeInHierarchy) return;

        if (!gameStarted && !isTransitioning)
        {
            StartCoroutine(StartGameRoutine());
        }
    }

    private void OnToggleLevel(InputAction.CallbackContext context)
    {
        // Only allow toggling if the core game is running and no load/unload is active
        if (gameStarted && !isTransitioning)
        {
            StartCoroutine(ToggleLevelRoutine());
        }
    }
    
    public void LoadMainMenu(string mainMenuSceneName = "Main Menu")
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadMainMenuRoutine(mainMenuSceneName));
        }
    }

    private IEnumerator StartGameRoutine()
    {
        isTransitioning = true;
        gameStarted = true;
        
        yield return SceneManager.LoadSceneAsync(coreScene, LoadSceneMode.Single);
        
        yield return SceneManager.LoadSceneAsync(hellScene, LoadSceneMode.Additive);
        currentLevel = hellScene;
        SFXManager.Instance.PlayMusic(SFXManager.Instance.hellBackground);
        
        yield return StartCoroutine(FinalizeSceneLoad(hellScene, "SpawnPoint_Hell"));
        
        // --- ADD THIS HERE ---
        // Ultimate safety net. Regardless of what UI components silently panicked 
        // or locked during the async loading phase, we crush the locks right 
        // before giving the player control.
        PauseLogic.SetPaused(false);
        PauseLogic.who = null;
        DeathScreen.IsDead = false; // Add this if IsDead is still causing ghost UI issues
        // ---------------------

        isTransitioning = false;
        Debug.Log("<color=green>[SceneSwitchManager] Coroutine Step 5: Transition Complete! Locks cleared.</color>");
    }

    private IEnumerator ToggleLevelRoutine()
    {
        isTransitioning = true;

        string levelToUnload = currentLevel;
        string levelToLoad = (currentLevel == hellScene) ? heavenScene : hellScene;
        string spawnPointName = (levelToLoad == hellScene) ? "SpawnPoint_Hell" : "SpawnPoint_Heaven";

        if (!string.IsNullOrEmpty(levelToUnload))
        {
            yield return SceneManager.UnloadSceneAsync(levelToUnload);
        }
        
        yield return SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
        currentLevel = levelToLoad;
        SFXManager.Instance.PlayMusic(levelToLoad == hellScene ? SFXManager.Instance.hellBackground : SFXManager.Instance.heavenBackground);
        
        yield return StartCoroutine(RelocatePlayerRoutine(spawnPointName));
        
        isTransitioning = false;
    }

    private IEnumerator FinalizeSceneLoad(string loadedSceneName, string spawnPointName)
    {
        Scene loadedScene = SceneManager.GetSceneByName(loadedSceneName);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }
        
        yield return StartCoroutine(RelocatePlayerRoutine(spawnPointName));
    }

  private IEnumerator RelocatePlayerRoutine(string spawnPointName)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint == null)
        {
            yield break;
        }

        Vector3 targetPos = spawnPoint.transform.position;

        if (player != null)
        {
            // 1. Move the visual shell immediately
            player.transform.position = targetPos;
            player.transform.rotation = spawnPoint.transform.rotation;

            var em = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = em.CreateEntityQuery(typeof(Components.PlayerTag), typeof(Unity.Transforms.LocalTransform));

            
            float timeout = 2.0f;
            float timer = 0f;
            
            while (!query.HasSingleton<Components.PlayerTag>())
            {
                timer += Time.deltaTime;
                if (timer > timeout)
                {
                    yield break;
                }
                yield return null;
            }
            
            Unity.Entities.Entity playerEntity = query.GetSingletonEntity();
            var playerTransform = em.GetComponentData<Unity.Transforms.LocalTransform>(playerEntity);
            playerTransform.Position = targetPos;
            playerTransform.Rotation = spawnPoint.transform.rotation;
            em.SetComponentData(playerEntity, playerTransform);

            // 3. Clear Momentum
            if (em.HasComponent<Unity.Physics.PhysicsVelocity>(playerEntity))
            {
                em.SetComponentData(playerEntity, new Unity.Physics.PhysicsVelocity());
            }
        }
    }
    private IEnumerator LoadMainMenuRoutine(string mainMenuSceneName)
    {
        isTransitioning = true;
        
        PauseLogic.SetPaused(false);
        PauseLogic.who = null;

        if (!string.IsNullOrEmpty(currentLevel))
        {
            yield return SceneManager.UnloadSceneAsync(currentLevel);
            currentLevel = "";
        }
        
        yield return SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Single);
        SFXManager.Instance.PlayMusic(SFXManager.Instance.menuBackground);
        
        gameStarted = false;
        isTransitioning = false;
    }
}