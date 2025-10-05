using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Stats")]
public class EnemyData : CharacterData
{
    public int pointValue = 10;

    public string damageSource;
    public DamageType damageType = DamageType.Normal;

    //Boolean added to determine IMMORTALITY
    public bool mortal = true;
    public bool canFly = false;

    public GameObject deathEffect;
    public GameObject hitEffect;

}
