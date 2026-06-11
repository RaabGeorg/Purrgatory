using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    
    public void OnStartButtonClicked()
    {
        Time.timeScale = 1;
        PauseLogic.isPaused = false;
        if (SceneSwitchManager.Instance != null)
        {
            SceneSwitchManager.Instance.StartGame();
        }
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}