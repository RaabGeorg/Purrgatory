using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlotUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image rarity;

    public void Setup(UpgradeDefinition upgrade) 
    {
        icon.sprite = upgrade.icon;
        rarity.color = GetColor(upgrade.rarity);
    }

    Color GetColor(RarityTier rarity) 
    {
        return rarity switch
        {
            RarityTier.Common => Color.green,
            RarityTier.Uncommon => Color.blue,
            RarityTier.Rare => Color.purple,
            _ => Color.white
        };
    }
}
