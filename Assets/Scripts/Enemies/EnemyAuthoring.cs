using UnityEngine;
using Unity.Entities;
using Components;
public struct EnemyData : IComponentData
{
    public float Speed;
}

public class EnemyAuthoring : MonoBehaviour
{
    public float health = 50f;
    public float moveSpeed;

    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyData { 
                Speed = authoring.moveSpeed
            });
            AddComponent(entity, new Health { Value = authoring.health });
            AddComponent(entity, new Enemy());
        }
    }
}