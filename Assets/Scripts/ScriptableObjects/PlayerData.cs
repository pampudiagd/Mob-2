using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Player/Stats")]
public class PlayerData : CharacterData
{
    [Tooltip("MUST be a multiple of 4.")]
    public int baseMaxAmmo = 4;

    public float BaseRollSpeed = 40f;
    public float baseRollInvulWindow = 0.07f;

    public float baseMaxEnergy = 20;
    public int baseLives = 0;
}
