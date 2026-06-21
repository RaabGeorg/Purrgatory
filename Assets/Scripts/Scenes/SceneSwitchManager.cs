using System.Collections;
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
    public static bool inBossRoom = false;
    
    
    //public getter setter
    public string CurrentLevel => currentLevel;
    public string HeavenScene => heavenScene;

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
        if (gameStarted && !isTransitioning && !inBossRoom)
        {
            if (GameObject.FindGameObjectWithTag("Boss") != null)
            {
                StartCoroutine(BossMsg());
                return;
            }
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
        
        PauseLogic.SetPaused(false);
        PauseLogic.who = null;
        DeathScreen.IsDead = false; 

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

        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            
            if (controller != null)
            {
                controller.enabled = false;
            }
            
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
            
            if (controller != null)
            {
                controller.enabled = true;
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

    private IEnumerator BossMsg() 
    {
        GameObject bossMsg = GameObject.FindGameObjectWithTag("BossMsg");

        if (bossMsg != null)
        {

            var TMProText = bossMsg.GetComponent<TMPro.TextMeshProUGUI>();

            TMProText.enabled = true;

            yield return new WaitForSeconds(2f);

            TMProText.enabled = false;

        }
    }
}