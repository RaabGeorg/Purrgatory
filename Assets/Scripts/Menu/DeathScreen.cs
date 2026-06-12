using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject container;
    
    public static bool IsDead { get; set; }

    private void Awake()
    {
        IsDead = false; 
    }

    private void OnEnable()
    {
        GameEvents.OnHealthChanged += CheckDeathCondition;
    }

    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= CheckDeathCondition;
    }

    private void CheckDeathCondition(float currentHealth)
    {
        if (currentHealth <= 0f && !IsDead)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        IsDead = true;
        
        PauseLogic.PauseGame("DeathScreen"); 
        
        container.SetActive(true);
    }

    public void MainMenuButton()
    {
        IsDead = false;
        
        PauseLogic.SetPaused(false);
        PauseLogic.who = null;
        
        if (SceneSwitchManager.Instance != null)
        {
            SceneSwitchManager.Instance.LoadMainMenu("Main Menu");
        }
        else
        {
            Debug.LogError("[DeathScreen] SceneSwitchManager Instance is missing!");
        }
    }

    public void QuitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}