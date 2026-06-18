using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class UpgradePickerUI : MonoBehaviour
{
    [Header("Options")] 
    private UpgradeDefinition[] upgradeOptions;
    public UpgradeDefinition[] upgradePool;
    
    [Header("References")]
    public GameObject container;
    public Button[] buttons;
    public TextMeshProUGUI[] nameText;
    public TextMeshProUGUI[] descriptionText;

    [Header("XP")] public GameObject xpBarContainer;

    [Header("UI")] public UpdatePerkUI updatePerkUI;

    void OnEnable() => GameEvents.OnLevelUp += Show;
    void OnDisable() => GameEvents.OnLevelUp -= Show;

    public void Start()
    {
        xpBarContainer.SetActive(true);
    }

    void Show(int level)
    {
        upgradeOptions = upgradePool.OrderBy(x => Random.value).Take(3).ToArray();
        PauseLogic.PauseGame("Pick");
        GameEvents.OnUpgradeShow?.Invoke();
        container.SetActive(true);
        xpBarContainer.SetActive(false);
        
        for (int i = 0; i < buttons.Length; i++)
        {
            var upgrade = upgradeOptions[i];
            var texts = buttons[i].GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = upgrade.displayName;
            texts[1].text = upgrade.description;
            
            buttons[i].onClick.RemoveAllListeners();
            int chosen = i;
            buttons[i].onClick.AddListener(() => Pick(chosen));
        }
    }

    void Pick(int i)
    {
        var upgrade = upgradeOptions[i];

        updatePerkUI.addPerkUI(upgrade);

        if (upgrade.category == UpgradeCategory.Stat)
            PlayerStatsManager.Instance.AddModifier(upgrade.statType, new StatModifier(upgrade.value, upgrade.modifierType));
        else if (upgrade.category == UpgradeCategory.Weapon)
        {
            if (upgrade.weaponUpgradeType == WeaponUpgradeType.Damage)
                WeaponUpgradeSystem.Instance.UpgradeDamage(upgrade.value);
            else
                WeaponUpgradeSystem.Instance.UpgradeFireRate(upgrade.value);
        }

        xpBarContainer.SetActive(true);
        container.SetActive(false);
        PauseLogic.PauseGame("Pick");
        GameEvents.OnUpgradeHide?.Invoke();
    }
}
