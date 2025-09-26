using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Trigger_Zone : MonoBehaviour
{
    private Enemy_Base parentEnemy;

    private void Awake()
    {
        parentEnemy = GetComponentInParent<Enemy_Base>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Inside attack trigger!");
        if (other.CompareTag("Player") && !parentEnemy.isAttacking)
        {
            parentEnemy.OnAttackTriggered();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Left attack trigger!");
        if (collision.CompareTag("Player"))
        {
            parentEnemy.OnPlayerOutRange();
        }
    }
}
