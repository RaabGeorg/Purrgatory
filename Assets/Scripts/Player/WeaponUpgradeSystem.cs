using Components;
using Unity.Entities;
using UnityEngine;

public class WeaponUpgradeSystem : MonoBehaviour
{
    public static WeaponUpgradeSystem Instance { get; private set; }
    private EntityManager _em;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void UpgradeDamage(float amount)
    {
        var query = _em.CreateEntityQuery(typeof(Weapon), typeof(WeaponFromPlayerTag));
        if (!query.HasSingleton<WeaponFromPlayerTag>()) return;

        var entity = query.GetSingletonEntity();
        var weapon = _em.GetComponentData<Weapon>(entity);
        weapon.Damage *= 1 + amount;
        _em.SetComponentData(entity, weapon);
        //Debug.Log($"Weapon damage upgraded to: {weapon.Damage}");
    }

    public void UpgradeFireRate(float amount)
    {
        var query = _em.CreateEntityQuery(typeof(Weapon), typeof(WeaponFromPlayerTag));
        if (!query.HasSingleton<WeaponFromPlayerTag>()) return;

        var entity = query.GetSingletonEntity();
        var weapon = _em.GetComponentData<Weapon>(entity);
        weapon.FireRate *= 1 + amount;
        _em.SetComponentData(entity, weapon);
         //Debug.Log($"AttackSpeed: {weapon.FireRate}");
    }
}