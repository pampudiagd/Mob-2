using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivePerk : PerkEffect
{
    // Subclass of PerkEffect, and base class for Active Perk Effects, which are
    // effects that only become activated upon a condition being fulfilled, and 
    // deactivate once the condition is no longer fulfilled. This encompasses 
    // effects that occur once upon an action/condition, and effects that continue
    // until the condition is no longer fulfilled.

    protected PlayerEvent playerEvent;

    public override void Start()
    {
        base.Start(); // Calls Effect.cs' Start() which searches through the Resources folder to set the effectEvent
        playerEvent = Resources.Load<PlayerEvent>("Event Listeners/Player Event Listener");
    }

    // Checks if a condition is met, then calls ActivateEffect() or DeactivateEffect() depending on the result
    protected abstract void CheckCondition();

    protected abstract void ActivateEffect();

    // Not abstract since some Active Perks only receive an Event with no further condition to be fulfilled
    protected virtual void DeactivateEffect() { }
}
