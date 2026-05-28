using UnityEngine;
using UnityEngine.InputSystem; 

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pause();
        }
    }

    public void Pause()
    {
        container.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void MainMenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");        
    }
}