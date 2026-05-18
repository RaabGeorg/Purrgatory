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

    public struct MagicField : IComponentData
    {
        public float Damage;
        
    }
    public struct VortexMovement : IComponentData
    {
        public float3 Center;
        public float RadiusX;
        public float RadiusZ;
        public float Speed;
        public float Time;
    }

    public struct Explosion : IComponentData
    {
        public float Radius;
        public float Damage;
    }
    public struct Lifetime : IComponentData
    {
        public float Value;
    }
    public struct MagicFieldPrefabRef : IComponentData
    {
        public Entity Value;
    }
    public struct Vortex : IComponentData
    {
        public float Radius;
        public float PullStrength;
    }
    public struct VortexPrefabRef : IComponentData
    {
        public Entity Value;
    }
    public struct CurrencyRewardComponent : IComponentData
    {
        public int Souls;
        public int Coins;
    }
    public struct MarkedForExecution : IComponentData {}
    public struct Executed : IComponentData {}
    
    public struct EnemyMovementData : IComponentData
    {
        public float Speed;
    }
}