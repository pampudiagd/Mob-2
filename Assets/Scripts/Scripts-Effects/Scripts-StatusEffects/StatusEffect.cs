using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : Effect
{
    // Base class for Status Effects, which are temporary and encompass Instance Statuses, and Continuous Statuses.

    public override void Apply(StatEntity targetScript)
    {
        base.Apply(targetScript);
    }

    // Broadcast for EffectManager to remove and destroy me, will be used by Instance and Continuous Effects
    public override void RevertChanges()
    {
        base.effectEvent.DeleteStatusEffect(this);
    }

}
