using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Components;

[BurstCompile]
public partial struct EnemyMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        
        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (velocity, transform, movement) in 
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RefRO<EnemyMovementData>>()
                     .WithNone<MarkedForExecution, EngagementRange, ApplyKnockback>().WithAll<Enemy>()) // Added ApplyKnockback here
        {
            float3 direction = math.normalize(playerPos - transform.ValueRO.Position);
            float3 targetVelocity = direction * movement.ValueRO.Speed;
            float3 current = velocity.ValueRO.Linear;
    
            velocity.ValueRW.Linear = math.lerp(current, targetVelocity, dt * 5f);
    
            float angle = math.atan2(direction.x, direction.z);
            transform.ValueRW.Rotation = quaternion.RotateY(angle - math.radians(180));
        }
    }
}