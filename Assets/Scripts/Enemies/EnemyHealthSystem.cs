using Components;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct EnemyHealthSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithAll<Enemy>().WithNone<DeadTag>().WithEntityAccess())
        {
            if (health.ValueRO.Value <= 0f)
            {
                ecb.AddComponent<DeadTag>(entity);
            }
        }
    }
}