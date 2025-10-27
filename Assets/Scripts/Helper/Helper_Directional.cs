using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helper_Directional
{
    public static Vector2 DirectionToVector(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            Direction.UpRight => Vector2.up + Vector2.right,
            Direction.UpLeft => Vector2.up + Vector2.left,
            Direction.DownRight => Vector2.down + Vector2.right,
            Direction.DownLeft => Vector2.down + Vector2.left,
            _ => Vector2.zero
        };
    }

    public static Direction VectorToDirection(Vector2 dir)
    {
        return dir switch
        {
            { x: 0, y: 1 } => Direction.Up,
            { x: 0, y: -1 } => Direction.Down,
            { x: -1, y: 0 } => Direction.Left,
            { x: 1, y: 0 } => Direction.Right,
            { x: 1, y: 1 } => Direction.UpRight,
            { x: -1, y: 1 } => Direction.UpLeft,
            { x: 1, y: -1 } => Direction.DownRight,
            { x: -1, y: -1 } => Direction.DownLeft,
            _ => Direction.Up
        };
    }

    public static Direction RandomCardinalDirection()
    {
        return UnityEngine.Random.Range(0, 4) switch
        {
            0 => Direction.Up,
            1 => Direction.Down,
            2 => Direction.Left,
            3 => Direction.Right,
            _ => Direction.Up
        };
    }

    public static Vector2 RandomCardinalVector()
    {
        return DirectionToVector(UnityEngine.Random.Range(0, 4) switch
        {
            0 => Direction.Up,
            1 => Direction.Down,
            2 => Direction.Left,
            3 => Direction.Right,
            _ => Direction.Up
        });
    }

    public static Vector2 RandomOctilinearVector()
    {
        return DirectionToVector(UnityEngine.Random.Range(0, 8) switch
        {
            0 => Direction.Up,
            1 => Direction.Down,
            2 => Direction.Left,
            3 => Direction.Right,
            4 => Direction.UpRight,
            5 => Direction.UpLeft,
            6 => Direction.DownRight,
            7 => Direction.DownLeft,
            _ => Direction.Up
        });
    }

    // Returns a random normalized direction vector, biased towards vectors that are closer to the vector to the target
    public static Vector2 BiasedDirection(Vector2 selfPosition, Vector2 targetPosition, float baseWeight = 0.5f, float biasStrength = 1.5f)
    {
        float alignment;
        Dictionary<Direction, float> directionWeights = new Dictionary<Direction, float>();

        // Get normalized vector between self and player
        Vector2 toTarget = (targetPosition - selfPosition).normalized;

        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            // Calculate the dot product of all 8 directions and the vector to the player
            alignment = Vector2.Dot(DirectionToVector(dir), toTarget);

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
        //Debug.Log("Random num is "  + randomNum);

        // Loop the dictionary, adding weights until the random float is met or exceeded
        float cumulative = 0f;

        foreach (var pair in directionWeights)
        {
            cumulative += pair.Value;

            // Return the direction associated with the weight
            if (randomNum <= cumulative)
                return DirectionToVector(pair.Key);
        }

        // Fallback
        return Vector2.right;
    }

    // Returns the 8-directional normal vector pointing to the targetPos
    public static Vector2 VectorToTargetOctilinear(Vector3 targetPos, Vector3 myGridPos)
    {
        // Convert targetPos to a Vector2
        Vector2 targetGridPos = (Vector2)targetPos;
        Vector2 normalVector = Vector2.zero;

        // Check x and y relations
        // Switch statement
        switch (myGridPos.x.CompareTo((int)targetGridPos.x))
        {
            case < 0:
                normalVector = Vector2.right;
                break;
            case 0:
                normalVector = Vector2.zero;
                break;
            case > 0:
                normalVector = Vector2.left;
                break;
        }

        switch (myGridPos.y.CompareTo((int)targetGridPos.y))
        {
            case < 0:
                normalVector += Vector2.up;
                break;
            case 0:
                normalVector += Vector2.zero;
                break;
            case > 0:
                normalVector += Vector2.down;
                break;
        }

        return normalVector;
    }

    public static Vector2 VectorInverseTargetOctilinear(Vector3 targetPos, Vector3 myGridPos) => -VectorToTargetOctilinear(targetPos, myGridPos);


    // Returns the 4-directional normal vector pointing to the targetPos
    public static Vector2 VectorToTargetCardinal(Vector3 targetPos, Vector3 myGridPos, float threshold = 0.1f)
    {
        // Convert targetPos to a Vector2
        Vector2 normalVector = Vector2.zero;
        Vector2 delta = targetPos - myGridPos;

        // Ignore tiny differences that cause flicker
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > threshold)
                normalVector = Vector2.right;
            else if (delta.x < -threshold)
                normalVector = Vector2.left;
        }
        else
        {
            if (delta.y > threshold)
                normalVector = Vector2.up;
            else if (delta.y < -threshold)
                normalVector = Vector2.down;
        }

        return normalVector;
    }

    public static Quaternion VectorToQuaternion(Vector3 dirVector) => Quaternion.Euler(0f, 0f, (Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg) - 90);
}
