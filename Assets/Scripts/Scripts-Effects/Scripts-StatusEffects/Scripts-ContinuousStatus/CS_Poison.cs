using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Poison : ContinuousStatus
{
    public float myDamage = 1;

    protected override void EffectTick()
    {
        print("Ticking");
        statEntityScript.TakePassiveDamage(myDamage, DamageType.Poison);
    }

    public override void RevertChanges()
    {
        base.RevertChanges();
    }
}
