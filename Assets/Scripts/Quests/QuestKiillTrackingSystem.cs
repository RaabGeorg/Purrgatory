using Components;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class QuestKillTrackingSystem : SystemBase
{
    private EntityQuery _query;

    protected override void OnCreate()
    {
        _query = GetEntityQuery(
            ComponentType.ReadOnly<Enemy>(),
            ComponentType.ReadOnly<Executed>(),
            ComponentType.Exclude<KillCounted>());
    }

    protected override void OnUpdate()
    {
        if (!QuestManager.Instance.IsQuestActive) return;

        var entities = _query.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            bool isRanged = EntityManager.HasComponent<WeaponTarget>(entity);
            QuestManager.Instance.RegisterKill(isRanged ? EnemyKillType.Ranged : EnemyKillType.Melee);
            EntityManager.AddComponent<KillCounted>(entity);
        }
        entities.Dispose();
    }
}