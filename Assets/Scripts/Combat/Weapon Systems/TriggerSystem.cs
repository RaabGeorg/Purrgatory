using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct TriggerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        state.Dependency = new TriggerJob
        {
            dt = SystemAPI.Time.DeltaTime,
            MagicFieldLookup = SystemAPI.GetComponentLookup<MagicFieldTag>(true),
            DamageLookup = SystemAPI.GetComponentLookup<Damage>(true),
            HealthLookup = SystemAPI.GetComponentLookup<Health>(false),
            ExplosiveLookup = SystemAPI.GetComponentLookup<Explosion>(true),
            EnemyLookup = SystemAPI.GetComponentLookup<Enemy>(true),
            ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
struct TriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<Damage> DamageLookup;
    [ReadOnly] public ComponentLookup<MagicFieldTag> MagicFieldLookup;
    [ReadOnly] public ComponentLookup<Explosion>  ExplosiveLookup;
    [ReadOnly] public ComponentLookup<Enemy> EnemyLookup;
    public ComponentLookup<Health> HealthLookup;
    public EntityCommandBuffer ecb;
    public float dt;

    [BurstCompile]
    public void Execute(TriggerEvent triggerEvent)
    {
        Entity damageEntity = Entity.Null;
        Entity healthEntity = Entity.Null;

        if (DamageLookup.HasComponent(triggerEvent.EntityA) && HealthLookup.HasComponent(triggerEvent.EntityB))
        {
            damageEntity = triggerEvent.EntityA;
            healthEntity = triggerEvent.EntityB;
        }
        else if (DamageLookup.HasComponent(triggerEvent.EntityB) && HealthLookup.HasComponent(triggerEvent.EntityA))
        {
            damageEntity = triggerEvent.EntityB;
            healthEntity = triggerEvent.EntityA;
        }
        
        if (damageEntity == Entity.Null) return;

        bool damageIsEnemy = EnemyLookup.HasComponent(damageEntity);
        bool healthIsEnemy = EnemyLookup.HasComponent(healthEntity);

        //if both are enemies, ignore the trigger
        if (damageIsEnemy && healthIsEnemy) return;

        bool isMagicField = MagicFieldLookup.HasComponent(damageEntity);
        float damage = DamageLookup[damageEntity].Value;
        var health = HealthLookup[healthEntity];

        health.Value -= isMagicField ? damage * dt : damage;
        Debug.Log($"Dealt {damage} damage, HP remaining: {health.Value}");
        HealthLookup[healthEntity] = health;

        if (!isMagicField)
        {
            if (damageIsEnemy)
            {
                ecb.AddComponent(damageEntity, new ApplyKnockback());
            }
            else
            {
                ecb.AddComponent<MarkedForExecution>(damageEntity);
            
                if (!ExplosiveLookup.HasComponent(damageEntity))
                {
                    ecb.AddComponent<Executed>(damageEntity);
                }
            }
        }
    }
}