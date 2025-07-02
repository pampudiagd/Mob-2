using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContinuousStatus : StatusEffect
{
    // Subclass of StatusEffect, and base class for Continuous Effects, which are statuses that
    // apply their effect every time they tick at the set interval, until no more ticks remain. 
    // Attempting to inflict these while already active on a target should add ticks to the current effect.

    private float timer;

    [SerializeField]
    protected int tickCount = 1; // Number of ticks
    protected float tickDelay = 1; // Seconds between each tick
    

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start(); // Calls Effect.cs' Start() which searches through the Resources folder to set the effectEvent
        timer = tickDelay;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (TickTimer() <= 0)
        {
            EffectTick();
            tickCount--;
            timer = tickDelay;
        }
        if (tickCount <= 0)
            RevertChanges();
    }

    protected abstract void EffectTick();

    private float TickTimer() => timer -= Time.deltaTime;
    
}
