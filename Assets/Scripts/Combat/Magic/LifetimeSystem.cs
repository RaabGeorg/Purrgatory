using Combat.Magic;
using Unity.Burst;
using Unity.Entities;
using Components;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct MagicFieldLifetimeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var VortexLookup = SystemAPI.GetComponentLookup<Explosion>(true);
        foreach (var (field, transform,entity) in 
                 SystemAPI.Query<RefRW<Lifetime>,RefRO<LocalTransform>>().WithEntityAccess())
        {
            float3 pos = transform.ValueRO.Position;
            field.ValueRW.Value -= dt;

            if (field.ValueRO.Value <= 0f)
            {
                if (VortexLookup.HasComponent(entity))
                {
                    ExplosionVFXSpawner.instance.Spawn(new float3(pos.x, pos.y + 0.1f, pos.z),1);
                    foreach (var (enemyTransform, health,enemy) in
                             SystemAPI.Query<RefRO<LocalTransform>, RefRW<Health>>()
                                 .WithAll<Enemy>().WithEntityAccess())
                    {
                        float dist = math.distance(
                            pos,
                            enemyTransform.ValueRO.Position
                        );

                        if (dist > VortexLookup[entity].Radius)
                            continue;

                        health.ValueRW.Value -= VortexLookup[entity].Damage;
                        if (health.ValueRW.Value <= 0f)
                            ecb.AddComponent<MarkedForExecution>(entity);
                        Debug.Log(health.ValueRW.Value);
                    }
                }
                else
                {
                    ExplosionVFXSpawner.instance.Spawn(new float3(pos.x, pos.y + 0.1f, pos.z), 0);
                }

                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
