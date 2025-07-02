using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_DoubleDmg : PassivePerk
{
    public override void Apply(IDamageable targetScript)
    {
        // Casts the passed script into a MonoBehavior and also stores its GameObject
        base.Apply(targetScript);

        playerScript.globalDamageMod = 2;
    }

    public override void RevertChanges()
    {
        playerScript.globalDamageMod = 1;

        // Runs the overriden method, which broadcasts DeleteEffect
        base.RevertChanges();
    }
}
