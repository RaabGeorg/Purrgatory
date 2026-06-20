using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Components;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BossPortalSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BossPortalPhaseActive>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new BossPortalJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            
            ECB       = ecb.AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct BossPortalJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex,
        ref BossPortal portal, in LocalTransform transform, in BulletPrefabRef prefabRef)
    {
        portal.FireCooldown -= DeltaTime;
        if (portal.FireCooldown > 0f) return;
        portal.FireCooldown = 1f / portal.FireRate;

        float3 origin = transform.Position;

        switch (portal.Pattern)
        {
            case AttackPattern.TripleSpiral:
                FireTripleSpiral(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.FlowerPetal:
                FireFlowerPetal(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.ChaosStorm:
                FireChaosStorm(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.ConcentricRings:
                FireConcentricRings(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.None:
                break;
        }
    }
    
    void FireTripleSpiral(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        for (int arm = 0; arm < 3; arm++)
        {
            float armOffset = 120f * arm;
            for (int i = 0; i < 3; i++)
            {
                
                float angle = portal.CurrentAngle + armOffset + i * 8f;
                float speed = portal.BulletSpeed - i * 1.5f; 
                SpawnBulletSpeed(chunkIndex, portal, origin, prefabRef, AngleToDir(angle), speed);
            }
        }
        portal.CurrentAngle += portal.RotationSpeed * (1f / portal.FireRate);
    }
    
    void FireFlowerPetal(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        int petals      = 5;
        int bulletsPerPetal = 4;
        float petalSpread = 12f; 

        for (int p = 0; p < petals; p++)
        {
            float petalCenter = portal.CurrentAngle + (360f / petals) * p;

            for (int b = 0; b < bulletsPerPetal; b++)
            {
                float t     = (float)b / (bulletsPerPetal - 1);
                float angle = petalCenter + math.lerp(-petalSpread, petalSpread, t);
                
                float speedMod = 1f - math.abs(t - 0.5f) * 0.4f;
                SpawnBulletSpeed(chunkIndex, portal, origin, prefabRef,
                    AngleToDir(angle), portal.BulletSpeed * speedMod);
            }
        }

        portal.CurrentAngle += portal.RotationSpeed * (1f / portal.FireRate);
    }
    
    
    
    void FireChaosStorm(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        var rng = new Unity.Mathematics.Random(portal.RandomSeed + (uint)(portal.CurrentAngle * 100));

        int total = portal.BulletsPerShot;

        for (int i = 0; i < total; i++)
        {
            float baseAngle  = (360f / total) * i;
            float jitter     = rng.NextFloat(-15f, 15f);
            float finalAngle = baseAngle + portal.CurrentAngle + jitter;

            
            int skipIndex = (int)(portal.CurrentAngle / 10f) % 4;
            if (i % 4 == skipIndex) continue;

            float speedVar = rng.NextFloat(0.7f, 1.3f);
            SpawnBulletSpeed(chunkIndex, portal, origin, prefabRef,
                AngleToDir(finalAngle), portal.BulletSpeed * speedVar);
        }

        portal.CurrentAngle += portal.RotationSpeed * (1f / portal.FireRate);
        portal.RandomSeed   += 1337;
    }
    
    void FireConcentricRings(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        int ringsPerShot = 2;
        int bulletsPerRing = portal.BulletsPerShot / ringsPerShot;

        for (int ring = 0; ring < ringsPerShot; ring++)
        {
            
            float ringAngle = portal.CurrentAngle * (ring % 2 == 0 ? 1f : -1.3f);
            float speedMod  = 1f + ring * 0.3f; 

            for (int i = 0; i < bulletsPerRing; i++)
            {
                float angle = ringAngle + (360f / bulletsPerRing) * i;
                SpawnBulletSpeed(chunkIndex, portal, origin, prefabRef,
                    AngleToDir(angle), portal.BulletSpeed * speedMod);
            }
        }

        portal.CurrentAngle += portal.RotationSpeed * (1f / portal.FireRate);
        portal.RingIndex++;
    }

    
    
    void SpawnBullet(int chunkIndex, in BossPortal portal, float3 origin,
        in BulletPrefabRef prefabRef, float3 dir)
        => SpawnBulletSpeed(chunkIndex, portal, origin, prefabRef, dir, portal.BulletSpeed);

    void SpawnBulletSpeed(int chunkIndex, in BossPortal portal, float3 origin,
        in BulletPrefabRef prefabRef, float3 dir, float speed)
    {
        var bullet = ECB.Instantiate(chunkIndex, prefabRef.Value);
        ECB.SetComponent(chunkIndex, bullet,
            LocalTransform.FromPositionRotation(origin,
                quaternion.LookRotationSafe(dir, math.up())));
        ECB.SetComponent(chunkIndex, bullet, new Speed  { Value = dir * speed });
        ECB.SetComponent(chunkIndex, bullet, new Damage { Value = portal.Damage });
    }

    float3 AngleToDir(float degrees)
    {
        float rad = math.radians(degrees);
        return new float3(math.sin(rad), 0f, math.cos(rad));
    }
}