using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Components;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial struct PullEffectSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
        // System will skip OnUpdate entirely if 0 or >1 of these entities exist.
        state.RequireForUpdate<PlayerTag>(); 
    }
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        
        // Ensure you have a valid way to retrieve playerEntity (e.g., a Singleton)
        // SystemAPI.GetSingleton<PlayerTag>() is the standard ECS way if only 1 player exists.
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>(); 
        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        var bulletLookup = SystemAPI.GetComponentLookup<BulletTag>(true);
        foreach (var (vortex, vortexTransform, entity) in
                 SystemAPI.Query<RefRO<PullEffect>, RefRO<LocalTransform>>().WithEntityAccess().WithNone<MarkedForExecution,Executed>())
        {
            float3 vortexPos = vortexTransform.ValueRO.Position;
            float radiusSq = vortex.ValueRO.Radius * vortex.ValueRO.Radius;
            
            foreach (var (enemyTransform, physicsVelocity) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRW<PhysicsVelocity>>()
                         .WithAll<Enemy>())
            {
                float3 enemyPos = enemyTransform.ValueRO.Position;
                float3 toVortex = vortexPos - enemyPos;
                float distSq = math.lengthsq(toVortex);

                if (distSq > radiusSq || distSq < 0.001f)
                    continue;

                float3 pullDir = math.normalize(toVortex);
                if (bulletLookup.HasComponent(entity))
                {
                    float3 toPlayerDir = math.normalizesafe(playerPos - enemyPos);

                    // Calculate projection of pullDir onto toPlayerDir
                    float dot = math.dot(pullDir, toPlayerDir);

                    // Logic: If dot > 0, the pull vector has a component pointing TOWARDS the player.
                    if (dot > 0)
                    {
                        // Vector Rejection: Remove the 'towards player' component.
                        // This forces the pull to be perpendicular to the player direction 
                        // or away from it, effectively sliding the enemy "around" the player.
                        pullDir -= toPlayerDir * dot;
                    }
                }
                // Apply the modified force
                physicsVelocity.ValueRW.Linear += pullDir * vortex.ValueRO.Strength * dt;
            }
        }
    }
}
