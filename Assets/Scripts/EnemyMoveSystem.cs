using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct EnemyMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
       
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        
        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float deltaTime = SystemAPI.Time.DeltaTime;

        
        foreach (var (transform, enemy) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyData>>())
        {
            float3 direction = math.normalize(playerPos - transform.ValueRO.Position);
            
            transform.ValueRW.Position += direction * enemy.ValueRO.Speed * deltaTime;
            
        }
    }
}