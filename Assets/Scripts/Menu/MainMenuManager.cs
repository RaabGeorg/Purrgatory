using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    public Canvas menu;
    public Canvas meta;
    public TMP_Dropdown dropdown;

    public void OnStartButtonClicked()
    {
        GameData.Weapon = dropdown.options[dropdown.value].text;
        GameData.Yallah = 1;
        GameData.HasSpawnedSlimeBoss  = false;
        GameData.HasSpawnedEyeBoss = false;
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