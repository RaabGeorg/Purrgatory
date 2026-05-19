using System;

public static class GameEvents
{
    public static Action<int> OnXPGained;
    public static Action<int> OnLevelUp;
    public static Action<float> OnHealthChanged;
}