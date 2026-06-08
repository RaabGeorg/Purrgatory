using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{
    public GameObject container;


    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (PauseLogic.PauseGame("Inventory")) 
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
}
