using System;
using System.Collections.Generic;
using UnityEngine;

public class chunkManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject chunkPrefab;

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    private int chunkSize = 80;
    [SerializeField] private int viewDistance = 1;

    void Update()
    {
        int currentX = Mathf.RoundToInt(player.transform.position.x / 80);
        int currentZ = Mathf.RoundToInt(player.transform.position.z / 80);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int targetCoordinate = new Vector2Int(x + currentX, z + currentZ);

                if (!activeChunks.ContainsKey(targetCoordinate))
                {
                    RecycleOrCreate(targetCoordinate); 
                }
            }
        }

        Cleanup(currentX, currentZ);
    }

    private void RecycleOrCreate(Vector2Int target)
    {
        GameObject chunk;
        Vector3Int coordinates = new Vector3Int(target.x * chunkSize, 0, target.y * chunkSize);

        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
            chunk.transform.position = coordinates;
            chunk.SetActive(true);
        }
        else
        {
            chunk = Instantiate(chunkPrefab, coordinates, Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0));
            chunk.transform.parent = this.transform;
        }

        activeChunks.Add(target, chunk);
    }

    private void Cleanup(int x, int z)
    {
        List<Vector2Int> toBeRemoved = new List<Vector2Int>();

        foreach (var chunk in activeChunks) 
        { 
            if (Mathf.Abs(chunk.Key.x - x) > viewDistance || Mathf.Abs(chunk.Key.y - z) > viewDistance)    
            {
                toBeRemoved.Add(chunk.Key);
            }
        }

        foreach (var key in toBeRemoved) 
        {
            GameObject chunk = activeChunks[key];

            chunk.SetActive(false);
            chunkPool.Enqueue(chunk);
            activeChunks.Remove(key);
        }
    }
}
