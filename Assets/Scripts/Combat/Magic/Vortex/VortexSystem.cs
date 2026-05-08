using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Components;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial struct VortexSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        
        foreach (var (vortex, vortexTransform) in
                 SystemAPI.Query<RefRO<Vortex>, RefRO<LocalTransform>>())
        {
            
            float3 vortexPos = vortexTransform.ValueRO.Position;
            
            foreach (var (enemyTransform,physicsVelocity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>>()
                         .WithAll<Enemy>())
            {
                
                float3 enemyPos = enemyTransform.ValueRO.Position;

                float dist = math.distance(vortexPos, enemyPos);

                if (dist > vortex.ValueRO.Radius)
                    continue;
                float3 dir = math.normalizesafe(vortexPos - enemyPos);
                
                physicsVelocity.ValueRW.Linear +=
                    dir * vortex.ValueRO.PullStrength * dt;
            }
        }
    }
}
