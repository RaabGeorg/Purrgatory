using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public struct PlayerTag : IComponentData { }

public class PlayerBridge : MonoBehaviour
{
    private float3 lastPosition;
    private Entity playerEntity;
    private EntityManager entityManager;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var collider = Unity.Physics.SphereCollider.Create(
            new SphereGeometry { Center = float3.zero, Radius = 0.5f },
            CollisionFilter.Default);

        playerEntity = entityManager.CreateEntity(
            typeof(LocalTransform),
            typeof(PlayerTag),
            typeof(PhysicsCollider),
            typeof(PhysicsMass),
            typeof(PhysicsVelocity));
        
        entityManager.SetComponentData(playerEntity, new PhysicsCollider { Value = collider });
        entityManager.SetComponentData(playerEntity, PhysicsMass.CreateKinematic(MassProperties.UnitSphere));
        
    }

    void Update()
    {
        
        float3 currentPosition = transform.position;
        float3 velocity = (currentPosition -  lastPosition ) / Time.deltaTime;
        
        //position sync
        entityManager.SetComponentData(playerEntity, LocalTransform.FromPosition(currentPosition));
        
        //sync velocty
        entityManager.SetComponentData(playerEntity, new PhysicsVelocity { Linear = velocity });
        
        lastPosition = currentPosition;

    }
}