using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Stats")]
public class EnemyData : CharacterData
{
    public float contactDamage = 1f;
    public float attackDamage = 2f;

    public string damageSource;
    public DamageType damageType = DamageType.Normal;

    //Boolean added to determine IMMORTALITY
    public bool mortal = true;

    public GameObject deathEffect;
    public GameObject hitEffect;

}
