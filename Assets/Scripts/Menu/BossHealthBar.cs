using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private GameObject bossHealthBar;

    private void Awake()
    {
        bossHealthBar.SetActive(false);
        GameEvents.OnBossHealthChanged += UpdateBar;
        GameEvents.OnBossArenaEntered += Show;
    }

    
    private void OnDisable()
    {
        GameEvents.OnBossHealthChanged -= UpdateBar;
        GameEvents.OnBossArenaEntered -= Show;
    }

    private void Show() => bossHealthBar.SetActive(true);

    private void UpdateBar(float current, float max)
    {
        float fill = Mathf.Clamp01(current / max);
        var anchor = fillRect.anchorMax;
        anchor.x = fill;
        fillRect.anchorMax = anchor;
    }
}