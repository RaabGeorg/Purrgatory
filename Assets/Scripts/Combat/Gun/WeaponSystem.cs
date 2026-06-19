using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct WeaponSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        string currentSelection = GameData.Weapon;

        if (currentSelection == "Shotgun" && GameData.Yallah == 1)
        {
            Debug.Log("IM IF");

            foreach (var (weapon, _, entity) in
                 SystemAPI.Query<RefRW<Weapon>, RefRO<WeaponFromPlayerTag>>().WithEntityAccess().WithNone<MarkedForExecution, Executed>())
            {
                Debug.Log("AIEUJHRBTPIOUABHETR");
                weapon.ValueRW.Type = WeaponType.Shotgun;
                weapon.ValueRW.BulletSpeed = 15;
                weapon.ValueRW.FireRate = 1;
                weapon.ValueRW.Damage = 25;
                weapon.ValueRW.BulletScale = 0.7f;
                GameData.Yallah = 0;
            }
        }

        new WeaponJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB       = ecb.AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct WeaponJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    public WeaponType currentWeapon;
    public int yallah;
    void Execute([ChunkIndexInQuery] int chunkIndex,
        ref Weapon weapon, ref LocalTransform transform,
        in WeaponTarget target, in BulletPrefabRef prefabRef,
        ref VortexMod vortexMod)
    {
        weapon.FireCooldown -= DeltaTime;
        if (!weapon.IsFiring || weapon.FireCooldown > 0f) return;

        float3 dir = target.Value - transform.Position;
        dir.y = 0f;
        dir = math.normalize(dir);

        quaternion rotation = quaternion.LookRotationSafe(dir, math.up());
        transform.Rotation = rotation;
        weapon.FireCooldown = 1f / weapon.FireRate;

        float3 rotatedOffset = math.mul(rotation, weapon.SpawnOffset);
        float3 spawnPos = transform.Position + rotatedOffset;


        if (weapon.Type == WeaponType.Shotgun)
        {    
            FireShotgun(chunkIndex, ref weapon, ref vortexMod, prefabRef, spawnPos, rotation);
        }
        else
        {
            FireSingleBullet(chunkIndex, ref weapon, ref vortexMod, prefabRef, spawnPos, rotation, dir);
        }
    }

    void FireSingleBullet(int chunkIndex, ref Weapon weapon, ref VortexMod vortexMod,
        in BulletPrefabRef prefabRef, float3 spawnPos, quaternion rotation, float3 dir)
    {
        var bullet = ECB.Instantiate(chunkIndex, prefabRef.Value);

        if (vortexMod.Radius > 0)
        {
            ECB.AddComponent(chunkIndex, bullet, new Explosion
            {
                Radius = vortexMod.Radius,
                Damage = vortexMod.Damage,
            });
            ECB.AddComponent(chunkIndex, bullet, new PullEffect
            {
                Radius   = vortexMod.Radius,
                Strength = vortexMod.Strength,
            });
        }

        ECB.SetComponent(chunkIndex, bullet, LocalTransform.FromPositionRotationScale(
            spawnPos, rotation, weapon.BulletScale));
        ECB.SetComponent(chunkIndex, bullet, new Speed  { Value = dir * weapon.BulletSpeed });
        ECB.SetComponent(chunkIndex, bullet, new Damage { Value = weapon.Damage });
    }

    void FireShotgun(int chunkIndex, ref Weapon weapon, ref VortexMod vortexMod,
        in BulletPrefabRef prefabRef, float3 spawnPos, quaternion rotation)
    {
        int pelletCount = math.max(1, weapon.PelletCount);
        float damagePerPellet = weapon.Damage / pelletCount;

        for (int i = 0; i < pelletCount; i++)
        {
            float t = pelletCount > 1 ? (float)i / (pelletCount - 1) : 0.5f;
            float angleOffset = math.lerp(-weapon.SpreadAngle * 0.5f, weapon.SpreadAngle * 0.5f, t);

            quaternion spreadRotation = math.mul(rotation, quaternion.RotateY(math.radians(angleOffset)));
            float3 pelletDir = math.mul(spreadRotation, new float3(0, 0, 1));

            var bullet = ECB.Instantiate(chunkIndex, prefabRef.Value);

            if (vortexMod.Radius > 0)
            {
                ECB.AddComponent(chunkIndex, bullet, new Explosion
                {
                    Radius = vortexMod.Radius,
                    Damage = vortexMod.Damage,
                });
                ECB.AddComponent(chunkIndex, bullet, new PullEffect
                {
                    Radius   = vortexMod.Radius,
                    Strength = vortexMod.Strength,
                });
            }

            ECB.SetComponent(chunkIndex, bullet, LocalTransform.FromPositionRotationScale(
                spawnPos, spreadRotation, weapon.BulletScale));
            ECB.SetComponent(chunkIndex, bullet, new Speed  { Value = pelletDir * weapon.BulletSpeed });
            ECB.SetComponent(chunkIndex, bullet, new Damage { Value = damagePerPellet });
            ECB.SetComponent(chunkIndex, bullet, new Lifetime{ Value = 1.5f});
        }
    }
}