using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BossPortalSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Spielerposition holen
        float3 playerPos = float3.zero;
        foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
        {
            playerPos = transform.ValueRO.Position;
            break;
        }

        new BossPortalJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            PlayerPos = playerPos,
            ECB       = ecb.AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct BossPortalJob : IJobEntity
{
    public float DeltaTime;
    public float3 PlayerPos;
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
            case AttackPattern.Spiral:
                FireSpiral(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.AimedBurst:
                FireAimedBurst(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.Wall:
                FireWall(chunkIndex, ref portal, origin, prefabRef);
                break;
            case AttackPattern.CrossRotate:
                FireCross(chunkIndex, ref portal, origin, prefabRef);
                break;
        }
    }

    // 🌀 Spiral – rotiert mit jedem Schuss weiter
    void FireSpiral(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        float angleStep = 360f / portal.BulletsPerShot;

        for (int i = 0; i < portal.BulletsPerShot; i++)
        {
            float angle = portal.CurrentAngle + angleStep * i;
            SpawnBullet(chunkIndex, portal, origin, prefabRef, AngleToDir(angle));
        }

        portal.CurrentAngle += portal.RotationSpeed * (1f / portal.FireRate);
    }

    // 🎯 Aimed Burst – zielt auf Spieler + Fächerspread
    void FireAimedBurst(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        float3 toPlayer = math.normalizesafe(PlayerPos - origin);
        float baseAngle = math.degrees(math.atan2(toPlayer.x, toPlayer.z));
        float spreadStep = portal.BulletsPerShot > 1 ? 30f / (portal.BulletsPerShot - 1) : 0f;

        for (int i = 0; i < portal.BulletsPerShot; i++)
        {
            float angle = baseAngle - 15f + spreadStep * i;
            SpawnBullet(chunkIndex, portal, origin, prefabRef, AngleToDir(angle));
        }
    }

    // 🧱 Wall – Bulletwand, Lücke NICHT beim Spieler → muss ausweichen
    void FireWall(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        float3 toPlayer = math.normalizesafe(PlayerPos - origin);
        float playerAngle = math.degrees(math.atan2(toPlayer.x, toPlayer.z));
        float angleStep = 360f / portal.BulletsPerShot;

        for (int i = 0; i < portal.BulletsPerShot; i++)
        {
            float angle = i * angleStep;

            // Lücke von 30 Grad BEIM Spieler → invertierte Logik → schwer zu sehen
            float diff = math.abs(DeltaAngle(angle, playerAngle));
            if (diff < 15f) continue; // Lücke beim Spieler → Portal beschützt sich selbst

            SpawnBullet(chunkIndex, portal, origin, prefabRef, AngleToDir(angle));
        }
    }

    // ✝️ Cross – 4 Bullets, rotiert langsam
    void FireCross(int chunkIndex, ref BossPortal portal, float3 origin, in BulletPrefabRef prefabRef)
    {
        for (int i = 0; i < 4; i++)
        {
            float angle = portal.CurrentAngle + 90f * i;
            SpawnBullet(chunkIndex, portal, origin, prefabRef, AngleToDir(angle));
        }
        portal.CurrentAngle += portal.RotationSpeed * (1f / portal.FireRate);
    }

    // ── Helpers ──────────────────────────────────────────────

    void SpawnBullet(int chunkIndex, in BossPortal portal, float3 origin,
        in BulletPrefabRef prefabRef, float3 dir)
    {
        quaternion rot = quaternion.LookRotationSafe(dir, math.up());
        var bullet = ECB.Instantiate(chunkIndex, prefabRef.Value);

        ECB.SetComponent(chunkIndex, bullet, LocalTransform.FromPositionRotation(origin, rot));
        ECB.SetComponent(chunkIndex, bullet, new Speed  { Value = dir * portal.BulletSpeed });
        ECB.SetComponent(chunkIndex, bullet, new Damage { Value = portal.Damage });
    }

    float3 AngleToDir(float degrees)
    {
        float rad = math.radians(degrees);
        return new float3(math.sin(rad), 0f, math.cos(rad));
    }

    float DeltaAngle(float a, float b)
    {
        float diff = (b - a + 360f) % 360f;
        return diff > 180f ? diff - 360f : diff;
    }
}
