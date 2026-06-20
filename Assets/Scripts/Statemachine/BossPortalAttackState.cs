using Unity.Entities;
using UnityEngine;
using Components;

public class BossPortalAttackState : IState
{
    private readonly BossController _boss;
    private EntityManager _em;
    private Entity _phaseEntity;
    private float _duration;
    private float _timer;
    private bool first = true;

    public BossPortalAttackState(BossController boss, float duration = 10f)
    {
        _boss     = boss;
        _duration = duration;
    }
    public void Enter()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        _phaseEntity = _em.CreateEntity();
        _em.AddComponent<BossPortalPhaseActive>(_phaseEntity);
        _timer = 0f;
        
        
        
        Debug.Log("Boss Portal Attack – START");
    }

    public void Tick()
    {
         _timer += Time.deltaTime;
         if (_timer >= _duration)
         {
             if (first)
             {
                 // didi mach alle zu FLowerPetal Pattern, is not playable
                 SetPattern(AttackPattern.FlowerPetal, 4);
                 first = false;
             }
            _boss.SwitchState(new IdleState(_boss));
        }
    }

    public void Exit()
    {
        if (_em.Exists(_phaseEntity))
            _em.DestroyEntity(_phaseEntity);

        Debug.Log("Boss Portal Attack – END");
    }
    
    private void SetPattern(AttackPattern pattern, int PatternsToChange)
    {
        
        var query = _em.CreateEntityQuery(typeof(BossPortal));
        var portals = query.ToEntityArray(Unity.Collections.Allocator.Temp);
        Debug.Log(portals.Length);
        
        foreach (var entity in portals)
        {
            if (PatternsToChange <= 0)  break;
            var portal = _em.GetComponentData<BossPortal>(entity);
            portal.Pattern      = pattern;
            portal.CurrentAngle = 0f;
            portal.FireCooldown = 0f;
            _em.SetComponentData(entity, portal);
            PatternsToChange--;
            
        }

        portals.Dispose();
    }
}