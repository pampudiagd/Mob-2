using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    // Interface for all effects (statuses and perks)
    // All effects must implement a method to apply an effect, and a method to revert the effect


    // Execute initial effects
    // Instance and Passive will also inflict effects here
    // Continuous and Active will just set up here and inflict effects in their tick/condition functions 
    void Apply(StatEntity targetScript);

    // Remove effects, reverting any changes in parent's stats to their unmodified states
    // Broadcast for EffectManager to remove and destroy me
    void RevertChanges();
}
