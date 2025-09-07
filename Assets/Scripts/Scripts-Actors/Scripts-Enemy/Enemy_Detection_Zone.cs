using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detection_Zone : MonoBehaviour
{
    private Enemy_Base parentEnemy;

    private void Awake()
    {
        parentEnemy = GetComponentInParent<Enemy_Base>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentEnemy.OnPlayerDetected();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        {
            parentEnemy.OnPlayerLost();
        }
    }
}
