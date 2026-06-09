using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject container;
    
    // Kept static if other scripts (like player input) need to check if the player is dead
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

        PauseLogic.PauseGame("DeathScreen"); 
        
        container.SetActive(true);
    }

    // public void ResumeButton()
    // {
    //     IsDead = false;
    //     PauseLogic.PauseGame("DeathScreen");
    //     container.SetActive(false);
    // }

    public void MainMenuButton()
    {
        IsDead = false;
        PauseLogic.PauseGame("DeathScreen");
        SceneManager.LoadScene("Main Menu");        
    }

    public void QuitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}