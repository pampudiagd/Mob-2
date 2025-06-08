using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sword", menuName = "Items/Gun")]
public class GunData : ScriptableObject
{
    public string gunName;
    public float damage;
    public Sprite gunSprite;
    public Sprite bulletSprite;
    public GameObject GunAttackPrefab;

}
