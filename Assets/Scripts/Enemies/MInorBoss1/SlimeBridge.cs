using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Components;

public class SlimeBossHitboxLinker : MonoBehaviour
{
    [Tooltip("Optional offset if the hitbox center doesn't match the pivot.")]
    public Vector3 hitboxOffset = Vector3.zero;

    [Tooltip("Seconds after Start() before a missing ECS entity triggers Die(). " +
             "Covers the baking delay on runtime spawn.")]
    public float entitySearchGracePeriod = 2f;

    private EntityManager em;
    private Entity hitboxEntity;
    private SlimeController slimeController;
    private bool entityFound = false;
    private float aliveTime = 0f;

    // Last known GROUND position — updated every frame we're NOT attacking,
    // so the hitbox stays on the floor during the jump instead of flying up.
    private Vector3 lastGroundPos;
    private SlimeJumpAttack jumpAttack;

    void Start()
    {
        slimeController = GetComponent<SlimeController>();
        jumpAttack = GetComponent<SlimeJumpAttack>();
        lastGroundPos = transform.position;
        TryFindEntity();
    }

    void Update()
    {
        aliveTime += Time.deltaTime;

        TryFindEntity();

        if (!entityFound)
        {
            if (aliveTime > entitySearchGracePeriod)
            {
                Debug.LogWarning("[SlimeBossHitboxLinker] ECS entity not found after grace period — calling Die().");
                slimeController.Die();
            }
            return;
        }

        // Entity was found before but is now genuinely gone (health hit 0).
        if (!em.Exists(hitboxEntity))
        {
            slimeController.Die();
            return;
        }

        // While attacking, move the hitbox off-map so the boss is invulnerable mid-jump.
        // When grounded, track normally.
        if (jumpAttack != null && jumpAttack.IsAttacking)
        {
            var localTransform = em.GetComponentData<LocalTransform>(hitboxEntity);
            localTransform.Position = new float3(0f, -9999f, 0f);
            em.SetComponentData(hitboxEntity, localTransform);
            return;
        }

        lastGroundPos = transform.position;
        float3 targetPos = (float3)(lastGroundPos + hitboxOffset);

        var lt = em.GetComponentData<LocalTransform>(hitboxEntity);
        lt.Position = targetPos;
        lt.Rotation = transform.rotation;
        em.SetComponentData(hitboxEntity, lt);
    }

    void TryFindEntity()
    {
        if (entityFound) return; // already linked, stop searching

        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;

        em = world.EntityManager;
        var query = em.CreateEntityQuery(typeof(SlimeBossTag), typeof(LocalTransform));

        if (query.HasSingleton<SlimeBossTag>())
        {
            hitboxEntity = query.GetSingletonEntity();
            entityFound = true;
        }

        query.Dispose();
    }
}