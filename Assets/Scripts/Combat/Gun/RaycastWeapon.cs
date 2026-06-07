
using System.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Mathematics;

 
public class RaycastWeapon : MonoBehaviour
{
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private float laserDuration = 0.03f;
    private Coroutine laserCoroutine;
    [SerializeField] private GameObject projectilePrefab;  
    [SerializeField] private Transform firePoint;          
    [SerializeField] private float fireRate = 0.2f;        
 
    private float nextFireTime = 0f;
    private PlayerControls playerControls;

    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    void Update()
    {
        if (playerControls.Player.Fire.IsPressed() &&
            Time.time >= this.nextFireTime)
        {
            this.Shoot();
            this.nextFireTime = Time.time + this.fireRate;
        }
    }

    private void Shoot()
    {
        // Hole CollisionWorld über EntityManager statt SystemAPI
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        var physicsWorldSingleton = entityManager.CreateEntityQuery(
                typeof(PhysicsWorldSingleton))
            .GetSingleton<PhysicsWorldSingleton>();

        var collisionWorld = physicsWorldSingleton.CollisionWorld;
        
        SFXManager.Instance.ShootLaser();

        var rayInput = new RaycastInput
        {
            Start = this.firePoint.position,
            End = this.firePoint.position + this.firePoint.forward * 100f,
            Filter = CollisionFilter.Default
        };
        Vector3 endPoint = this.firePoint.position + this.firePoint.forward * 100f;
        
        if (collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            Debug.LogWarning("hit");
            Entity hitEntity = hit.Entity;
            endPoint = hit.Position;
            entityManager.DestroyEntity(hit.Entity);

        }
        if (this.laserCoroutine != null)
            StopCoroutine(this.laserCoroutine);
        this.laserCoroutine = StartCoroutine(ShowLaser(this.firePoint.position, endPoint));
    }
    private IEnumerator ShowLaser(Vector3 start, Vector3 end)
    {
        this.laserRenderer.SetPosition(0, start);
        this.laserRenderer.SetPosition(1, end);
        this.laserRenderer.enabled = true;

        yield return new WaitForSeconds(this.laserDuration);

        this.laserRenderer.enabled = false;
    }
}