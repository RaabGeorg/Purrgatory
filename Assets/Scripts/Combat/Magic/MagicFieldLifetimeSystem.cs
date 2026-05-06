using Unity.Burst;
using Unity.Entities;
using Components;

[BurstCompile]
public partial struct MagicFieldLifetimeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (field, entity) in 
                 SystemAPI.Query<RefRW<MagicField>>().WithEntityAccess())
        {
            field.ValueRW.Lifetime -= dt;

            if (field.ValueRO.Lifetime <= 0f)
                ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
