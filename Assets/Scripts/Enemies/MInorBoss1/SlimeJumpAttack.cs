using System.Collections;
using Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class SlimeJumpAttack : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";

    [Tooltip("Flat ground decal/sprite shown at the landing spot. Should be ~1 unit diameter at scale 1.")]
    public GameObject targetingCirclePrefab;

    [Header("Range")]
    [Tooltip("Player must be within this distance for the slime to start the jump.")]
    public float jumpRange = 15f;

    [Header("Timing")]
    [Tooltip("Brief grounded anticipation before leaving the ground.")]
    public float windupDuration = 0.4f;

    [Tooltip("How long it takes to leap up and out of view.")]
    public float ascendDuration = 0.6f;

    [Tooltip("How long the slime stays hidden in the air while the circle telegraphs the landing.")]
    public float hangDuration = 1.0f;

    [Tooltip("How long the slam down takes - keep this fast.")]
    public float fallDuration = 0.3f;

    [Tooltip("Recovery time after landing before it can move/attack again.")]
    public float recoveryDuration = 0.6f;

    [Tooltip("Cooldown before the next jump attempt.")]
    public float attackCooldown = 5f;

    [Header("Jump")]
    [Tooltip("How high (in world units) the slime rises before it's considered 'out of view'.")]
    public float jumpHeight = 15f;

    [Tooltip("Radius of the landing/impact area, used for both the targeting circle and damage check.")]
    public float landingRadius = 3f;

    [Header("Damage")]
    public float impactDamage = 25f;

    private SlimeMovement slimeMovement;
    private float cooldownTimer;
    private bool isAttacking;
    private GameObject activeCircle;

    public bool IsAttacking => isAttacking;

    public void CancelAttack()
    {
        if (activeCircle != null) { Destroy(activeCircle); activeCircle = null; }
        StopAllCoroutines();
        isAttacking = false;
        if (slimeMovement != null) slimeMovement.CanMove = true;
    }

    void OnDestroy()
    {
        if (activeCircle != null) Destroy(activeCircle);
    }

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[SlimeJumpAttack] No player found with tag: " + playerTag);
        }

        slimeMovement = GetComponent<SlimeMovement>();
        cooldownTimer = attackCooldown;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // Always face the player.
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(lookDir), Time.deltaTime * 8f);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > jumpRange) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;
            StartCoroutine(JumpAttackSequence());
        }
    }

    IEnumerator JumpAttackSequence()
    {
        isAttacking = true;
        if (slimeMovement != null) slimeMovement.CanMove = false;

        Vector3 startPos = transform.position;

        // Lock in the landing spot from the player's position at the moment of takeoff.
        Vector3 landingPos = player.position;
        landingPos.y = startPos.y;

        // Spawn the targeting circle immediately so the player has the
        // full windup + ascend + hang time to get out of the way.
        GameObject circle = null;
        if (targetingCirclePrefab != null)
        {
            circle = Instantiate(targetingCirclePrefab, landingPos, Quaternion.Euler(90f, 0f, 0f));
            activeCircle = circle;
            circle.transform.localScale = Vector3.one * (landingRadius * 2f);

            var telegraph = circle.GetComponent<SlimeTargetingCircle>();
            if (telegraph != null) telegraph.Play(windupDuration + ascendDuration + hangDuration);
        }

        // Windup - brief crouch/anticipation, still grounded and visible.
        yield return new WaitForSeconds(windupDuration);

        // Ascend - leap upward and disappear from view.
        yield return MoveOverTime(startPos, startPos + Vector3.up * jumpHeight, ascendDuration);
        SetVisible(false);

        // Hang in the air - track the player slowly so the attack isn't trivial to dodge.
        float hangElapsed = 0f;
        while (hangElapsed < hangDuration)
        {
            hangElapsed += Time.deltaTime;

            // Drift the landing position toward the player during hang.
            Vector3 targetLanding = player.position;
            targetLanding.y = landingPos.y;
            landingPos = Vector3.Lerp(landingPos, targetLanding, Time.deltaTime * 6f);

            // Keep the circle in sync with the updated landing pos.
            if (circle != null)
                circle.transform.position = landingPos;

            yield return null;
        }

        // Reposition directly above the (now updated) landing spot, then slam down.
        Vector3 abovePos = landingPos + Vector3.up * jumpHeight;
        transform.position = abovePos;
        SetVisible(true);
        yield return MoveOverTime(abovePos, landingPos, fallDuration);

        // Impact - check for the player anywhere under the landing circle.
        DealImpactDamage(landingPos);

        if (circle != null) Destroy(circle);
        activeCircle = null;

        yield return new WaitForSeconds(recoveryDuration);

        if (slimeMovement != null) slimeMovement.CanMove = true;
        isAttacking = false;
    }

    IEnumerator MoveOverTime(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.position = to;
    }

    void SetVisible(bool visible)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }

    [Header("Physics Collision Filter")]
    [Tooltip("BelongsTo layer bitmask for the damage raycast (must match your physics layer setup).")]
    public uint rayBelongsTo = 1u << 3;

    [Tooltip("CollidesWith layer bitmask. Set this to the player's ECS physics layer.")]
    public uint rayCollidesWith = 1u << 1;

    // ─────────────────────────────────────────────────────────────
    void DealImpactDamage(Vector3 impactPos)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;

        var entityManager = world.EntityManager;

        var physicsWorldSingleton = entityManager.CreateEntityQuery(
                typeof(PhysicsWorldSingleton))
            .GetSingleton<PhysicsWorldSingleton>();

        var collisionWorld = physicsWorldSingleton.CollisionWorld;

        if (TryDamageAt(collisionWorld, entityManager, impactPos)) return;

        const int ringSamples = 8;
        for (int i = 0; i < ringSamples; i++)
        {
            float angle = (360f / ringSamples) * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * landingRadius;
            if (TryDamageAt(collisionWorld, entityManager, impactPos + offset)) return;
        }

        Debug.LogWarning("[SlimeJumpAttack] Stomp hit nothing. Check rayBelongsTo / rayCollidesWith match your player ECS physics layer.");
    }

    bool TryDamageAt(CollisionWorld collisionWorld, EntityManager entityManager, Vector3 origin)
    {
        var rayInput = new RaycastInput
        {
            Start = origin + Vector3.up * 5f,
            End = origin + Vector3.down * 5f,
            Filter = new CollisionFilter
            {
                BelongsTo = rayBelongsTo,
                CollidesWith = rayCollidesWith,
                GroupIndex = 0
            }
        };

        if (collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            if (entityManager.HasComponent<Health>(hit.Entity))
            {
                var health = entityManager.GetComponentData<Health>(hit.Entity);
                if (health.Value < 0) return true;

                health.Value -= impactDamage;
                entityManager.SetComponentData(hit.Entity, health);
                return true;
            }
        }

        return false;
    }
}