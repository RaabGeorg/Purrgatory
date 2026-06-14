using System.Collections.Generic;
using UnityEngine;

public class MetaApplier : MonoBehaviour
{
    public List<UpgradeDefinition> allUpgrades;
    void Start()
    {
        foreach (string name in GameData.ownedUpgrades)
        {
            UpgradeDefinition upgrade = allUpgrades.Find(u => u.displayName == name);
            if (upgrade != null)
                PlayerStatsManager.Instance.AddModifier(upgrade.statType, new StatModifier(upgrade.value, upgrade.modifierType));
        }
    }
}
