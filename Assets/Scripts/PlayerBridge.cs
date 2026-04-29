using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct PlayerTag : IComponentData { }

public class PlayerBridge : MonoBehaviour
{
    private Entity playerEntity;
    private EntityManager entityManager;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        playerEntity = entityManager.CreateEntity(typeof(LocalTransform), typeof(PlayerTag));
        
    }

    void Update()
    {
        
        entityManager.SetComponentData(playerEntity, LocalTransform.FromPosition(transform.position));
    }
}