using UnityEngine;

public class EyeMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";
 
    [Header("Movement")]
    public float moveSpeed = 4f;
 
    [Tooltip("Stops moving once within this distance from the player.")]
    public float stopDistance = 8f;
    
 
    [Header("Hover Bob")]
    [Tooltip("Set to 0 to disable bobbing.")]
    public float hoverAmplitude = 0.3f;
    public float hoverSpeed = 2f;
    
    private Vector3 startPosition;
    
    
 
    
    public bool IsInShootRange => player != null &&
        Vector3.Distance(transform.position, player.position) <= stopDistance * 1.5f;
    
    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[EyeMovement] No player found with tag: " + playerTag);
        }
 
        startPosition = transform.position;
    }
 
    void Update()
    {
        if (player == null) return;
 
        float dist = Vector3.Distance(transform.position, player.position);
        float bob  = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        
        if (dist > stopDistance)
        {
           
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;
 
            Vector3 target = transform.position + dir * moveSpeed * Time.deltaTime;
 
            
            target.y += bob * Time.deltaTime;
            
            transform.position = Vector3.Lerp(transform.position, target,  Time.deltaTime);
        }
        else
        {
            Vector3 idle = transform.position;
            idle.y = startPosition.y + bob;
            transform.position = Vector3.Lerp(transform.position, idle, Time.deltaTime);

        }
    }
}

