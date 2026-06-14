using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "Scriptable Objects/UpgradeDefinition")]
public class UpgradeDefinition : ScriptableObject
{
    public Sprite icon;
    public string displayName;
    [TextArea] public string description;
    public RarityTier rarity;
    public StatType statType;
    public StatModType modifierType;
    public float value;
    public UpgradeCategory category;
    public WeaponUpgradeType weaponUpgradeType;

    public int cost;
    public int maxLevel;
}

public enum RarityTier
{
    Common,
    Uncommon,
    Rare,
}

public enum UpgradeCategory { Stat, Weapon }
public enum WeaponUpgradeType { Damage, FireRate }