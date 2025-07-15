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

public class StatModifier : MonoBehaviour
{
    public float value;
    public ModifierType type;

    public StatModifier(float value, ModifierType type)
    {
        this.value = value;
        this.type = type;
    }
}
