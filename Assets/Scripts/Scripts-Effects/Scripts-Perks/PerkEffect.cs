using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerkEffect : Effect
{
    // Base class for Perk Effects, which are tied to perk items and encompass Passive Perks and Active Perks.

    protected Player playerScript;

    public override void Apply(StatEntity targetScript)
    {
        base.Apply(targetScript);
        playerScript = modScript as Player; // This is done here instead of parent class since perks will only be applied to the player
    }

    // Broadcast for EffectManager to remove and destroy me, will be used by Instance and Continuous Effects
    public override void RevertChanges()
    {
        base.effectEvent.DeletePerkEffect(this);
    }
}
