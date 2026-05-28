using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class chunkManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject chunkPrefab;

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    [SerializeField] private int chunkSize = 80;
    [SerializeField] private int viewDistance = 1;

    void Update()
    {
        int currentX = Mathf.RoundToInt(player.transform.position.x / chunkSize);
        int currentZ = Mathf.RoundToInt(player.transform.position.z / chunkSize);

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
        float yOffset = ChunkRandom.GetYOffset(target.x, target.y);
        Vector3 coordinates = new Vector3(target.x * chunkSize, yOffset, target.y * chunkSize);

        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
            chunk.transform.position = coordinates;
            chunk.transform.rotation = Quaternion.Euler(0, ChunkRandom.GetRotation(target.x, target.y), 0);
            chunk.SetActive(true);
        }
        else
        {
            chunk = Instantiate(chunkPrefab, coordinates, Quaternion.Euler(0, ChunkRandom.GetRotation(target.x, target.y), 0));
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
