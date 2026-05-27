using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePickerUI : MonoBehaviour
{
    [Header("Options")] public UpgradeDefinition[] upgradeOptions;
    
    [Header("References")]
    public GameObject container;
    public Button[] buttons;
    public TextMeshProUGUI[] nameText;
    public TextMeshProUGUI[] descriptionText;

    void OnEnable() => GameEvents.OnLevelUp += Show;
    void OnDisable() => GameEvents.OnLevelUp -= Show;

    void Show(int level)
    {
        PauseMenu.SetPaused(true);
        container.SetActive(true);

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
        PlayerStatsManager.Instance.AddModifier(
            upgrade.statType,
            new StatModifier(upgrade.value, upgrade.modifierType)
            );
        
        container.SetActive(false);
        PauseMenu.SetPaused(false);
    }
}
