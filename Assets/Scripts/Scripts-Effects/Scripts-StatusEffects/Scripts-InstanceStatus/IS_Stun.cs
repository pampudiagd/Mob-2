using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IS_Stun : InstanceStatus
{
    //private GameObject myTarget;
    private Rigidbody2D rb;

    public override void Apply(StatEntity targetScript)
    {
        base.Apply(targetScript);

        rb = myTarget.GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public override void RevertChanges()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Runs the overriden method, which broadcasts DeleteEffect
        base.RevertChanges();
    }
}
