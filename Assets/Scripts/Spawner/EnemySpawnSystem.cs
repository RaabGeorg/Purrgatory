using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Components;

public struct EnemyTag : IComponentData { }

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerTag>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;

        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        float maxTeleportDistanceSq = 40f * 40f; 

        var teleportRandom = Unity.Mathematics.Random.CreateFromIndex((uint)(currentTime * 1000f) + 1);

        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<EnemyTag>())
        {
            if (math.distancesq(transform.ValueRO.Position, playerPos) > maxTeleportDistanceSq)
            {
                float angle = teleportRandom.NextFloat(0f, math.PI * 2f);
                float distance = teleportRandom.NextFloat(20f, 40f);
                
                float3 newPos = playerPos + new float3(math.cos(angle) * distance, 0f, math.sin(angle) * distance);
                newPos.y = 1f;

                transform.ValueRW.Position = newPos;
            }
        }

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            if (currentTime >= spawner.ValueRO.NextSpawnTime)
            {
                Entity newEnemy = ecb.Instantiate(spawner.ValueRO.Enemy);
                ecb.AddComponent<EnemyTag>(newEnemy);
                
                ref var rand = ref spawner.ValueRW.Random;

                float angle = rand.NextFloat(0f, math.PI * 2f);
                float distance = rand.NextFloat(50f, 80f);
                
                float3 spawnPos = playerPos + new float3(math.cos(angle) * distance, 0f, math.sin(angle) * distance);
                spawnPos.y = 1f;
                
                ecb.SetComponent(newEnemy, LocalTransform.FromPosition(spawnPos));
                
                spawner.ValueRW.NextSpawnTime = currentTime + spawner.ValueRO.SpawnInterval;
            }
        }
    }
}