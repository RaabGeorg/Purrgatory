using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct DotsChunkBrain : ISystem
{
    private NativeHashMap<int2, Entity> activeChunks;
    private NativeList<Entity> pooledChunks;

    public void OnCreate(ref SystemState state)
    {
        activeChunks = new NativeHashMap<int2, Entity>(256, Allocator.Persistent);
        pooledChunks = new NativeList<Entity>(Allocator.Persistent);

        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<ChunkConfigData>();
    }

    public void OnDestroy(ref SystemState state)
    {
        activeChunks.Dispose();
        pooledChunks.Dispose();
    }

    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        var config = SystemAPI.GetSingleton<ChunkConfigData>();

        int currentX = (int)math.round(playerPos.x / config.ChunkSize);
        int currentZ = (int)math.round(playerPos.z / config.ChunkSize);

        // 1. Cleanup Phase (Execute first to populate the pool before generation)
        var toRemove = new NativeList<int2>(Allocator.Temp);

        foreach (var kv in activeChunks)
        {
            int2 coord = kv.Key;
            if (math.abs(coord.x - currentX) > config.ViewDistance ||
                math.abs(coord.y - currentZ) > config.ViewDistance)
            {
                toRemove.Add(coord);
            }
        }

        foreach (var coord in toRemove)
        {
            Entity chunk = activeChunks[coord];

            // Move out of view. We deliberately avoid removing ChunkTag here 
            // to prevent structural changes and maintain maximum performance.
            em.SetComponentData(chunk, LocalTransform.FromPosition(new float3(0f, -9999f, 0f)));
            pooledChunks.Add(chunk);

            activeChunks.Remove(coord);
        }

        toRemove.Dispose();

        // 2. Generation Phase
        for (int x = -config.ViewDistance; x <= config.ViewDistance; x++)
        {
            for (int z = -config.ViewDistance; z <= config.ViewDistance; z++)
            {
                var coord = new int2(currentX + x, currentZ + z);

                if (activeChunks.ContainsKey(coord)) continue;

                Entity chunk;
                if (pooledChunks.Length > 0)
                {
                    // Pop from end of list (O(1) operation)
                    chunk = pooledChunks[pooledChunks.Length - 1];
                    pooledChunks.RemoveAt(pooledChunks.Length - 1);
                }
                else
                {
                    // Fallback to instantiation only when pool is empty
                    var prefabData = SystemAPI.GetSingleton<ChunkPrefabData>();
                    chunk = em.Instantiate(prefabData.Prefab);
                }

                float yOffset = ChunkRandom.GetYOffset(coord.x, coord.y);
                float3 worldPos = new float3(coord.x * config.ChunkSize, yOffset, coord.y * config.ChunkSize);
                em.SetComponentData(chunk, new LocalTransform
                {
                    Position = worldPos,
                    Rotation = quaternion.RotateY(math.radians(ChunkRandom.GetRotation(coord.x, coord.y))),
                    Scale = 1f
                });

                // If it's a recycled chunk, it already has ChunkTag. Just update it.
                // If it's a new instantiation, it needs the component added.
                if (em.HasComponent<ChunkTag>(chunk))
                {
                    em.SetComponentData(chunk, new ChunkTag { Coordinate = coord });
                }
                else
                {
                    em.AddComponentData(chunk, new ChunkTag { Coordinate = coord });
                }

                activeChunks.Add(coord, chunk);
            }
        }
    }
}