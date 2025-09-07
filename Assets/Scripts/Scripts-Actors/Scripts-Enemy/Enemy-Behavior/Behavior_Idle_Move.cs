using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Idle_Move
{
    private GridScanner gridScanner;
    private IGridNav navigator;

    // Checks if the adjacent tile can be entered.
    public Behavior_Idle_Move(GridScanner gridScanner, IGridNav navigator)
    {
        this.gridScanner = gridScanner;
        this.navigator = navigator;
    }

    // Decides the next target tile for the enemy to move to.
    // Returns null if movement should not happen.
    public Vector3? GetNextTarget(Transform enemyTransform, Vector2 movementVector, Vector3Int forwardTile)
    {
        // If wall is in the way, cancel
        if (gridScanner.levelTilemap.GetTile(forwardTile) == navigator.GetWallTile())
            return null;

        // Otherwise pick the next tile in the chosen direction
        return enemyTransform.position + (Vector3)movementVector;
    }

}
