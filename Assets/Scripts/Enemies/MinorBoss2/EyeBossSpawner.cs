using System.Collections;
using UnityEngine;

public class EyeBossSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject eyeBossPrefab;

    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Timing")]
    public float initialDelay = 20f;

    [Header("Spawn Position")]
    public float spawnRadius = 24f;

    
    public float minSpawnDistance = 12f;
    
    public float spawnHeight = 2f;
    
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
        // Wait before first spawn
        yield return new WaitForSeconds(initialDelay);

        if (!GameData.HasSpawnedEyeBoss)
        {
            currentBoss = SpawnBoss();
            Debug.Log("[EyeBossSpawner] Spawned " + currentBoss);
            GameData.HasSpawnedEyeBoss =  true;
            GameData.Save();
        }
    }
    
    GameObject SpawnBoss()
    {
        if (eyeBossPrefab == null)
        {
            Debug.LogWarning("[EyeBossSpawner] No prefab assigned!");
            return null;
        }

        Vector3 spawnPos = GetSpawnPosition();
        GameObject boss  = Instantiate(eyeBossPrefab, spawnPos, Quaternion.identity);

        Debug.Log("[EyeBossSpawner] Eye Boss spawned at " + spawnPos);
        return boss;
    }
    
    Vector3 GetSpawnPosition()
    {
        if (player == null)
            return transform.position;

        // Try to find a valid position within the ring (minDist < pos < maxDist)
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Random angle
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // Random distance between min and max
            float dist = Random.Range(minSpawnDistance, spawnRadius);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * dist,
                0f,
                Mathf.Sin(angle) * dist
            );

            Vector3 candidate = player.position + offset;
            candidate.y = spawnHeight; // fixed height for top-down

            return candidate; // first valid candidate is fine for top-down
        }

        // Fallback: directly beside player
        return player.position + new Vector3(spawnRadius, spawnHeight, 0f);
    }
}