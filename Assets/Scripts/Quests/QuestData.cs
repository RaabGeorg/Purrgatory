using UnityEngine;

public enum EnemyKillType { Melee, Ranged, Any }

[CreateAssetMenu(fileName = "QuestData", menuName = "Quests/QuestData")]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public EnemyKillType requiredType;
    public int requiredCount;
    public WeaponModReward requiredModType;
}

public enum WeaponModReward { None, WeaponModUnlock, BossUnlock }