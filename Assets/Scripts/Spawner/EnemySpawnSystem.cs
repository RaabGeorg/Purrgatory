using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
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
                
                float3 randomOffset = new float3(UnityEngine.Random.Range(-10, 10), 1, UnityEngine.Random.Range(-10, 10));
                ecb.SetComponent(newEnemy, LocalTransform.FromPosition(randomOffset));
                
                spawner.ValueRW.NextSpawnTime = currentTime + spawner.ValueRO.SpawnInterval;
            }
        }
    }
}