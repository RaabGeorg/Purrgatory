using System;

public static class GameEvents
{
    public static Action<int> OnXPGained;
    public static Action<int> OnLevelUp;
    public static Action<float> OnHealthChanged;
    public static Action OnStatsChanged;
    public static Action OnUpgradeShow;
    public static Action OnUpgradeHide;
}