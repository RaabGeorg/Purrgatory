using UnityEngine;
using Unity.Entities;

public struct EnemyData : IComponentData
{
    public float Speed;
}

public class EnemyAuthoring : MonoBehaviour
{
    public float moveSpeed = 3.0f;

    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyData { 
                Speed = authoring.moveSpeed 
            });
        }
    }
}