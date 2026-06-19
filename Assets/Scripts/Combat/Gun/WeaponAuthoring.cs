using Components;
using Unity.Entities;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public float damage      = 10f;
    public float fireRate    = 2f;
    public Transform spawnPoint;

    public bool isPlayer = false;

    [Header("Weapon Type")]
    public WeaponType weaponType = WeaponType.Normal;

    [Header("Shotgun")]
    public int   pelletCount  = 5;
    public float spreadAngle  = 25f;

    [Header("Vortex")]
    public bool  vortexMode;
    public float pullRadius;
    public float pullStrength;
    public float explosiveDamage;
    public float bulletScale = 1f;

    class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            if (authoring.weaponType == WeaponType.Shotgun)
            {
                authoring.bulletSpeed = 15;
                authoring.fireRate = 1;
                authoring.damage = 25;
                authoring.bulletScale = 0.7f;
            }

            AddComponent(entity, new Weapon
            {
                Type         = authoring.weaponType,
                BulletSpeed  = authoring.bulletSpeed,
                Damage       = authoring.damage,
                FireRate     = authoring.fireRate,
                FireCooldown = 1f / authoring.fireRate,
                IsFiring     = false,
                SpawnOffset  = authoring.spawnPoint.position,
                PelletCount  = authoring.pelletCount,
                SpreadAngle  = authoring.spreadAngle,
            });

            AddComponent(entity, new BulletPrefabRef
            {
                Value = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic)
            });

            AddComponent(entity, new WeaponTarget());
            AddComponent(entity, new WeaponTag());
            AddComponent(entity, new VortexMod
            {
                Radius   = authoring.vortexMode ? authoring.pullRadius : 0,
                Strength = authoring.pullStrength,
                Damage   = authoring.explosiveDamage,
                Scale    = authoring.bulletScale,
            });
            AddComponent(entity, new WeaponFromPlayerTag());
        }
    }
}