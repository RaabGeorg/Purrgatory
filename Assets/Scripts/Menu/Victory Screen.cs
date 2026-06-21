using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject container;
    
    public static bool IsVictory { get; private set; }

    private void Awake()
    {
        IsVictory = false; 
        MainBossBridge.isBossDead = false; 
    }

    private void Update()
    {
        if (MainBossBridge.isBossDead && !IsVictory)
        {
            TriggerVictory();
        }
    }

    private void TriggerVictory()
    {
        IsVictory = true;
        PauseLogic.PauseGame("VictoryScreen"); 
        
        container.SetActive(true);
    }

    public void MainMenuButton()
    {
        SaveSouls();
        
        IsVictory = false;
        MainBossBridge.isBossDead = false;
        
        PauseLogic.SetPaused(false);
        PauseLogic.who = null; 
        
        if (SceneSwitchManager.Instance != null)
        {
            SceneSwitchManager.Instance.LoadMainMenu("Main Menu");
        }
        else
        {
            Debug.LogError("[VictoryScreen] SceneSwitchManager Instance is missing!");
        }
    }

    public void QuitButton()
    {
        SaveSouls();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SaveSouls() 
    {
        GameData.Souls += PlayerWallet.Instance.Souls;
        GameData.CondensedSouls += PlayerWallet.Instance.CondensedSouls;
        GameData.Save();
    }
}