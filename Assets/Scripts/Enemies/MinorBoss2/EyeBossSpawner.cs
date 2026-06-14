using System.Collections;
using UnityEngine;

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
        
    }
    
}