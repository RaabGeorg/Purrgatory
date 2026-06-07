using UnityEngine;
using UnityEngine.InputSystem; 

public class DeathScreen : MonoBehaviour
{
    public GameObject container;
    public static bool isPaused { get; private set; }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pause();
        }
    }

    public void Pause()
    {
        SetPaused(true);
        container.SetActive(true);
    }
    
    
    public void MainMenuButton()
    {
        SetPaused(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");        
    }
    public static void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0 : 1;
    }
}