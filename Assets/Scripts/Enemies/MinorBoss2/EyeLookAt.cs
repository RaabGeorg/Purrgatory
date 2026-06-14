using UnityEngine;

/// <summary>
/// Rotates the eye (or a child pupil object) to always face the player.
/// Works for both 2D and 3D top-down games.
/// </summary>
public class EyeLookAt : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";

    [Tooltip("The object to rotate. Assign a child pupil/iris transform, " +
             "or leave empty to rotate this GameObject.")]
    public Transform eyeRotator;

    [Header("Settings")]
    [Tooltip("How fast the eye tracks the player.")]
    public float rotationSpeed = 8f;

    [Tooltip("True = 3D top-down (Y-axis rotation). False = 2D top-down (Z-axis rotation).")]
    public bool is3D = false;

    // ─────────────────────────────────────────────────────────────
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

        // Grab laser script to check if currently firing
        eyeLaser = GetComponent<EyeLaser>();
    }

    void Update()
    {
        if (player == null) return;

        // Freeze rotation while the laser is firing (lock-on or fire phase)
        if (eyeLaser != null && eyeLaser.IsShooting) return;

        Vector3 dir = player.position - eyeRotator.position;

        if (is3D)
        {
            // 3D top-down: rotate on Y axis
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            eyeRotator.rotation = Quaternion.Slerp(
                eyeRotator.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // 2D top-down: rotate on Z axis
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
            eyeRotator.rotation = Quaternion.Slerp(
                eyeRotator.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}
