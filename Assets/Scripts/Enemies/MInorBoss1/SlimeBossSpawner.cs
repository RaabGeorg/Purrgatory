using System.Collections;
using UnityEngine;

public class SlimeBossSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Slime Boss prefab (must have SlimeController attached).")]
    public GameObject slimeBossPrefab;

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

        GameObject boss = Instantiate(slimeBossPrefab, pos, rot);

    }

}
