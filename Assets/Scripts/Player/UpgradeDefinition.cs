using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "Scriptable Objects/UpgradeDefinition")]
public class UpgradeDefinition : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public RarityTier rarity;
    public StatType statType;
    public StatModType modifierType;
    public float value;
}

public enum RarityTier
{
    Common,
    Uncommon,
    Rare,
}