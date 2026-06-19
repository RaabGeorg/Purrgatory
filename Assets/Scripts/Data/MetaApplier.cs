using Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MetaApplier : MonoBehaviour
{
    public List<UpgradeDefinition> allUpgrades;

    void Start()
    {
        StartCoroutine(LateStart(1));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Apply();
    }

    void Apply()
    {
        foreach (string name in GameData.ownedUpgrades)
        {
            UpgradeDefinition upgrade = allUpgrades.Find(u => u.displayName == name);
            if (upgrade != null)
                if (upgrade.category == UpgradeCategory.Stat)
                    PlayerStatsManager.Instance.AddModifier(upgrade.statType, new StatModifier(upgrade.value, upgrade.modifierType));
                else if (upgrade.category == UpgradeCategory.Weapon)
                {
                    if (upgrade.weaponUpgradeType == WeaponUpgradeType.Damage)
                    {
                        WeaponUpgradeSystem.Instance.UpgradeDamage(new StatModifier(upgrade.value, upgrade.modifierType));
                    }
                    else
                    {
                        WeaponUpgradeSystem.Instance.UpgradeFireRate(new StatModifier(upgrade.value, upgrade.modifierType));
                    }
                }
        }
    }
}
