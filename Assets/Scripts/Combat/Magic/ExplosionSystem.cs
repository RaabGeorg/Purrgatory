using Combat.Magic;
using Unity.Burst;
using Unity.Entities;
using Components;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ExplosionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (field, transform, entity) in
                 SystemAPI.Query<RefRW<Explosion>, RefRO<LocalTransform>>().WithAll<MarkedForExecution>().WithEntityAccess().WithNone<Executed>())
        {
            float3 pos = transform.ValueRO.Position;
            foreach (var (enemyTransform, health, enemy) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRW<Health>>()
                         .WithAll<Enemy>().WithEntityAccess().WithNone<MarkedForExecution,Executed>())
            {
                
                float dist = math.distance(
                    pos,
                    enemyTransform.ValueRO.Position
                );
                
                if (dist > field.ValueRO.Radius)
                    continue;
                health.ValueRW.Value -= field.ValueRW.Damage;
                if (health.ValueRW.Value <= 0f)
                    ecb.AddComponent<MarkedForExecution>(enemy);
            }
            ExplosionVFXSpawner.instance.Spawn(new float3(pos.x, pos.y + 0.1f, pos.z),1);
            ecb.AddComponent<Executed>(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
