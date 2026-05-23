using UnityEngine;
using Unity.Entities;
using Components;

public class EnemyAuthoring : MonoBehaviour
{
    [Header("Base Stats")]
    public float health = 50f;
    public float moveSpeed = 5f;
    public int soulsReward = 5;
    public int condensedSoulsReward = 1; //only from bosses?
    public int xpReward = 10;
    
    [Header("Combat Config")]
    public bool isRanged = false;
    
    [Header("Ranged stats")]
    public float attackRange = 10f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public float damage = 10f;
    public GameObject bulletPrefab;
    
    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            //Core components
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

            if (authoring.isRanged)
            {
                AddComponent(entity, new EngagementRange {Value = authoring.attackRange});
                AddComponent(entity, new WeaponTarget
                {
                    Player = false 
                });
                AddComponent(entity, new BulletPrefabRef
                {
                    Value = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new Weapon()
                {
                    BulletSpeed = authoring.bulletSpeed,
                    Damage = authoring.damage,
                    FireRate = authoring.fireRate,
                    FireCooldown = 5f,
                    IsFiring = false,
                    SpawnOffset = new Unity.Mathematics.float3(0f, 0f, 1f)
                });
                AddComponent(entity, new VortexMod
                {
                    Radius = 0f,    // Default to 0 so standard enemies don't spawn vortexes
                    Strength = 0f,
                    Damage = 0f,
                    Scale = 1
                });
            }
        }
    }
}