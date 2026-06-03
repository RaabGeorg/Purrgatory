using Unity.Entities;
using Components;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct KnockBackSystem : ISystem
{
    private EntityQuery _playerQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        
        //find player tag
        _playerQuery = SystemAPI.QueryBuilder().WithAll<PlayerTag, LocalTransform>().Build();
        state.RequireForUpdate(_playerQuery);
    }
    
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
        
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        //gte player position for source of knockback
        var playerTransform = _playerQuery.GetSingleton<LocalTransform>();
        float3 playerPosition = playerTransform.Position;

        foreach (var (velocity, transform, entity) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>>()
                     .WithAll<ApplyKnockback>()
                     .WithEntityAccess())
        {
            //calculate directly away from that position
            float3 knockbackDir = transform.ValueRO.Position - playerPosition;
            knockbackDir.y = 0f; 
            
            if (math.lengthsq(knockbackDir) > 0.001f)
            {
                float3 normalizedDir = math.normalize(knockbackDir);
                velocity.ValueRW.Linear = normalizedDir * 12.0f;
            }
            else
            {
                //fallback if the somehow are on the same position
                velocity.ValueRW.Linear = new float3(0, 0, 12.0f);
            }
            
            ecb.RemoveComponent<ApplyKnockback>(entity);
        }
    } 
}