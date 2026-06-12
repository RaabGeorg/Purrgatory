using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Components;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct PlanarMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var velocity in SystemAPI.Query<RefRW<PhysicsVelocity>>().WithAll<PlayerTag>())
        {
            velocity.ValueRW.Linear.y = 0f;
        }
    }
}