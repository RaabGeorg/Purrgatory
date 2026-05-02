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
        
        foreach (var (velocity, transform, enemy) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>, RefRO<EnemyData>>())
        {
            float3 direction = math.normalize(playerPos - transform.ValueRO.Position);
    
            // Instead of changing position, we set the Linear velocity
            // This allows the physics engine to resolve "bumps" between enemies
            velocity.ValueRW.Linear = direction * enemy.ValueRO.Speed;
        }
    }
}