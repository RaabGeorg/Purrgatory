using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Health : IComponentData { public float Value; }
    public struct Bullet : IComponentData { public float Damage; }
    public struct Speed  : IComponentData { public float3 Value; }

    public struct PlayerTag : IComponentData { }
    public struct Enemy     : IComponentData { }
    public struct WeaponTag  : IComponentData { } // ← neu
    // Entity raus → eigene Component, sonst "must be valid unmanaged type" Error
    public struct BulletPrefabRef : IComponentData
    {
        public Entity Value;
    }

    public struct Weapon : IComponentData
    {
        public float BulletSpeed;
        public float Damage;
        public float FireRate;
        public float FireCooldown;
        public bool IsFiring;
        public float3 SpawnOffset;
    }

    public struct WeaponTarget : IComponentData
    {
        public float3 Value;
    }
}