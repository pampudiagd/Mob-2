using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstanceStatus : StatusEffect
{
    // Subclass of StatusEffect, and base class for Instance Effects, which are statuses that
    // only apply their effect when they are initially created, and have a timer that counts
    // down to remove the effect. Attempting to inflict these while already active on a target
    // should restart the timer.

    public float maxDuration = 2f;
    protected float currentDuration;

    public override void Start()
    {
        base.Start(); // Calls Effect.cs' Start() which searches through the Resources folder to set the effectEvent
        currentDuration = maxDuration;
    }

    void Update()
    {
        EffectTimer(Time.deltaTime);
    }

    // Simple timer that should be executed in relation to frame updates
    public void EffectTimer(float deltaTime)
    {
        currentDuration -= deltaTime;
        if (currentDuration <= 0f)
        {
            RevertChanges();
        }
    }

}
