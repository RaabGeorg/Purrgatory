using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public static PlayerXP Instance { get; private set; }

    [SerializeField] private int currentXp;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int xpToNextLevel;

    public int CurrentXp => currentXp;
    public int CurrentLevel => currentLevel;
    public int XpToNextLevel => xpToNextLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        xpToNextLevel = XpForLevel(currentLevel);
    }

    private void OnEnable()  => GameEvents.OnXPGained += AddXP;
    private void OnDisable() => GameEvents.OnXPGained -= AddXP;

    private void AddXP(int amount)
    {
        Debug.Log($"Xp gained -> XP: {CurrentXp} (+{amount})");
        currentXp += amount;
        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = XpForLevel(currentLevel);
            GameEvents.OnLevelUp?.Invoke(currentLevel);
        }
    }

    private static int XpForLevel(int level) =>
        //(int)(100 * Mathf.Pow(level, 1.6f)) + (level * 17);
        level * 200;
}