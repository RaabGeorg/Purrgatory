using System;
using UnityEngine;
using TMPro;

public class DashCooldownDisplay : MonoBehaviour
{
    public PlayerMovement player;
    public TextMeshProUGUI dashText;


    private void Update()
    {
        if (player == null || dashText == null) return;

        float count = player.dashCount;
        
        if (count == 0f){
            dashText.text = $"Dash: {count}";
            dashText.color = Color.white;
        }
        else
        {
            dashText.text = $"Dash: {count}";
            dashText.color = Color.green;
        }
        /*
        float timer = player.GetDashTimer();

        if (timer > 0f){
            dashText.text = $"Dash: {timer:F1}";
            dashText.color = Color.white;
        }
        else
        {
            dashText.text = "Dash: READY";
            dashText.color = Color.green;
        }
        */
    }
}
