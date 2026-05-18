using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Explicitly require the singleton to prevent OnUpdate running before it's ready
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        // Cache the elapsed time once for the entire frame execution
        float currentTime = (float)SystemAPI.Time.ElapsedTime;

        // Use RefRW because we are modifying both NextSpawnTime and RandomState
        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            if (currentTime >= spawner.ValueRO.NextSpawnTime)
            {
                Entity newEnemy = ecb.Instantiate(spawner.ValueRO.Enemy);
                
                // Pull the random state reference to mutate the seed
                ref var rand = ref spawner.ValueRW.Random;

                // Unity.Mathematics uses NextFloat(min, max)
                float3 randomOffset = new float3(
                    rand.NextFloat(20f, 40f), 
                    2f, 
                    rand.NextFloat(20f, 40f)
                );
                
                ecb.SetComponent(newEnemy, LocalTransform.FromPosition(randomOffset));
                
                spawner.ValueRW.NextSpawnTime = currentTime + spawner.ValueRO.SpawnInterval;
            }
        }
    }
}