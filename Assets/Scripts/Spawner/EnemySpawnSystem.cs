using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Components;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        float currentTime = (float)SystemAPI.Time.ElapsedTime;

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            if (currentTime >= spawner.ValueRO.NextSpawnTime)
            {
                Entity newEnemy = ecb.Instantiate(spawner.ValueRO.Enemy);
                
                ref var rand = ref spawner.ValueRW.Random;

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