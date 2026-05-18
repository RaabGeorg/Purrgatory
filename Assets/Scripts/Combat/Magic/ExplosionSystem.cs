using Unity.Burst;
using Unity.Entities;
using Components;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ExplosionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (field, transform, entity) in
                 SystemAPI.Query<RefRW<Explosion>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (enemyTransform, health, enemy) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRW<Health>>()
                         .WithAll<Enemy>().WithEntityAccess())
            {
                float3 pos = transform.ValueRO.Position;
                float dist = math.distance(
                    pos,
                    enemyTransform.ValueRO.Position
                );

                if (dist > field.ValueRO.Radius)
                    continue;

                health.ValueRW.Value -= field.ValueRW.Damage;
                if (health.ValueRW.Value <= 0f)
                    ecb.DestroyEntity(enemy);
            }
        }
    }
}
