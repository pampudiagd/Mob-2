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
    private float lowerSpeedMod = -0.5f;
    private float upperSpeedMod = 1.5f;
    private int counter = 0;
    Vector2 currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        base.rb = GetComponent<Rigidbody2D>();
        Behavior_1();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Behavior_1();
    }

    protected override void Behavior_1()
    {
        if (counter > 30)
        {
            // Pick a number between 0 and 7 to set direction, biasing toward the player => call BiasedDirection
            currentDirection = BiasedDirection();

            // Set effectiveSpeed to a # between speed+- speedMod
            effectiveMoveSpeed = ChangeSpeed();

            counter = 0;
        }

        // Move in direction using effectiveSpeed
        base.rb.MovePosition(rb.position + currentDirection * effectiveMoveSpeed * Time.fixedDeltaTime);

        counter++;
    }

    // Picks a normalized direction vector, biased towards vectors that are closer to the vector to the target
    private Vector2 BiasedDirection()
    {
        float alignment;
        Dictionary<Direction, float> directionWeights = new Dictionary<Direction, float> { };

        // Get normalized vector between self and player
        Vector2 toTarget = (base.target.transform.position - transform.position).normalized;

        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            // Calculate the dot product of all 8 directions and the vector to the player
            alignment = Vector2.Dot(base.GetDirectionVector(dir), toTarget);

            // Convert each dot product to a weight (add a base to each weight) and bias it, storing it in a dictionary of <Direction enum, float weight>
            float weight = baseWeight + Mathf.Max(0, alignment);
            weight = Mathf.Pow(weight, biasStrength);
            directionWeights.Add(dir, weight);
            //print(dir + " " + weight);
        }

        // Sum the weights
        float totalWeight = directionWeights.Values.Sum();

        // Pick a random float between 0 and the total weight
        float randomNum = UnityEngine.Random.Range(0, totalWeight);
        //print("Random num is "  + randomNum);

        // Loop the dictionary, adding weights until the random float is met or exceeded
        float cumulative = 0f;

        foreach(var pair in directionWeights)
        {
            cumulative += pair.Value;

            // Return the direction associated with the weight
            if (randomNum <= cumulative)
                return base.GetDirectionVector(pair.Key);
        }

        // Fallback
        return Vector2.right;
    }

    // Returns the sum of moveSpeed and a random float between +- speedMod
    private float ChangeSpeed()
    {
        return base.myBaseStats.baseSpeed + UnityEngine.Random.Range(lowerSpeedMod, upperSpeedMod);
    }


}
