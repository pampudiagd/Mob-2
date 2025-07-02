using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StatusEffectEvent : UnityEvent<StatusEffect> { }

[System.Serializable]
public class PerkEffectEvent : UnityEvent<PerkEffect> { }

[CreateAssetMenu(menuName = "Events/EffectEvent")]
public class EffectEventWithData : ScriptableObject
{
    public StatusEffectEvent onStatusReverted;
    public PerkEffectEvent onPerkReverted;

    public void DeleteStatusEffect(StatusEffect status)
    {
        Debug.Log("Reached DeleteStatus");
        onStatusReverted?.Invoke(status);
    }

    public void DeletePerkEffect(PerkEffect perk)
    {
        Debug.Log("Reach DeletePerk");
        onPerkReverted?.Invoke(perk);
    }
}
