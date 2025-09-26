using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IS_Freeze : InstanceStatus
{
    public override void Apply(StatEntity targetScript)
    {
        Debug.Log(targetScript);
    }

    public override void RevertChanges()
    {
        // Runs the overriden method, which broadcasts DeleteEffect
        base.RevertChanges();
    }
}
