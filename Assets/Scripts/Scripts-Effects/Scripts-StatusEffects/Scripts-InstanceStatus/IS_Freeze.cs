using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

// Apply to enemies only!
public class IS_Freeze : InstanceStatus
{

    private Enemy_Base enemyScript;

    public override void Apply(StatEntity targetScript)
    {
        Debug.Log(targetScript);
        // Check if target isn't already frozen
        // Set the targetScript's state to frozen
        enemyScript = targetScript as Enemy_Base;
        enemyScript.SetState(EnemyState.Frozen);
        // Start a timer
    }

    public override void RevertChanges()
    {
        // Set the targetScript's state to default
        enemyScript.SetState(EnemyState.Default);

        // Runs the overriden method, which broadcasts DeleteEffect
        base.RevertChanges();
    }
}
