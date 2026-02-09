using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour, IEffect
{
    // Base class for all effects.

    protected StatEntity myTargetScript;
    protected GameObject myTargetObject;
    protected EffectEventWithData effectEvent;

    // Stores the passed script and its GameObject
    public virtual void Apply(StatEntity targetScript)
    {
        myTargetScript = targetScript;
        myTargetObject = myTargetScript.gameObject;
    }

    public abstract void RevertChanges();

    // Loads the event handler for effects from the Resources folder
    public virtual void Start()
    {
        effectEvent = Resources.Load<EffectEventWithData>("Event Listeners/Effect Event Listener");
    }
}
