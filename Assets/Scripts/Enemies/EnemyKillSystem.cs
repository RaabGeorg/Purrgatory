using Components;
using Unity.Entities;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct EnemyKillSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }
    
    
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (_,_, entity) in SystemAPI.Query<MarkedForExecution, Executed>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
        }
    }
} 