using Components;
using Unity.Entities;



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

        foreach (var (_, entity) in SystemAPI.Query<Enemy>().WithAll<MarkedForExecution, Executed>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
        }
    }
}