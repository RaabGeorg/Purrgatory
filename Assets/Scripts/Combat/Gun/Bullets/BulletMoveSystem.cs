using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct BulletMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (physicsVelocity, speed) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<Speed>>()
                     .WithAll<BulletTag>())
        {
            physicsVelocity.ValueRW.Linear = speed.ValueRO.Value;
        }
    }
}