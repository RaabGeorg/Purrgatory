using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeUIElement : MonoBehaviour
{
    [SerializeField] UpgradeDefinition upgrade;
    
    [SerializeField] TextMeshProUGUI upgradeText;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] Button buyButton;

    private void Start()
    {
        buyButton.onClick.AddListener(BuyUpgrade);
        UpdateElement();
    }

    public void BuyUpgrade()
    {
        int level = GameData.GetLevel(upgrade.displayName);
        if (level >= upgrade.maxLevel) return;

        int price = upgrade.cost * (level + 1);
        if (GameData.Souls < price) return;

        GameData.Souls -= price;
        GameData.AddUpgrade(upgrade.displayName);
        GameData.Save();
        UpdateElement();
    }

    void UpdateElement()
    {
        int currentLevel = GameData.GetLevel(upgrade.displayName);

        upgradeText.text = upgrade.displayName;
        level.text = $"{currentLevel}/{upgrade.maxLevel}";

        if (currentLevel >= upgrade.maxLevel)
        {
            cost.text = "Souls: MAX";
            buyButton.interactable = false;
        }
        else
        {
            int price = upgrade.cost * (currentLevel + 1);
            cost.text = $"Souls: {price.ToString()}";
            buyButton.interactable = GameData.Souls >= price;
        }
    }

}
