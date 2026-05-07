using System;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI playerHealthText;
    public Character targetCharacter;

    private void Update()
    {
        if (targetCharacter != null && playerHealthText != null)
        {
            float currentHP = targetCharacter.playerStats.baseHealth.Value;
            
            playerHealthText.text = $"Health: {currentHP}";
        }
    }
}
