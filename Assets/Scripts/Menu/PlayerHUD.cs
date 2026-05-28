using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI playerHealthText;
    public PlayerMovement player;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI soulsText;
    

    private void Update()
    {
        if (player != null && playerHealthText != null)
        {
            float currentHP = player.stats.baseHealth.Value;
            
            playerHealthText.text = $"Health: {currentHP}";
        }

        if (player != null && coinsText != null && soulsText != null)
        {
            int currentCoins = PlayerWallet.Instance.Coins;
            int currentSouls = PlayerWallet.Instance.Souls;
            
            coinsText.text = $"Coins: {currentCoins}";
            soulsText.text = $"Souls: {currentSouls}";
        }
    }
}
