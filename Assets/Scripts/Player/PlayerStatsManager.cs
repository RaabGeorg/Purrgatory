using Components;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    public CharacterStats stats = new CharacterStats();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    
    private void Start()
    {
        
    }

    private void OnEnable() => GameEvents.OnLevelUp += HandleLevelUp; 
    private void OnDisable() => GameEvents.OnLevelUp -= HandleLevelUp;

    private void HandleLevelUp(int newLevel)
    {
        // upgradesystem will call addmodifier after player picks
        PushToBridge();
    }

    public void AddModifier(StatType statType, StatModifier mod)
    {
        switch (statType)
        {
            case StatType.MoveSpeed:   stats.baseMoveSpeed.AddModifier(mod);   break;
            case StatType.AttackSpeed: stats.baseAttackSpeed.AddModifier(mod); break;
            case StatType.Damage:      stats.Damage.AddModifier(mod);          break;
            case StatType.MaxHealth:   stats.baseHealth.AddModifier(mod);      break;
        }
        PushToBridge();
    }

    public void PushToBridge()
    {
        GameEvents.OnStatsChanged.Invoke();
        var newHealth = Instance.stats.baseHealth.Value;
        PlayerBridge.Instance?.ApplyStats(new PlayerStatsComponent
        {
            AttackSpeed = stats.baseAttackSpeed.Value,
            Damage      = stats.Damage.Value,
        });
        PlayerBridge.Instance?.ApplyHealth(newHealth);
    }
}