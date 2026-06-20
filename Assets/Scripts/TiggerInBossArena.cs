using UnityEngine;

public class TiggerInBossArena : MonoBehaviour
{
    public bool playerInside;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player")) playerInside = true; Debug.Log("ENTER");
    }
}
