using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Components;

public class EyeBossHitboxLinker : MonoBehaviour
{
    
    public Vector3 hitboxOffset = Vector3.zero;
    
    private EntityManager em;
    private Entity hitboxEntity;
    private EyeController EyeController;

   
    void Start()
    {
        EyeController = GetComponent<EyeController>();
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;

        em = world.EntityManager;

       
        var query = em.CreateEntityQuery(typeof(EyeBossTag), typeof(LocalTransform));

        if (query.HasSingleton<EyeBossTag>())
        {
            hitboxEntity = query.GetSingletonEntity();
            
        }
       

        query.Dispose();
    }

    void Update()
    {
        TryFindEntity();
        
    
        if (!em.Exists(hitboxEntity)) return;

        float3 targetPos = (float3)(transform.position + hitboxOffset);

        var localTransform = em.GetComponentData<LocalTransform>(hitboxEntity);
        localTransform.Position = targetPos;
        localTransform.Rotation = transform.rotation;
        em.SetComponentData(hitboxEntity, localTransform);
    }

    void TryFindEntity()
    {
        
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;

        em = world.EntityManager;
        var query = em.CreateEntityQuery(typeof(EyeBossTag), typeof(LocalTransform));

        if (query.HasSingleton<EyeBossTag>())
        {
            hitboxEntity = query.GetSingletonEntity();
            
        }
        else
        {
            EyeController.Die();
        }

        query.Dispose();
    }
    
}
