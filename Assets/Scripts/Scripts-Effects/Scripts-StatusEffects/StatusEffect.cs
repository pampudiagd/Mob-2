using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : Effect
{
    // Base class for Status Effects, which are temporary and encompass Instance Statuses, and Continuous Statuses.

    protected StatEntity statEntityScript;

    public override void Apply(IDamageable targetScript)
    {
        base.Apply(targetScript);
        statEntityScript = modScript as StatEntity; // Casts into StatEntity so both player and enemy stats can be accessed
    }

    // Broadcast for EffectManager to remove and destroy me, will be used by Instance and Continuous Effects
    public override void RevertChanges()
    {
        base.effectEvent.DeleteStatusEffect(this);
    }

}
