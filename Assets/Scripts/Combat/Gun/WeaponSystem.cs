using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct WeaponSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // ECBSystem statt Allocator.Temp → ParallelWriter funktioniert
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        new WeaponJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB       = ecb.AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct WeaponJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;
    

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

        // Offset relativ zur aktuellen Schussrichtung rotieren
        float3 rotatedOffset = math.mul(rotation, weapon.SpawnOffset);
        float3 spawnPos = transform.Position + rotatedOffset;
        
        var bullet = ECB.Instantiate(chunkIndex, prefabRef.Value);
        
        
        if (vortexMod.Radius > 0)
        {
            ECB.AddComponent(chunkIndex, bullet,
                new Explosion
                {
                    Radius = vortexMod.Radius,
                    Damage = vortexMod.Damage,
                });
            
            ECB.AddComponent(chunkIndex, bullet,
                new PullEffect
                {
                    Radius =  vortexMod.Radius,
                    Strength = vortexMod.Strength,
                });
            
            
        }
        
        ECB.SetComponent(chunkIndex, bullet, LocalTransform.FromPositionRotationScale(
            spawnPos,
            rotation,
            vortexMod.Scale
        ));
        ECB.SetComponent(chunkIndex, bullet, new Speed  { Value  = dir * weapon.BulletSpeed });
        ECB.SetComponent(chunkIndex, bullet, new Damage { Value = weapon.Damage });
    }
}