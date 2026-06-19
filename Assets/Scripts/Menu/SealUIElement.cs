using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SealUIElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeText;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] Button buyButton;
    [SerializeField] bool bought;
    [SerializeField] int price;


    private void Start()
    {
        if (GameData.HealthSeal == 1) 
        {
            bought = true;
        }

        if (!bought) cost.text = $"Con. Souls: " + price.ToString();
        buyButton.onClick.AddListener(BuyUpgrade);
        UpdateElement();
    }

    public void BuyUpgrade()
    {
        if (bought) return;

        if (GameData.CondensedSouls < price) return;

        GameData.CondensedSouls -= price;
        Debug.Log(upgradeText.text);

        if (upgradeText.text.Equals("HealthSeal"))
        {
            GameData.HealthSeal = 1;
            bought = true;
        }
        else if (false)
        {
            return;
        } else 
        {
            return;
        }


        GameData.Save();
        UpdateElement();
    }

    void UpdateElement()
    {
        if (bought)
        {
            cost.text = "Con. Souls: MAX";
            buyButton.interactable = false;
        }
    }

}
