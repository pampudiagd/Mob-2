using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[System.Serializable]
//public class 

[CreateAssetMenu(menuName = "Events/EnemyEvent")]
public class EnemyEvent : ScriptableObject
{
    public UnityEvent<int> onEnemyDeath;
    public UnityEvent onEnemyHit;

    public void RaiseEnemyDeath(int scoreValue)
    {
        onEnemyDeath?.Invoke(scoreValue);
    }

    public void RaiseEnemyHit()
    {
        onEnemyHit?.Invoke();
    }
}