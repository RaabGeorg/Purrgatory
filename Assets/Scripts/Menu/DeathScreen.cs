using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject container;
    
    public static bool IsDead { get; private set; }

    private void OnEnable()
    {
        // Subscribe to the centralized event stream
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
        container.SetActive(true);
        
        // Halt physics and frame-rate independent updates
        Time.timeScale = 0f; 
    }

    public void MainMenuButton()
    {
        // Reset time scale before changing scenes to avoid frozen state on reload
        IsDead = false;
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Main Menu");        
    }
}