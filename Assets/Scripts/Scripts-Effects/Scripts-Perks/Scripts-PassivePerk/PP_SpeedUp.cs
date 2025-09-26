using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_SpeedUp : PassivePerk
{
    public void Awake()
    {
        statMod = new StatModifier(1.25f, ModifierType.Multiplicative, StatType.Speed);
    }

    public override void Apply(StatEntity targetScript)
    {
        // Casts the passed script into a MonoBehavior and also stores its GameObject
        base.Apply(targetScript);

        //playerScript.moveSpeed = playerScript.myBaseStats.baseSpeed * 1.25f;

        playerScript.AddModifier(statMod);
    }

    public override void RevertChanges()
    {
        playerScript.moveSpeed = playerScript.myBaseStats.baseSpeed;

        // Runs the overriden method, which broadcasts DeleteEffect
        base.RevertChanges();
    }
}
