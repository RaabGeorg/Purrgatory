using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Health : IComponentData { public float Value; }
    public struct Speed  : IComponentData { public float3 Value; }
    //public struct Bullet : IComponentData {}
    public struct MagicFieldTag : IComponentData { }
    public struct PlayerTag : IComponentData { }
    public struct Enemy     : IComponentData { }
    public struct WeaponTag  : IComponentData { } // ← neu
    public struct BulletTag : IComponentData { }
    public struct EyeBossTag : IComponentData { }
    
    public struct WeaponFromPlayerTag : IComponentData { }

    public struct MarkedForExecution : IComponentData {}
    public struct Executed : IComponentData {}
    
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

    public struct VortexMod : IComponentData
    {
        public float Radius;
        public float Strength;
        public float Damage;
        public float Scale;
    }

    public struct WeaponTarget : IComponentData
    {
        public float3 Value;
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
    public struct PullEffect : IComponentData
    {
        public float Radius;
        public float Strength;
    }
    public struct VortexPrefabRef : IComponentData
    {
        public Entity Value;
    }
    public struct CurrencyRewardComponent : IComponentData
    {
        public int Souls;
        public int CondensedSouls;
        public int Xp;
    }
    
    
    public struct EnemyMovementData : IComponentData
    {
        public float Speed;
    }

    public struct EngagementRange : IComponentData
    {
        public float Value;
    }

    public struct PlayerStatsComponent : IComponentData
    {
        public float AttackSpeed;
        public float Damage;
    }

    public struct EnemySpawner : IComponentData
    {
        public Entity Enemy;
        public float SpawnInterval;
        public float NextSpawnTime;
        public Unity.Mathematics.Random Random;
    }

    
    public struct ChunkTag : IComponentData { public int2 Coordinate; } // ← neuer
    public struct ChunkConfigData : IComponentData // ← neuer
    {
        public int ChunkSize;
        public int ViewDistance;
    }
    public struct ChunkPrefabData : IComponentData
    {
        public Entity Prefab;
    }


    public struct Damage : IComponentData
    {
        public float Value;
    }
    public struct ApplyKnockback : IComponentData
    {
    }
    
    public struct ActiveSceneEntity : IComponentData, IEnableableComponent {}

    public struct LevelSceneTag : IComponentData
    {
        public Unity.Collections.FixedString64Bytes LevelName;
    }
    public struct PlayerInputState : IComponentData
    {
        public bool IsFiring;
    }

   
}
