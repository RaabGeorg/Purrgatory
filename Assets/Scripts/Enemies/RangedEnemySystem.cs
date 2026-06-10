using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems; // Required for FixedStep
using Components;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] // Sync with physics step
[UpdateBefore(typeof(PhysicsSystemGroup))] // Execute before solver runs
public partial struct RangedEnemySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        foreach (var (velocity, transform, movement, weapon, target, range) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RefRO<EnemyMovementData>, RefRW<Weapon>, RefRW<WeaponTarget>, RefRO<EngagementRange>>()
                     .WithAll<Enemy, ActiveSceneEntity>()
                     .WithNone<MarkedForExecution, Executed>())
        {
            target.ValueRW.Value = playerPosition;
            
            float3 vectorToPlayer = playerPosition - transform.ValueRO.Position;
            vectorToPlayer.y = 0f; 
            
            float distanceSq = math.lengthsq(vectorToPlayer);
            if (distanceSq < 0.001f) continue;
            
            float3 direction = math.normalize(vectorToPlayer);
            float3 currentVel = velocity.ValueRO.Linear;

            //enemy looks at player
            float angle = math.atan2(direction.x, direction.z);
            transform.ValueRW.Rotation = quaternion.RotateY(angle - math.radians(180));

            //check player range
            if (distanceSq > range.ValueRO.Value * range.ValueRO.Value)
            {
                //out of range
                float3 targetVelocity = direction * movement.ValueRO.Speed;
                currentVel.x = math.lerp(currentVel.x, targetVelocity.x, deltaTime * 5f);
                currentVel.z = math.lerp(currentVel.z, targetVelocity.z, deltaTime * 5f);
                
                weapon.ValueRW.IsFiring = false;
            }
            else
            {
                //in range
                currentVel.x = math.lerp(currentVel.x, 0f, deltaTime * 5f);
                currentVel.z = math.lerp(currentVel.z, 0f, deltaTime * 5f);
                
                weapon.ValueRW.IsFiring = true;
            }

            velocity.ValueRW.Linear = currentVel;
            
            // 4. Explicitly kill angular velocity to stop the prefab from rolling
            velocity.ValueRW.Angular = float3.zero;
        }
    }
}