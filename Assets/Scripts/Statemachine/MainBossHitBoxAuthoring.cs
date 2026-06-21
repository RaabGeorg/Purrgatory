using Components;
using Unity.Entities;
using UnityEngine;

public class MainBossHitBoxAuthoring : MonoBehaviour
{
    public float maxHealth = 5000f;

    class Baker : Baker<MainBossHitBoxAuthoring>
    {
        public override void Bake(MainBossHitBoxAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Health { Value = authoring.maxHealth });
            
            AddComponent(entity, new MainBossTag());
            
            AddComponent(entity, new Enemy());
        }
    }
}
