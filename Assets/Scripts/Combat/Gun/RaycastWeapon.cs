using System.Collections;
using UnityEngine;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

public class RaycastWeapon : MonoBehaviour
{
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private float laserDuration = 0.03f;
    private Coroutine laserCoroutine;
    [SerializeField] private Transform firePoint;
    [SerializeField] public float fireRate = 0.2f;
    [SerializeField] public float damage = 20f;
    
    [Header("Overcharge")]
    [SerializeField] public bool overchargeUnlocked = false;
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float overchargeCooldown = 3f;
    [SerializeField] private Gradient chargeColorGradient;
    
    private float chargeStartTime = -1f;
    private bool isCharging = false;
    private bool overchargeReady = true;
    private float overchargeCooldownTimer = 0f;

    private float nextFireTime = 0f;
    private PlayerControls playerControls;

    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    void Update()
    {
        if (!GameData.Weapon.Equals("Railgun"))return;
        
        if (!overchargeUnlocked)
        {
            if (playerControls.Player.Fire.IsPressed() && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
                
            }
            return;
        }

        if (!overchargeReady)
        {
            overchargeCooldownTimer -= Time.deltaTime;
            if (overchargeCooldownTimer <= 0f)
                overchargeReady = true;
        }

        
        if (overchargeReady && playerControls.Player.FireHold.IsPressed())
        {
            if (!isCharging)
            {
                isCharging = true;
                chargeStartTime = Time.time;
            }
            
            float chargeProgress = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);
            float width = Mathf.Lerp(0.05f, 1f, chargeProgress);
            laserRenderer.startWidth = width;
            laserRenderer.endWidth   = width;
            laserRenderer.startColor = chargeColorGradient.Evaluate(chargeProgress);
            laserRenderer.endColor   = chargeColorGradient.Evaluate(chargeProgress);

            
            Vector3 direction = firePoint.forward;
            direction.y = 0;
            direction.Normalize();
            laserRenderer.SetPosition(0, firePoint.position);
            laserRenderer.SetPosition(1, firePoint.position + direction * 100f);
            laserRenderer.enabled = true;

            return; 
        }

       
        if (isCharging && !playerControls.Player.FireHold.IsPressed())
        {
            isCharging = false;
            float chargeProgress = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);
            ShootOvercharge(chargeProgress);

            
            laserRenderer.startWidth = 0.1f;
            laserRenderer.endWidth   = 0.1f;
            overchargeReady = false;
            overchargeCooldownTimer = overchargeCooldown;
            return;
        }
        
        if (playerControls.Player.Fire.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
                
        }
        
    }

    private void ShootOvercharge(float chargeProgress)
    {
        var world             = World.DefaultGameObjectInjectionWorld;
        var entityManager     = world.EntityManager;
        var physicsWorldSingleton = entityManager
            .CreateEntityQuery(typeof(PhysicsWorldSingleton))
            .GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;

        Vector3 direction = firePoint.forward;
        direction.y = 0;
        direction.Normalize();

        SFXManager.Instance.ShootLaser(); 

        var rayInput = new RaycastInput
        {
            Start  = firePoint.position,
            End    = firePoint.position + direction * 100f,
            Filter = new CollisionFilter
            {
                BelongsTo    = 1 << 3,
                CollidesWith = 1 << 2,
                GroupIndex   = 0
            }
        };

        
        var hits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);
        collisionWorld.CastRay(rayInput, ref hits);

        float damage = Mathf.Lerp(this.damage, this.damage + 60, chargeProgress); 
        foreach (var hit in hits)
            DamageEntity(entityManager, hit.Entity, damage);

        hits.Dispose();
        
        float width = Mathf.Lerp(0.1f, 1f, chargeProgress);
        laserRenderer.startWidth = width;
        laserRenderer.endWidth   = width;

        if (laserCoroutine != null) StopCoroutine(laserCoroutine);
        laserCoroutine = StartCoroutine(ShowLaser(firePoint.position,
            firePoint.position + direction * 100f));
    }

    private void Shoot()
    {
        var world             = World.DefaultGameObjectInjectionWorld;
        var entityManager     = world.EntityManager;
        var physicsWorldSingleton = entityManager
            .CreateEntityQuery(typeof(PhysicsWorldSingleton))
            .GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;

        Vector3 direction = firePoint.forward;
        direction.y = 0;
        direction.Normalize();

        SFXManager.Instance.ShootLaser();

        var rayInput = new RaycastInput
        {
            Start  = firePoint.position,
            End    = firePoint.position + direction * 100f,
            Filter = new CollisionFilter
            {
                BelongsTo    = 1 << 3,
                CollidesWith = 1 << 2,
                GroupIndex   = 0
            }
        };

        Vector3 endPoint = firePoint.position + direction * 100f;

        if (collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            DamageEntity(entityManager, hit.Entity, damage);
            endPoint = hit.Position;
        }

        if (laserCoroutine != null) StopCoroutine(laserCoroutine);
        laserCoroutine = StartCoroutine(ShowLaser(firePoint.position, endPoint));
    }

    private void DamageEntity(EntityManager em, Entity entity, float damage)
    {
        if (!em.HasComponent<Health>(entity)) return;
        var health = em.GetComponentData<Health>(entity);
        if (health.Value < 0) return;
        health.Value -= damage;
        em.SetComponentData(entity, health);
    }

    private IEnumerator ShowLaser(Vector3 start, Vector3 end)
    {
        laserRenderer.SetPosition(0, start);
        laserRenderer.SetPosition(1, end);
        laserRenderer.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserRenderer.enabled = false;
    }

    void OnDisable() => playerControls.Disable();
    void OnDestroy() => playerControls.Disable();
}