using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Stats")]
public class EnemyData : ScriptableObject
{
    public float maxHealth = 3f;
    public float moveSpeed = 2f;

    public float contactDamage = 1f;
    public float attackDamage = 2f;

    public GameObject deathEffect;

}
