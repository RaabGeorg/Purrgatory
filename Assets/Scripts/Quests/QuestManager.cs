using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public bool IsQuestActive { get; private set; }
    public bool IsQuestComplete { get; private set; }

    private QuestData _activeQuest;
    private int _meleeKills;
    private int _rangedKills;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartQuest(QuestData quest)
    {
        _activeQuest = quest;
        _meleeKills = 0;
        _rangedKills = 0;
        IsQuestActive = true;
        IsQuestComplete = false;
        GameEvents.OnQuestProgressChanged?.Invoke();
    }

    public void RegisterKill(EnemyKillType type)
    {
        if (!IsQuestActive) return;

        if (type == EnemyKillType.Melee) _meleeKills++;
        else if (type == EnemyKillType.Ranged) _rangedKills++;

        GameEvents.OnQuestProgressChanged?.Invoke();

        int relevantKills = _activeQuest.requiredType == EnemyKillType.Any
            ? _meleeKills + _rangedKills
            : (_activeQuest.requiredType == EnemyKillType.Melee ? _meleeKills : _rangedKills);

        if (relevantKills >= _activeQuest.requiredCount)
        {
            IsQuestActive = false;
            IsQuestComplete = true;
            GameEvents.OnQuestCompleted?.Invoke();
        }
    }

    public string GetProgressText()
    {
        if (_activeQuest == null) return "";

        int relevantKills = _activeQuest.requiredType == EnemyKillType.Any
            ? _meleeKills + _rangedKills
            : (_activeQuest.requiredType == EnemyKillType.Melee ? _meleeKills : _rangedKills);

        return $"{_activeQuest.questName}: {relevantKills}/{_activeQuest.requiredCount}";
    }

    public QuestData ActiveQuest => _activeQuest;
}