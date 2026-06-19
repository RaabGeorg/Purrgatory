using UnityEngine;
using Components;
using Unity.Entities;
public class BossPortalAuthoring : MonoBehaviour
{
    public AttackPattern pattern = AttackPattern.TripleSpiral;
    public float fireRate        = 1.5f;
    public float bulletSpeed     = 8f;
    public float damage          = 10f;
    public float rotationSpeed   = 30f;
    public int   bulletsPerShot  = 12;
    public uint  randomSeed      = 42;
    public GameObject bulletPrefab;

    class Baker : Baker<BossPortalAuthoring>
    {
        public override void Bake(BossPortalAuthoring a)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BossPortal
            {
                Pattern        = a.pattern,
                FireRate       = a.fireRate,
                BulletSpeed    = a.bulletSpeed,
                Damage         = a.damage,
                RotationSpeed  = a.rotationSpeed,
                BulletsPerShot = a.bulletsPerShot,
                RandomSeed     = a.randomSeed,
            });
            AddComponent(entity, new BulletPrefabRef
            {
                Value = GetEntity(a.bulletPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}