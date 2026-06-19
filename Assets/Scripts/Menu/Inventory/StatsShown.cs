using Components;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class StatsShown : MonoBehaviour
{
    public TextMeshProUGUI statsField;
    private EntityManager _em;

    float maxHealth = 0;
    float moveSpeed = 0;
    float damage = 0;
    float fireRate = 0;
    private void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void UpdateStatsUI() 
    {
        if (_em == default)
        {
            if (World.DefaultGameObjectInjectionWorld != null)
            {
                _em = World.DefaultGameObjectInjectionWorld.EntityManager;
            }
            else
            {
                return;
            }
        }

        var query = _em.CreateEntityQuery(typeof(Weapon), typeof(WeaponFromPlayerTag));
        if (!query.HasSingleton<WeaponFromPlayerTag>()) return;

        var entity = query.GetSingletonEntity();
        var weapon = _em.GetComponentData<Weapon>(entity);

        maxHealth = PlayerStatsManager.Instance.stats.baseHealth.Value;
        moveSpeed = PlayerStatsManager.Instance.stats.baseMoveSpeed.Value;

        if (GameData.Weapon.Equals("Railgun"))
        {

        } else 
        {
            fireRate = weapon.FireRate;
            damage = weapon.Damage;
        }

        string characterStats = $"Max Health: {maxHealth}\nMove Speed: {moveSpeed}\nDamage: {damage}\nFire Rate: {fireRate}";
        statsField.text = characterStats;
    }
}
