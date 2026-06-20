using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MainBossBridge : MonoBehaviour
{
    public Vector3 hitboxOffset = Vector3.zero;

    private EntityManager _em;
    private Entity _hitboxEntity;
    private bool _entityFound;

    private void Start()
    {
        TryFindEntity();
    }

    private void Update()
    {
        if (!_entityFound) TryFindEntity();
        if (!_entityFound) return;

        if (!_em.Exists(_hitboxEntity))
        {
            _entityFound = false;
            return;
        }

        float3 targetPos = (float3)(transform.position + hitboxOffset);
        var lt = _em.GetComponentData<LocalTransform>(_hitboxEntity);
        lt.Position = targetPos;
        lt.Rotation = transform.rotation;
        _em.SetComponentData(_hitboxEntity, lt);
        //var hp = _em.GetComponentData<Health>(_hitboxEntity);
        //Debug.Log($"bob hp: {hp.Value}");
    }

    private void TryFindEntity()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated) return;

        _em = world.EntityManager;
        var query = _em.CreateEntityQuery(typeof(MainBossTag), typeof(LocalTransform));

        if (query.HasSingleton<MainBossTag>())
        {
            _hitboxEntity = query.GetSingletonEntity();
            _entityFound = true;
        }

        query.Dispose();
    }
}
