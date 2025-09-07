using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sourced from ChtGPT because I hate pathfinding algorithms

// Class responsible for finding a path using A* algorithm
public class AStarPathfinder

{
    // Reference to the grid of walkable and unwalkable nodes
    private GridScanner gridScanner;

    // Constructor to initialize the pathfinder with a GridScanner
    public AStarPathfinder(GridScanner scanner)
    {
        gridScanner = scanner;
    }

    // Finds a path between startPos and targetPos using A* and returns the path as a list of grid positions
    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        Node startNode = gridScanner.GetNode(startPos);
        Node targetNode = gridScanner.GetNode(targetPos);

        if (startNode == null || targetNode == null || !targetNode.walkable)
            return null;

        // The open set stores nodes we need to evaluate — starts with just the startNode
        List<Node> openSet = new() { startNode };

        // The closed set stores nodes we've already evaluated
        HashSet<Node> closedSet = new();

        // Initialize the starting node's costs
        startNode.gCost = 0; // Cost from start to this node
        startNode.hCost = GetHeuristic(startNode.gridPosition, targetPos); // Estimated cost to target
        startNode.parent = null; // No parent yet

        // While we still have nodes to evaluate
        while (openSet.Count > 0)
        {
            // Get the node with the lowest total cost (fCost)
            Node currentNode = GetLowestFCost(openSet);

            // If we've reached the target node, retrace and return the path
            if (currentNode.gridPosition == targetPos)
                return RetracePath(startNode, currentNode);

            // Remove this node from openSet and add to closedSet
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Loop through all valid neighboring nodes
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                // Skip if the neighbor is not walkable or already processed
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                // Calculate new tentative gCost (1 per move in cardinal directions)
                float tentativeGCost = currentNode.gCost + 1;

                // If this path to neighbor is better OR it's not yet in openSet
                if (tentativeGCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = tentativeGCost; // Update gCost
                    neighbor.hCost = GetHeuristic(neighbor.gridPosition, targetPos); // Update hCost
                    neighbor.parent = currentNode; // Remember how we got here

                    // Add neighbor to openSet if not already there
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // If we get here, no path was found
        return null;
    }

    // Estimates the distance between two points using Manhattan distance
    private float GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Returns the node from the list with the lowest fCost (with hCost as tiebreaker)
    private Node GetLowestFCost(List<Node> nodes)
    {
        Node lowest = nodes[0];
        foreach (Node node in nodes)
        {
            // Lower fCost preferred; if tied, lower hCost is better (closer to goal)
            if (node.fCost < lowest.fCost ||
                (node.fCost == lowest.fCost && node.hCost < lowest.hCost))
                lowest = node;
        }
        return lowest;
    }

    // Returns all valid, walkable neighboring nodes in 4 directions
    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new();

        // Cardinal directions: up, down, left, right
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // For each direction, check if there's a valid node and add it
        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = node.gridPosition + dir;
            Node neighbor = gridScanner.GetNode(checkPos);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    // Builds the final path by retracing parents from endNode back to startNode
    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new();
        Node currentNode = endNode;

        // Follow the parent chain back to the start node
        while (currentNode != startNode)
        {
            path.Add(currentNode.gridPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse(); // Start to finish order
        return path;
    }
}