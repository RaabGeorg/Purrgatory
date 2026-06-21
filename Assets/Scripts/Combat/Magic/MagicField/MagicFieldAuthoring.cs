using Unity.Entities;
using UnityEngine;
using Components;

public class MagicFieldAuthoring : MonoBehaviour
{
    public float damage = 4f;
    public float lifetime = 5f;

    class Baker : Baker<MagicFieldAuthoring>
    {
        public override void Bake(MagicFieldAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Damage
            {
                Value = authoring.damage,
                
            });
            AddComponent(entity, new Lifetime
            {
                Value = authoring.lifetime
            });
            AddComponent(entity, new MagicFieldTag());

        }
    }
}
