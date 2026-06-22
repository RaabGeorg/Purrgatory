using UnityEngine;

public class EyeLookAt : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public string playerTag = "Player";
    
    public Transform eyeRotator;

    [Header("Settings")]
    public float rotationSpeed = 8f;
    
    public bool is3D = false;

    private EyeLaser eyeLaser;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[EyeLookAt] No player found with tag: " + playerTag);
        }

        if (eyeRotator == null)
            eyeRotator = transform;
        
        eyeLaser = GetComponent<EyeLaser>();
    }

    void Update()
    {
        if (player == null) return;

        if (eyeLaser != null && eyeLaser.IsShooting) return;

        Vector3 dir = player.position - eyeRotator.position;

        if (is3D)
        {
          
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            eyeRotator.rotation = Quaternion.Slerp(
                eyeRotator.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
          
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
            eyeRotator.rotation = Quaternion.Slerp(
                eyeRotator.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}
