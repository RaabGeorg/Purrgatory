using UnityEngine;

public class LevelEntryPoint : MonoBehaviour
{
    [Tooltip("The name of the scene the player is coming FROM to spawn here.")]
    public string SourceSceneName; 

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}