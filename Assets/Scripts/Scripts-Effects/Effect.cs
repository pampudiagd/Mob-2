using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour, IEffect
{
    // Base class for all effects.

    protected MonoBehaviour modScript;
    protected GameObject myTarget;
    protected EffectEventWithData effectEvent;

    // Casts the passed script into a MonoBehavior and also stores its GameObject
    public virtual void Apply(IDamageable targetScript)
    {
        modScript = targetScript as MonoBehaviour;
        myTarget = modScript.gameObject;
    }

    public abstract void RevertChanges();

    // Loads the event handler for effects from the Resources folder
    public virtual void Start()
    {
        effectEvent = Resources.Load<EffectEventWithData>("Event Listeners/Effect Event Listener");
    }
}
