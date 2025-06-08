using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sword", menuName = "Items/Sword")]
public class SwordData : ScriptableObject
{
    public string swordName;
    public float damage;
    public Sprite attackSprite;
    public GameObject swordAttackPrefab;
}
