using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour, IEffect
{
    // Base class for all effects.

    protected StatEntity modScript;
    protected GameObject myTarget;
    protected EffectEventWithData effectEvent;

    // Stores the passed script and its GameObject
    public virtual void Apply(StatEntity targetScript)
    {
        modScript = targetScript;
        myTarget = modScript.gameObject;
    }

    public abstract void RevertChanges();

    // Loads the event handler for effects from the Resources folder
    public virtual void Start()
    {
        effectEvent = Resources.Load<EffectEventWithData>("Event Listeners/Effect Event Listener");
    }
}
