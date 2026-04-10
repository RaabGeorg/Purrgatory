using System;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

[Serializable]
public class CharacterStats
{
    public CharacterStat baseHealth = new CharacterStat();
    public CharacterStat baseMoveSpeed = new CharacterStat();
    public CharacterStat baseAttackSpeed = new CharacterStat();
}

[Serializable]
public class CharacterStat
{
    [SerializeField] private float _baseValue;
    
    private readonly List<StatModifier> statModifiers;
    private float _value;
    private bool isDirty = true;

    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
    }
    
    public float BaseValue{
            get { return _baseValue; }
            set {
                _baseValue = value;
                isDirty = true;
            }
        }
    public float Value
    {
        get
        {
            if (isDirty)
            {
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    public void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        isDirty = true;
        return statModifiers.Remove(mod);
    }

    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            
            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            
            else if (mod.Type == StatModType.Percent)
            {
                finalValue *= 1 + mod.Value;
            }
        }
        return (float)Math.Round(finalValue, 4);
    }

    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order > b.Order)
        {
            return 1;
        }
        else if (a.Order < b.Order)
        {
            return -1;
        }
        return 0;
    }
}

public class StatModifier
{
    public readonly float Value;
    public readonly StatModType Type;
    public readonly int Order;
    
    public StatModifier(float value, StatModType type,int order)
    {
        Value = value;
        Type = type;
        Order = order;
    }
    public StatModifier(float value, StatModType type) : this (value, type, (int) type) {}
}

public enum StatModType
{
    Flat,
    Percent
}