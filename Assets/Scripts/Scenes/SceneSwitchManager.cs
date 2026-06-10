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

    private IEnumerator StartGameRoutine()
    {
        isTransitioning = true;
        gameStarted = true;
        
        yield return SceneManager.LoadSceneAsync(coreScene, LoadSceneMode.Single);
        
        yield return SceneManager.LoadSceneAsync(hellScene, LoadSceneMode.Additive);
        currentLevel = hellScene;
        
        yield return StartCoroutine(FinalizeSceneLoad(hellScene, "SpawnPoint_Hell"));
        
        isTransitioning = false;
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

            
            float timeout = 2.0f; // Give up after 2 seconds to prevent infinite loops
            float timer = 0f;
            
            while (!query.HasSingleton<Components.PlayerTag>())
            {
                timer += Time.deltaTime;
                if (timer > timeout)
                {
                    Debug.LogError("<color=red>[Spawn Debug] TIMEOUT: Player Entity never appeared in the ECS World!</color>");
                    yield break;
                }
                yield return null; // Wait for the next Unity frame
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
}