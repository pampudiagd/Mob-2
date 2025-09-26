using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    Additive,
    Multiplicative,
    Subtractive,
    Divisive
}

public enum StatType
{
    Health,
    Energy,
    Attack,
    Ammo,
    Speed
}

public class StatModifier
{
    public float value;
    public ModifierType type;
    public StatType targetStat;

    public StatModifier(float value, ModifierType type, StatType targetStat)
    {
        this.value = value;
        this.type = type;
        this.targetStat = targetStat;
    }
}
