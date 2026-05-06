using Unity.Entities;
using UnityEngine;
using Components;

public class MagicFieldAuthoring : MonoBehaviour
{
    public float damage = 5f;
    public float radius = 3f;
    public float lifetime = 5f;
    public float tickRate = 1f;

    class Baker : Baker<MagicFieldAuthoring>
    {
        public override void Bake(MagicFieldAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MagicField
            {
                Damage = authoring.damage,
                Radius = authoring.radius,
                Lifetime = authoring.lifetime,
                TickTimer = 1f,
                TickRate = authoring.tickRate
            });
            
        }
    }
}
