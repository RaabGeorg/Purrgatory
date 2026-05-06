using Components;
using Unity.Entities;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public float damage      = 10f;
    public float fireRate    = 2f;
    public bool  isPlayer;
    public Transform spawnPoint;

    class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Weapon
            {
                BulletSpeed  = authoring.bulletSpeed,
                Damage       = authoring.damage,
                FireRate     = authoring.fireRate,
                FireCooldown = 1f / authoring.fireRate, // ← startet mit vollem Cooldown
                IsFiring     = false,
                SpawnOffset  = authoring.spawnPoint.position,
            });

            // Entity separat → kein unmanaged Problem
            AddComponent(entity, new BulletPrefabRef
            {
                Value = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic)
            });

            AddComponent(entity, new WeaponTarget());

            AddComponent(entity, new WeaponTag());
        }
    }
}