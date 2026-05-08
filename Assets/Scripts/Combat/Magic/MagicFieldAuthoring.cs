using Unity.Entities;
using UnityEngine;
using Components;

public class MagicFieldAuthoring : MonoBehaviour
{
    public float damage = 5f;
    public float lifetime = 5f;

    class Baker : Baker<MagicFieldAuthoring>
    {
        public override void Bake(MagicFieldAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MagicField
            {
                Damage = authoring.damage,
                Lifetime = authoring.lifetime,
                
            });
            
        }
    }
}
