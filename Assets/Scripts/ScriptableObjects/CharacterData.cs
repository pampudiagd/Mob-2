using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterData : ScriptableObject
{
    [Tooltip("MUST be a multiple of 4.")]
    public float baseMaxHealth;
    public float basePower;
    public float baseSpeed;
}
