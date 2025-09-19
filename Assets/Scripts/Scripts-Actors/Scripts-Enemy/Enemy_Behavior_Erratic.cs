using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy_Behavior_Erratic : Enemy_Base
{

    private float baseWeight = 0.5f;
    private float biasStrength = 1.5f;
    private float effectiveMoveSpeed;
    private float speedRange = 0.5f;
    private float lowerSpeedMod;
    private float upperSpeedMod;
    private int counter = 0;
    Vector2 currentDirection;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lowerSpeedMod = moveSpeed - speedRange;
        upperSpeedMod = moveSpeed + speedRange;
        Behavior_1();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        Behavior_0();
    }

    protected override void Behavior_0()
    {
        if (myState != EnemyState.Default || myBehaviorState != BehaviorState.Idle)
            return;

        if (counter > 30)
        {
            // Pick a number between 0 and 7 to set direction, biasing toward the player => call BiasedDirection
            currentDirection = Helper_Directional.BiasedDirection(
                transform.position,
                base.target.transform.position,
                baseWeight,
                biasStrength
                );

            // Set effectiveSpeed to a # between speed+- speedMod
            effectiveMoveSpeed = ChangeSpeed();

            counter = 0;
        }

        // Move in direction using effectiveSpeed
        base.rb.MovePosition(rb.position + currentDirection * effectiveMoveSpeed * Time.fixedDeltaTime);

        counter++;
    }

    // Returns the sum of moveSpeed and a random float between +- speedMod
    private float ChangeSpeed()
    {
        return base.myBaseStats.baseSpeed + UnityEngine.Random.Range(lowerSpeedMod, upperSpeedMod);
    }


}
