using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detection_Zone : MonoBehaviour
{
    private Enemy_Base parentEnemy;

    private enum ZoneType
    {
        Inner,
        Outer,
        Both
    }

    [SerializeField] private ZoneType type;

    private void Awake()
    {
        parentEnemy = GetComponentInParent<Enemy_Base>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && type != ZoneType.Outer)
        {
            parentEnemy.isTargetSeen = true;

            if (!parentEnemy.isAttacking)
                parentEnemy.OnPlayerDetected();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && type != ZoneType.Inner)
        {
            parentEnemy.isTargetSeen = false;
            parentEnemy.OnPlayerLost();
        }
    }
}
