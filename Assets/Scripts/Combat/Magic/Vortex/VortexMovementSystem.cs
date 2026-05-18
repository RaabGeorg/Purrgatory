using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Components;
[BurstCompile]
public partial struct VortexMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (movement, transform) in
                 SystemAPI.Query<RefRW<VortexMovement>,
                     RefRW<LocalTransform>>().WithNone<MarkedForExecution,Executed>())
        {
            movement.ValueRW.Time += dt * movement.ValueRO.Speed;

            float t = movement.ValueRO.Time;

            float x = math.cos(t) * movement.ValueRO.RadiusX;
            float z = math.sin(t) * movement.ValueRO.RadiusZ;

            transform.ValueRW.Position =
                movement.ValueRO.Center + new float3(x, 0, z);
        }
    }
}
