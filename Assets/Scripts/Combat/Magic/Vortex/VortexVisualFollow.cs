using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class VortexVisualFollow : MonoBehaviour
{
    private EntityManager _em;
    private Entity _entity;
    private bool _bound;

    public void Bind(EntityManager em, Entity entity)
    {
        _em = em;
        _entity = entity;
        _bound = true;
    }

    private void LateUpdate()
    {
        if (!_bound) return;
        
        if (!_em.Exists(_entity)
            || !_em.HasComponent<LocalTransform>(_entity)
            || _em.HasComponent<MarkedForExecution>(_entity))
        {
            _bound = false;
            Destroy(gameObject);
            return;
        }

        transform.position = _em.GetComponentData<LocalTransform>(_entity).Position;
    }
}
