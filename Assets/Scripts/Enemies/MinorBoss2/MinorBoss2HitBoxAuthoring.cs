using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using Components;

public class MinorBoss2HitBoxAuthoring : MonoBehaviour
{
    public float maxHealth = 200f;
 
    class Baker : Baker<MinorBoss2HitBoxAuthoring>
    {
        public override void Bake(MinorBoss2HitBoxAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
 
            AddComponent(entity, new Health
            {
                Value = authoring.maxHealth
            });
 
           
            AddComponent(entity, new EyeBossTag());
            
            AddComponent(entity, new Enemy());
            
            AddComponent(entity, new CurrencyRewardComponent
            {
                CondensedSouls = 20,
                Souls = 100,
                Xp = 100,
            });
        }
    }

}
