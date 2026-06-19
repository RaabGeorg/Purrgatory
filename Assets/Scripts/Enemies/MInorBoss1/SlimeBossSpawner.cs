using System.Collections;
using UnityEngine;

public class SlimeBossSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Eye Boss prefab (with EyeBossHitboxLinker, EyeFogController etc.)")]
    public GameObject slimeBossPrefab;

    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Timing")]
    [Tooltip("How long after scene load before the first spawn.")]
    public float initialDelay = 20f;

    [Header("Spawn Position")]
    [Tooltip("How far from the player the boss can spawn (max).")]
    public float spawnRadius = 24f;

    [Tooltip("Minimum distance from player so it doesn't spawn directly on them.")]
    public float minSpawnDistance = 12f;

    [Tooltip("Height offset above the ground (for 3D top-down).")]
    public float spawnHeight = 2f;

    [Tooltip("How many attempts to find a valid spawn position before giving up.")]
    public int maxSpawnAttempts = 10;
    
    private GameObject currentBoss;
    private bool isWaiting = false;
    
    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[EyeBossSpawner] No player found with tag: " + playerTag);
        }

        StartCoroutine(SpawnLoop());
    }
    
    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);
        
        currentBoss = SpawnBoss();
        
    }
    
    GameObject SpawnBoss()
    {
        if (slimeBossPrefab == null)
        {
            Debug.LogWarning("[EyeBossSpawner] No prefab assigned!");
            return null;
        }

        Vector3 spawnPos = GetSpawnPosition();
        GameObject boss  = Instantiate(slimeBossPrefab, spawnPos, Quaternion.identity);

        Debug.Log("[EyeBossSpawner] Eye Boss spawned at " + spawnPos);
        return boss;
    }
    
    Vector3 GetSpawnPosition()
    {
        if (player == null)
            return transform.position;
        
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            float dist = Random.Range(minSpawnDistance, spawnRadius);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * dist,
                0f,
                Mathf.Sin(angle) * dist
            );

            Vector3 candidate = player.position + offset;
            candidate.y = spawnHeight; 

            return candidate;
        }
        
        return player.position + new Vector3(spawnRadius, spawnHeight, 0f);
    }

}
