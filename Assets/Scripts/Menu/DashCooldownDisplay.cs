using UnityEngine;
using UnityEngine.UI;

public class DashCooldownDisplay : MonoBehaviour
{
    public PlayerMovement player;
    [SerializeField] private Image[] dashFillImages;
    [SerializeField] private Color readyColor = Color.darkOliveGreen;
    [SerializeField] private Color rechargeColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private void Update()
    {
        if (player == null) return;

        int count = (int)player.dashCount;

        for (int i = 0; i < dashFillImages.Length; i++)
        {
            float fill;
            Color color;

            if (i < count)
            {
                fill = 1f;
                color = readyColor;
            }
            else if (i == count && player.isRecharging)
            {
                fill = player._rechargeProgress;
                color = rechargeColor;
            }
            else
            {
                fill = 0f;
                color = rechargeColor;
            }

            dashFillImages[i].fillAmount = fill;
            dashFillImages[i].color = color;
        }
    }
}