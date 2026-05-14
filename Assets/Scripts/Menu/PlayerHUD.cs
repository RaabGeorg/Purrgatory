using System;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI playerHealthText;
    public PlayerMovement player;

    private void Update()
    {
        if (player != null && playerHealthText != null)
        {
            float currentHP = player.stats.baseHealth.Value;
            
            playerHealthText.text = $"Health: {currentHP}";
        }
        //Debug.Log(player.stats.baseHealth.Value);
    }
}
