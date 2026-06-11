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
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        float dt = SystemAPI.Time.DeltaTime;
        var vortexLookup = SystemAPI.GetComponentLookup<VortexMovement>(true);
        var bulletLookup = SystemAPI.GetComponentLookup<BulletTag>(true);
        foreach (var (field, transform,entity) in 
                 SystemAPI.Query<RefRW<Lifetime>,RefRO<LocalTransform>>().WithEntityAccess().WithNone<MarkedForExecution,Executed>())
        {
            float3 pos = transform.ValueRO.Position;
            field.ValueRW.Value -= dt;

            if (!(field.ValueRO.Value <= 0f)) continue;
            
            if (vortexLookup.HasComponent(entity))
            {
                ecb.AddComponent<MarkedForExecution>(entity);
            }else if (bulletLookup.HasComponent(entity))
            {
                Debug.Log("Hallo bin im elif");
                ecb.AddComponent<MarkedForExecution>(entity);
                ecb.AddComponent<Executed>(entity);
            }
            else
            {
                ExplosionVFXSpawner.instance.Spawn(new float3(pos.x, pos.y + 0.1f, pos.z), 0);
                ecb.AddComponent<MarkedForExecution>(entity);
                ecb.AddComponent<Executed>(entity);
            }
            
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    
}
