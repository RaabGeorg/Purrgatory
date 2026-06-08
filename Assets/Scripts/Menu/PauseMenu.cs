using UnityEngine;
using UnityEngine.InputSystem; 

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (PauseLogic.PauseGame("PauseMenu"))
            {
                ShowHide();
            }
        }
    }

    public void ShowHide()
    {
        if (container.activeSelf)
        {
            container.SetActive(false);
        }
        else
        {
            container.SetActive(true);
        }
    }

    public void ResumeButton()
    {
        PauseLogic.PauseGame("PauseMenu");
        container.SetActive(false);
    }
    
    public void MainMenuButton()
    {
        PauseLogic.PauseGame("PauseMenu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");        
    }
}