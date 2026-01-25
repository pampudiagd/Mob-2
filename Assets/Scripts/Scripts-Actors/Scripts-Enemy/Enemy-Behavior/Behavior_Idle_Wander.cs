using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Behavior_Idle_Wander : Behavior_Base
{
    private Rigidbody2D rb;
    //private IGridNav navigator;
    public Vector3 forwardTile;
    //public GridScanner gridScanner;
    //public TileBase wallTile;

    public Coroutine moveRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //navigator = LevelManager.Instance.gridNav;
    }

    protected Vector3 SetForwardTile(Vector3 myGridPos, Vector2 movementVector) => myGridPos + (Vector3)movementVector;

    // Moves an actor to a target tile, allowing for movement to be interrupted by a bool function
    public IEnumerator MoveToTile(Vector3 target, System.Func<bool> interruptCondition = null, float moveSpeed = 1f, float minPause = 0.3f, float maxPause = 2f)
    {
        yield return new WaitForSeconds(0.5f);
        int moveAttempts = 0;
        while ((transform.position - target).sqrMagnitude > 0.001f)
        {
            if ((interruptCondition != null && interruptCondition()) || moveAttempts >= 50)
            {
                print("Failed move attempt");
                moveRoutine = null;
                yield break; // stop movement early
            }
            Vector2 newPos = Vector2.MoveTowards(rb.position, LevelManager.Instance.LevelTilemap.GetCellCenterWorld(LevelManager.Instance.LevelTilemap.WorldToCell(target)), moveSpeed * Time.fixedDeltaTime);

            rb.MovePosition(newPos);

            moveAttempts++;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(LevelManager.Instance.LevelTilemap.GetCellCenterWorld(LevelManager.Instance.LevelTilemap.WorldToCell(target))); // Snap to exact center

        yield return new WaitForSeconds(UnityEngine.Random.Range(minPause, maxPause));

        moveRoutine = null;
    }

    // Returns the LOCAL grid coord of the tile in a direction(movementVector) away from the position(enemyTransform) if there isn't a wall in the way
    public Vector3? GetNextTarget(Vector3 gridPos, Vector2 movementVector)
    {
        forwardTile = SetForwardTile(gridPos, movementVector);
        // If wall is in the way, cancel
        print(forwardTile);
        if (!CheckTileOpen(forwardTile))
        {
            print("Tile not open!!!!!!!!!!");
            return null;
        }

        //if (forwardTile.x == Mathf.FloorToInt(enemyTransform.position.x) && forwardTile.y == Mathf.FloorToInt(enemyTransform.position.y))
        //{
        //    print("Attempting diagonal move");
        //    Vector3Int sideA = Vector3Int.FloorToInt(enemyTransform.position + new Vector3(movementVector.x, 0));
        //    Vector3Int sideB = Vector3Int.FloorToInt(enemyTransform.position + new Vector3(0, movementVector.y));
        //    if (CheckTileHole(sideA) || CheckTileHole(sideB))
        //    {
        //        print("Diagonal move blocked!!!!");
        //        return null;
        //    }
        //}
        // Otherwise pick the next tile in the chosen direction
        return forwardTile;
    }

    // Returns the grid coord of the tile a specified distance(tileDistance) in a direction(movementVector) away from the position(enemyTransform) if there isn't a wall in the way
    public Vector3? GetNextTarget(Vector3 gridPos, Vector2 movementVector, int tileDistance)
    {
        forwardTile = SetForwardTile(gridPos, movementVector);

        for (int i = 0; i < tileDistance; i++)
        {
            if (!CheckTileOpen(forwardTile))
            {
                print("Tile not open");
                return null;
            }
            print("USING ME");
            //if (forwardTile.x == currentTile.x && forwardTile.y == currentTile.y)
            //{
            //    print("Attempting diagonal move");
            //    Vector3Int sideA = Vector3Int.FloorToInt(currentTile + new Vector3(movementVector.x, 0));
            //    Vector3Int sideB = Vector3Int.FloorToInt(currentTile + new Vector3(0, movementVector.y));
            //    if (CheckTileHole(sideA) || CheckTileHole(sideB))
            //    {
            //        print("Diagonal move blocked!!!!");
            //        return null;
            //    }
            //}

            forwardTile = SetForwardTile(forwardTile, movementVector);
        }

        return gridPos + (Vector3)movementVector * tileDistance;
    }
}
