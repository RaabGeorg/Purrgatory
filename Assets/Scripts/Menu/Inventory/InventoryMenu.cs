using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{
    public GameObject container;
    public StatsShown updaterStats;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (PauseLogic.PauseGame("Inventory")) 
            {
                updaterStats.UpdateStatsUI();
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
}
