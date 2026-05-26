using Unity.Entities;
using Components;
using Unity.Mathematics;
using Unity.Physics;

public partial struct KnockBackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (velocity, entity) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>>()
                     .WithAll<ApplyKnockback>()
                     .WithEntityAccess())
        {
            float3 currentVelocity = velocity.ValueRO.Linear;
            
            float3 reverseDirection = -currentVelocity;
            reverseDirection.y = 0f;
            
            if (math.lengthsq(reverseDirection) > 0.01f)
            {
                float3 normalizedDir = math.normalize(reverseDirection);
                
                velocity.ValueRW.Linear = normalizedDir * 12.0f;
            }
            else
            {
                velocity.ValueRW.Linear = new float3(0, 0, 5f);
            }
            ecb.RemoveComponent<ApplyKnockback>(entity);
        }
    } 
}