using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/EnemyDeathEvent")]
public class EnemyDeathEvent : ScriptableObject
{
    public UnityEvent onEnemyDeath;

    public void Raise()
    {
        onEnemyDeath?.Invoke();
    }
}
