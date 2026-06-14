using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Canvas menu;
    public Canvas meta;

    public void OnStartButtonClicked()
    {
        Time.timeScale = 1;
        PauseLogic.isPaused = false;
        if (SceneSwitchManager.Instance != null)
        {
            SceneSwitchManager.Instance.StartGame();
        }
    }
    
    public void PanelSwap()
    {
        if (menu.gameObject.activeSelf)
        {
            menu.gameObject.SetActive(false);
            meta.gameObject.SetActive(true);
        }
        else 
        {
            menu.gameObject.SetActive(true);
            meta.gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}