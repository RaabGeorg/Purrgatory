using System;
using UnityEngine;

public class UpdatePerkUI : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform grid;

    public void addPerkUI(UpgradeDefinition upgrade) 
    {
        Instantiate(prefab, grid).GetComponent<UpgradeSlotUI>().Setup(upgrade);
    }
}
