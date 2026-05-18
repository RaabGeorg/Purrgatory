using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct BulletCollisionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var bulletLookup = SystemAPI.GetComponentLookup<Bullet>(true);
        var enemyLookup  = SystemAPI.GetComponentLookup<Enemy>(true);
        var healthLookup = SystemAPI.GetComponentLookup<Health>(false);

        foreach (var triggerEvent in simulation.AsSimulation().TriggerEvents)
        {
            Entity bullet = Entity.Null;
            Entity enemy  = Entity.Null;

            if (bulletLookup.HasComponent(triggerEvent.EntityA) && enemyLookup.HasComponent(triggerEvent.EntityB))
            {
                bullet = triggerEvent.EntityA;
                enemy  = triggerEvent.EntityB;
            }
            else if (bulletLookup.HasComponent(triggerEvent.EntityB) && enemyLookup.HasComponent(triggerEvent.EntityA))
            {
                bullet = triggerEvent.EntityB;
                enemy  = triggerEvent.EntityA;
            }

            if (bullet == Entity.Null) continue;

            if (healthLookup.HasComponent(enemy))
            {
                var health = healthLookup[enemy];
                health.Value -= bulletLookup[bullet].Damage;
                healthLookup[enemy] = health;
            }

            ecb.DestroyEntity(bullet);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}