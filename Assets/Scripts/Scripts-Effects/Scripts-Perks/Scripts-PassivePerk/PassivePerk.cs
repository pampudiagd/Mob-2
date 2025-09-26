using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassivePerk : PerkEffect
{
    // Subclass of PerkEffect, and base class for Passive Perk Effects, which are
    // effects that apply their properties on creation, and do nothing else until
    // the Passive Perk item is discarded by the player.

    protected StatModifier statMod;

    public override void RevertChanges()
    {
        if (statMod != null)
            playerScript.RemoveModifier(statMod);
        
        base.RevertChanges();
    }

    // This class exists for consistancy in how effects are structured. 
    // I think it's easier to understand how effects work if Statuses
    // and Perks are mirrored in hierarchy.
}
