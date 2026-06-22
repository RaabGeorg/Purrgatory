using System.Collections;
using Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using RaycastHit = UnityEngine.RaycastHit;

public class EyeLaser : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public string playerTag = "Player";
    
    public Transform laserOrigin;
    
    public LineRenderer laserLine;

    [Header("Range")]
    public float shootRange = 12f;

    [Header("Timing")]
    public float trackingTime = 2.0f;
    
    public float lockTime = 0.3f;
    
    public float fireDuration = 1.2f;
    
    public float shootCooldown = 4f;

    [Header("Laser")]
    public float laserDamage = 10f;
    public float laserMaxLength = 25f;
    public LayerMask laserHitMask;

    [Header("Colors")]
    public Color trackingColor = new Color(1f, 0.8f, 0.2f, 0.45f);

    
    public Color lockColor = new Color(1f, 1f, 1f, 1f);
    
    public Color fireColor = new Color(1f, 0.05f, 0.05f, 0.5f);

    private float shootTimer;
    private bool isShooting;
    private EyeMovement eyeMovement;
    
    private EyeController eyeController;
    
    public bool IsShooting => isShooting;
    

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[EyeLaser] No player found with tag: " + playerTag);
        }

        if (laserOrigin == null) laserOrigin = transform;

        if (laserLine == null) laserLine = GetComponent<LineRenderer>();
        if (laserLine != null)
        {
            laserLine.positionCount = 2;
            laserLine.enabled = false;
        }
        else Debug.LogWarning("[EyeLaser] No LineRenderer found!");

        eyeMovement  = GetComponent<EyeMovement>();
        shootTimer   = shootCooldown;
    }

    void Update()
    {
        if (player == null || isShooting) return;
        if (eyeController != null)
        {
            if (eyeController.isDying == true)
            {
                StopCoroutine(LaserSequence()); 
            }
        }
        bool inRange = Vector3.Distance(transform.position, player.position) <= shootRange;
        if (!inRange) return;
        
        if (eyeMovement != null && !eyeMovement.IsInShootRange) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            shootTimer = shootCooldown;
            StartCoroutine(LaserSequence());
        }
    }

 
    IEnumerator LaserSequence()
    {
        if (laserLine == null) yield break;

        
        laserLine.enabled = true;
        
        float elapsed = 0f;
        bool escaped = false;

        while (elapsed < trackingTime)
        {
            elapsed += Time.deltaTime;

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > shootRange)
            {
                escaped = true;
                break;
            }
            
            float t = elapsed / trackingTime;
            SetBeam(player.position, 0.7f, trackingColor);

            yield return null;
        }
        
        if (escaped)
        {
            yield return FadeOut(0.25f, trackingColor);
            laserLine.enabled = false;
            isShooting = false;
            shootTimer = shootCooldown * 0.5f; // shorter cooldown on cancel
            yield break;
        }
        
        Vector3 lockedTarget = player.position;
        isShooting = true;
        elapsed = 0f;
        while (elapsed < lockTime)
        {
            elapsed += Time.deltaTime;
            
            Color flashColor = (Mathf.FloorToInt(elapsed / 0.08f) % 2 == 0) ? lockColor : fireColor;
            SetBeam(lockedTarget, 1f, flashColor);

            yield return null;
        }

        elapsed = 0f;
        while (elapsed < fireDuration)
        {
            elapsed += Time.deltaTime;

            SetBeam(lockedTarget, 10f, fireColor);
            DealDamage(lockedTarget);

            yield return null;
        }
        
        yield return FadeOut(1f, fireColor);

        laserLine.enabled = false;
        isShooting = false;
    }
    
    System.Collections.IEnumerator FadeOut(float duration, Color fromColor)
    {
        float elapsed = 0f;
        Vector3 lastEnd = laserLine.GetPosition(1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / duration);
            Color faded = new Color(fromColor.r, fromColor.g, fromColor.b, alpha);
            SetBeam(lastEnd, alpha, faded);
            yield return null;
        }
    }
    
    void SetBeam(Vector3 targetPos, float widthMultiplier, Color color)
    {
        Vector3 dir = (targetPos - laserOrigin.position).normalized;

        Vector3 endPoint;
        if (Physics.Raycast(laserOrigin.position, dir, out RaycastHit hit, laserMaxLength, laserHitMask))
            endPoint = hit.point;
        else
            endPoint = laserOrigin.position + dir * laserMaxLength;

        laserLine.SetPosition(0, laserOrigin.position);
        laserLine.SetPosition(1, endPoint);

        laserLine.startWidth = 0.15f * widthMultiplier;
        laserLine.endWidth   = 0.15f * widthMultiplier;
        laserLine.startColor = color;
        laserLine.endColor   = color;
    }
    
    void DealDamage(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - laserOrigin.position).normalized;
        
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;
        var entityManager = world.EntityManager;

       
        var query = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

    
        if (!query.HasSingleton<PhysicsWorldSingleton>()) return;
        
        query.CompleteDependency();
        
        var physicsWorldSingleton = query.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;
        
        var rayInput = new RaycastInput
        {
            Start = laserOrigin.position,
            End = laserOrigin.position + dir * laserMaxLength,
            Filter = new CollisionFilter{
                BelongsTo = 1 << 2,
                CollidesWith = 1 << 1,
                GroupIndex = 0
            }
        };

        if (collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            if (entityManager.HasComponent<Health>(hit.Entity))
            {
                var health = entityManager.GetComponentData<Health>(hit.Entity);
                if (health.Value < 0) return;
                
                health.Value -= 1f;
                
                entityManager.SetComponentData(hit.Entity, health);
            }
        }
    }


}

