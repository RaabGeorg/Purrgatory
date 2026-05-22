using UnityEngine;
using Unity.Entities;
using Components;

public class EnemyAuthoring : MonoBehaviour
{
    public float health = 50f;
    public float moveSpeed = 5f;

    public int soulsReward = 5;
    public int condensedSoulsReward = 1; //only from bosses?
    public int xpReward = 10;
    
    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyMovementData { Speed = authoring.moveSpeed });
            AddComponent(entity, new Health { Value = authoring.health });
            AddComponent(entity, new Enemy());
            AddComponent(entity, new CurrencyRewardComponent
            { 
                Souls = authoring.soulsReward, 
                CondensedSouls = authoring.condensedSoulsReward,
                Xp = authoring.xpReward
            });
        }
    }
}