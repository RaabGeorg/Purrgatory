using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Components;

public class PlayerBridge : MonoBehaviour
{
    private Entity _playerEntity;
    private EntityManager _em;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerEntity = _em.CreateEntity(typeof(LocalTransform), typeof(PlayerTag));
    }

    void Update()
    {
        _em.SetComponentData(_playerEntity,
            LocalTransform.FromPosition(transform.position));
    }
}