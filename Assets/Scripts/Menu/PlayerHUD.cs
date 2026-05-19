using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI condensedSoulsText;
    public TextMeshProUGUI soulsText;
    
    private void OnEnable()  => GameEvents.OnHealthChanged += UpdateHealth;
    private void OnDisable() => GameEvents.OnHealthChanged -= UpdateHealth;

    private void UpdateHealth(float hp)
    {
        if (playerHealthText != null)
            playerHealthText.text = $"Health: {hp}";
    }

    private void Update()
    {
        if (soulsText != null)
            soulsText.text = $"Souls: {PlayerWallet.Instance.Souls}";

        if (condensedSoulsText != null)
            condensedSoulsText.text = $"CondensedSouls: {PlayerWallet.Instance.CondensedSouls}";
    }
}
