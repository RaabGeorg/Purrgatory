using Components;
using System.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class LaserCircleState : IState
{
    private readonly BossController _boss;

    private float cooldown = 0.3f;
    private float telegraphDuration = 1.5f;
    private float fireDuration = 0.5f;
    private float fadeInDuration = 0.2f;
    private float fadeOutDuration = 0.3f;
    private float telegraphWidth = 0.05f;
    private float fireWidth = 1f;
    private float laserLength = 150f;
    private float damage = 0.5f;
    private Color telegraphColor = new Color(1f, 0.3f, 0.3f, 0.4f);
    private Color fireColor = new Color(1f, 0f, 0f, 1f);

    private float nextFire;
    private int shotsLeft;
    private float oldSpeed;

    public LaserCircleState(BossController boss) => _boss = boss;

    public void Enter()
    {
        oldSpeed = _boss.speed;
        _boss.speed = 15f;
        nextFire = 0f;

        if (true)
        {
            shotsLeft = 66;
            cooldown = 0.05f;
        }
        else 
        {
            shotsLeft = 22;
            cooldown = 0.3f;
        }
    }

    public void Tick()
    {
        if (shotsLeft > 0 && Time.time >= nextFire)
        {
            nextFire = Time.time + cooldown;
            shotsLeft--;

            Vector3 origin = _boss.transform.position;
            Vector3 dir = (_boss.player.position - origin);
            dir.y = 0f;
            dir.Normalize();
            Vector3 target = origin + dir * laserLength;

            FireStaticLaser(origin, target);
        }
    }

    public void Exit()
    {
        _boss.speed = oldSpeed;
    }

    private void FireStaticLaser(Vector3 origin, Vector3 target)
    {
        var obj = GameObject.Instantiate(_boss.laserPrefab);
        var line = obj.GetComponent<LineRenderer>();
        _boss.StartCoroutine(StaticLaserSequence(line, origin, target, obj));
    }

    private IEnumerator StaticLaserSequence(LineRenderer line, Vector3 origin, Vector3 target, GameObject obj)
    {
        line.enabled = true;
        line.SetPosition(0, origin);
        line.SetPosition(1, target);

        // Fade in telegraph
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            line.startWidth = Mathf.Lerp(0f, telegraphWidth, t);
            line.endWidth = Mathf.Lerp(0f, telegraphWidth, t);
            SetAlpha(line, Mathf.Lerp(0f, telegraphColor.a, t));
            yield return null;
        }

        line.startWidth = telegraphWidth;
        line.endWidth = telegraphWidth;
        line.startColor = telegraphColor;
        line.endColor = telegraphColor;

        // Telegraph hold
        elapsed = 0f;
        while (elapsed < telegraphDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to fire
        line.startWidth = fireWidth;
        line.endWidth = fireWidth;
        line.startColor = fireColor;
        line.endColor = fireColor;
        elapsed = 0f;
        while (elapsed < fireDuration)
        {
            elapsed += Time.deltaTime;
            DealLaserDamage(origin, target);
            yield return null;
        }

        // Fire hold
        yield return new WaitForSeconds(fireDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            float w = Mathf.Lerp(fireWidth, 0f, t);
            line.startWidth = w;
            line.endWidth = w;
            SetAlpha(line, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        GameObject.Destroy(obj);

        if (shotsLeft == 0)
            _boss.SwitchState(new IdleState(_boss));
    }

    private void SetAlpha(LineRenderer line, float alpha)
    {
        Color sc = line.startColor;
        Color ec = line.endColor;
        sc.a = alpha;
        ec.a = alpha;
        line.startColor = sc;
        line.endColor = ec;
    }

    private void DealLaserDamage(Vector3 origin, Vector3 target)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var physicsWorldSingleton = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton))
            .GetSingleton<PhysicsWorldSingleton>();

        var rayInput = new RaycastInput
        {
            Start = origin,
            End = target,
            Filter = new CollisionFilter
            {
                BelongsTo = 1 << 2,
                CollidesWith = 1 << 1,
                GroupIndex = 0
            }
        };

        if (physicsWorldSingleton.CollisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            if (!entityManager.HasComponent<Health>(hit.Entity)) return;
            var health = entityManager.GetComponentData<Health>(hit.Entity);
            if (health.Value <= 0) return;
            health.Value -= damage;
            entityManager.SetComponentData(hit.Entity, health);
        }
    }
}