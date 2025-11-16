using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NOT BEING USED ATM


// Sourced from ChtGPT because I hate pathfinding algorithms
public class Node
{
    public Vector2Int gridPosition;  // The X and Y coordinate in grid space
    public bool walkable;            // Can the enemy/player walk on this tile?
    public Node parent;              // The node you came from when calculating a path
    public float gCost, hCost;       // Costs used in A* pathfinding
    public float fCost => gCost + hCost; // Total cost (g + h)

    public Node(Vector2Int gridPos, bool isWalkable)
    {
        gridPosition = gridPos;
        walkable = isWalkable;
    }
}
