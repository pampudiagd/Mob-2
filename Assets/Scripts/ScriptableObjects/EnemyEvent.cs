using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/EnemyEvent")]
public class EnemyEvent : ScriptableObject
{
    public UnityEvent onEnemyDeath;
    public UnityEvent onEnemyHit;

    public void RaiseEnemyDeath()
    {
        onEnemyDeath?.Invoke();
    }

    public void RaiseEnemyHit()
    {
        onEnemyHit?.Invoke();
    }
}