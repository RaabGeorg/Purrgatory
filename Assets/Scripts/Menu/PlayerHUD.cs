using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI condensedSoulsText;
    public TextMeshProUGUI soulsText;
    public TextMeshProUGUI currentXpText;
    public TextMeshProUGUI xpToNext;
    public RectTransform xpBarRect;
    private float _maxWidth;

    void Start() => _maxWidth = xpBarRect.sizeDelta.x;
    private void OnEnable()
    {
        GameEvents.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= UpdateHealth;
    }
    private void UpdateHealth(float hp)
    {
        if (playerHealthText != null)
            playerHealthText.text = $"Health: {hp}";
    }

    private void Update()
    {
        float fill = (float)PlayerXP.Instance.CurrentXp / PlayerXP.Instance.XpToNextLevel;
        xpBarRect.sizeDelta = new Vector2(_maxWidth * fill, xpBarRect.sizeDelta.y);
        if (soulsText != null)
            soulsText.text = $"Souls: {PlayerWallet.Instance.Souls}";

        if (condensedSoulsText != null)
            condensedSoulsText.text = $"CondensedSouls: {PlayerWallet.Instance.CondensedSouls}";
        if (currentXpText != null)
            currentXpText.text = $"Level: {PlayerXP.Instance.CurrentLevel}";
        if (xpToNext != null)
            xpToNext.text = $"XP: {PlayerXP.Instance.CurrentXp }/{PlayerXP.Instance.XpToNextLevel}";
    }
}
