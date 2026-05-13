using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Components;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial struct MagicFieldDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
        float dt = SystemAPI.Time.DeltaTime;
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var magicFieldLookup = SystemAPI.GetComponentLookup<MagicField>(true);
        var enemyLookup  = SystemAPI.GetComponentLookup<Enemy>(true);
        var healthLookup = SystemAPI.GetComponentLookup<Health>(false);

        foreach (var triggerEvent in simulation.AsSimulation().TriggerEvents)
        {
           
            Entity magicField = Entity.Null;
            Entity enemy  = Entity.Null;
            
            if (magicFieldLookup.HasComponent(triggerEvent.EntityA) && enemyLookup.HasComponent(triggerEvent.EntityB))
            {
                magicField = triggerEvent.EntityA;
                enemy  = triggerEvent.EntityB;
            }
            else if (magicFieldLookup.HasComponent(triggerEvent.EntityB) && enemyLookup.HasComponent(triggerEvent.EntityA))
            {
                magicField = triggerEvent.EntityB;
                enemy = triggerEvent.EntityA;
            }

            if (magicField == Entity.Null) continue;
           
            if (healthLookup.HasComponent(enemy))
            {
                var health = healthLookup[enemy];
                health.Value -= magicFieldLookup[magicField].Damage * dt;
                Debug.Log(health.Value);
                healthLookup[enemy] = health;
                if (health.Value <= 0f)
                    ecb.DestroyEntity(enemy);
                    
            }
            
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

