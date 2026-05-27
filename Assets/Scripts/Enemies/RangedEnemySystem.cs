using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Components;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(WeaponSystem))]
public partial struct RangedEnemySystem : ISystem
{
    //[BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity  playerEntity)) return;
        
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        //We need the entity to have enemy movement, weapon data and engagement range
        foreach (var (velocity, transform, movement, weapon, target, range) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>, RefRO<EnemyMovementData>, RefRW<Weapon>, RefRW<WeaponTarget>, RefRO<EngagementRange>>()
                     .WithAll<Enemy>()
                     .WithNone<MarkedForExecution, Executed>())
        {
            float3 dirToPlayer = playerPosition - transform.ValueRO.Position;
            float distance = math.lengthsq(dirToPlayer);

            target.ValueRW.Value = playerPosition;
            
            //Check if player is too far
            if (distance > range.ValueRO.Value *  range.ValueRO.Value)
            {
                //if player is outta range
                float3 normalizedDir = math.normalize(dirToPlayer);
                float3 targetVelocity = normalizedDir * movement.ValueRO.Speed;
                
                //move to the player
                velocity.ValueRW.Linear = math.lerp(velocity.ValueRO.Linear, targetVelocity, deltaTime * 5f);
                weapon.ValueRW.IsFiring = false;
            }
            else
            {
                //player is in range
                velocity.ValueRW.Linear = math.lerp(velocity.ValueRO.Linear, float3.zero, deltaTime * 5f);
                weapon.ValueRW.IsFiring = true;
            }
        }
        
        
    }
}