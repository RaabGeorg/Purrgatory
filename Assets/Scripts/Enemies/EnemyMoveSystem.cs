using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems; // Required for physics system groups
using Components;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] // 1. Sync with physics timestep
[UpdateBefore(typeof(PhysicsSystemGroup))] // 2. Feed velocity before the solver runs
public partial struct EnemyMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        
        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (velocity, transform, movement) in 
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RefRO<EnemyMovementData>>()
                     .WithNone<MarkedForExecution, EngagementRange, ApplyKnockback>().WithAll<Enemy>()) 
        {
            float3 vectorToPlayer = playerPos - transform.ValueRO.Position;
            vectorToPlayer.y = 0f; 
            
            if (math.lengthsq(vectorToPlayer) < 0.001f) continue; 
            
            float3 direction = math.normalize(vectorToPlayer);
            float3 targetVelocity = direction * movement.ValueRO.Speed;
            
            float3 currentVel = velocity.ValueRO.Linear;
            currentVel.x = math.lerp(currentVel.x, targetVelocity.x, dt * 5f);
            currentVel.z = math.lerp(currentVel.z, targetVelocity.z, dt * 5f);
            
            velocity.ValueRW.Linear = currentVel;
            velocity.ValueRW.Angular = float3.zero;
    
            float angle = math.atan2(direction.x, direction.z);
            transform.ValueRW.Rotation = quaternion.RotateY(angle - math.radians(180));
        }
    }
}