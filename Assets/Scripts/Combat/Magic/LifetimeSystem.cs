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
        var vortexLookup = SystemAPI.GetComponentLookup<Explosion>(true);
        foreach (var (field, transform,entity) in 
                 SystemAPI.Query<RefRW<Lifetime>,RefRO<LocalTransform>>().WithEntityAccess())
        {
            float3 pos = transform.ValueRO.Position;
            field.ValueRW.Value -= dt;

            if (!(field.ValueRO.Value <= 0f)) continue;
            
            if (vortexLookup.HasComponent(entity))
            {
                ExplosionVFXSpawner.instance.Spawn(new float3(pos.x, pos.y + 0.1f, pos.z),1);
                
            }
            else
            {
                ExplosionVFXSpawner.instance.Spawn(new float3(pos.x, pos.y + 0.1f, pos.z), 0);
            }
        }
    }
}
