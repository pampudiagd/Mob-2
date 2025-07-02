using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_SpeedUp : PassivePerk
{
    public override void Apply(IDamageable targetScript)
    {
        // Casts the passed script into a MonoBehavior and also stores its GameObject
        base.Apply(targetScript);

        playerScript.moveSpeed = playerScript.myBaseStats.baseSpeed * 1.25f;
    }

    public override void RevertChanges()
    {
        playerScript.moveSpeed = playerScript.myBaseStats.baseSpeed;

        // Runs the overriden method, which broadcasts DeleteEffect
        base.RevertChanges();
    }
}
