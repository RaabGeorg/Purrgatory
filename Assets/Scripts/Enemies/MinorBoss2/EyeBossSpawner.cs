using System.Collections;
using UnityEngine;

/// <summary>
/// Place in your level scene.
/// After spawnDelay seconds, spawns the Eye Boss prefab.
/// The EyeFogController on the boss handles all fog logic automatically.
/// </summary>
public class EyeBossSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Eye Boss prefab (must have EyeFogController attached).")]
    public GameObject eyeBossPrefab;

    [Tooltip("Where the boss spawns. Leave empty to use this object's position.")]
    public Transform spawnPoint;

    [Header("Timing")]
    [Tooltip("Seconds after scene load before the boss spawns.")]
    public float spawnDelay = 10f;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject boss = Instantiate(eyeBossPrefab, pos, rot);

        Debug.Log("[EyeBossSpawner] Eye Boss spawned — fog activating.");
    }

    // Scene gizmo so you can see the spawn point
    void OnDrawGizmos()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Gizmos.color = new Color(0.6f, 0f, 1f, 0.6f);
        Gizmos.DrawWireSphere(pos, 1f);
        Gizmos.DrawIcon(pos + Vector3.up * 1.5f, "sv_icon_dot4_sml", true);
    }
}