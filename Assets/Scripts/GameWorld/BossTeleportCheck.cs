using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Components;

public class BossTeleportCheck : MonoBehaviour
{
    [SerializeField] private int requiredSouls = 10;
    [SerializeField] private int requiredCondensedSouls = 0;
    
    [SerializeField] private Transform destinationTarget;

    // private void OnTriggerEnter(UnityEngine.Collider other)
    // {
    //     //Debug.Log(QuestManager.Instance.BossUnlocked);
    //     // 1. Resolve to the root player Rigidbody
    //     if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag("Player"))
    //     {
    //         return; 
    //     }
    //
    //     Transform playerRoot = other.attachedRigidbody.transform;
    //
    //     // 2. Validate Economy
    //     var wallet = PlayerWallet.Instance;
    //     if (wallet == null) return;
    //     
    //     // 3. Execute
    //     if (QuestManager.Instance.BossUnlocked)
    //     {
    //         Teleport(playerRoot);
    //     }
    // }
    
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        //Debug.Log(QuestManager.Instance.BossUnlocked);
        // 1. Resolve to the root player Rigidbody
        if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag("Player"))
        {
            return; 
        }

        Transform playerRoot = other.attachedRigidbody.transform;

        // 2. Validate Economy
        var wallet = PlayerWallet.Instance;
        if (wallet == null) return;
    
        // 3. Execute
        Teleport(playerRoot);
    }

    private void Teleport(Transform playerTransform)
    {
        if (destinationTarget == null) return;

        Vector3 targetPos = destinationTarget.position;
        Quaternion targetRot = destinationTarget.rotation;

        // 1. Move the Hybrid GameObject visual shell
        playerTransform.position = targetPos;
        playerTransform.rotation = targetRot;

        // 2. Force ECS Synchronization
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(PlayerTag), typeof(LocalTransform));

        if (query.HasSingleton<PlayerTag>())
        {
            Entity playerEntity = query.GetSingletonEntity();
            
            // Overwrite ECS positional data
            var localTransform = em.GetComponentData<LocalTransform>(playerEntity);
            localTransform.Position = targetPos;
            localTransform.Rotation = targetRot;
            em.SetComponentData(playerEntity, localTransform);

            // 3. Clear Momentum
            if (em.HasComponent<PhysicsVelocity>(playerEntity))
            {
                em.SetComponentData(playerEntity, new PhysicsVelocity());
            }
        }
    }
}