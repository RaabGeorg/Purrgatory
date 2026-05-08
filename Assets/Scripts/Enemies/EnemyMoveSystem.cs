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

        foreach (var (velocity, transform, enemy) in 
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>, RefRO<EnemyData>>())
        {
            float3 direction = math.normalize(playerPos - transform.ValueRO.Position);
            float3 targetVelocity = direction * enemy.ValueRO.Speed;

            // Vortex Velocity beibehalten — nur überschreiben wenn kein Vortex zieht
            float3 current = velocity.ValueRW.Linear;
            velocity.ValueRW.Linear = math.lerp(current, targetVelocity, dt * 5f);
        }
    }
}