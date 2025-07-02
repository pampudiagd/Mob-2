using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_AmmoGain : ActivePerk
{
    // Awake is used because playerEvent needs to be initialized before OnEnable runs
    public void Awake()
    {
        base.Start();
    }

    private void OnEnable()
    {
        playerEvent.onPlayerDamaged.AddListener(CheckCondition);
    }

    private void OnDisable()
    {
        playerEvent.onPlayerDamaged.RemoveListener(CheckCondition);
    }

    protected override void CheckCondition()
    {
        // No check needed
        ActivateEffect();
    }

    protected override void ActivateEffect()
    {
        playerScript.GainAmmoCharge();
    }
}
