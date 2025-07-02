using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Player/PlayerEvent")]
public class PlayerEvent : ScriptableObject
{
    public UnityEvent onPlayerDamaged;
    public UnityEvent onPlayerDeath;
    public UnityEvent onPlayerWeaponContact;

    public void RaisePlayerDamaged()
    {
        onPlayerDamaged?.Invoke();
    }

    public void RaisePlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }

    public void RaisePlayerWeaponContact()
    {
        onPlayerWeaponContact?.Invoke();
    }
}
