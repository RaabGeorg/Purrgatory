using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using Components;

public class MinorBoss1HitBoxAuthoring : MonoBehaviour
{
    public float maxHealth = 200f;

    class Baker : Baker<MinorBoss1HitBoxAuthoring>
    {
        public override void Bake(MinorBoss1HitBoxAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Health
            {
                Value = authoring.maxHealth
            });


            AddComponent(entity, new SlimeBossTag());

            AddComponent(entity, new Enemy());
            
            AddComponent(entity, new Damage
            {
                Value = 15
            });

            AddComponent(entity, new CurrencyRewardComponent
            {
                CondensedSouls = 5,
                Souls = 100,
                Xp = 100,
            });
        }
    }

}
