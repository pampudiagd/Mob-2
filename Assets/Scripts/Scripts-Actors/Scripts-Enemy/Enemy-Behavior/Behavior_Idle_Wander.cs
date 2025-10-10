using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Behavior_Idle_Wander : MonoBehaviour
{
    private Rigidbody2D rb;
    private IGridNav navigator;
    public Vector3Int forwardTile;
    //public GridScanner gridScanner;
    //public TileBase wallTile;

    public Coroutine moveRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navigator = LevelManager.Instance.gridNav;
    }

    protected Vector3Int SetForwardTile(Vector3Int myGridPos, Vector2 movementVector) => Vector3Int.FloorToInt(myGridPos + (Vector3)movementVector);

    public IEnumerator MoveToTile(Vector3 target, System.Func<bool> interruptCondition = null, float moveSpeed = 1f, float minPause = 0.3f, float maxPause = 2f)
    {
        yield return new WaitForSeconds(0.5f);
        int moveAttempts = 0;
        while ((rb.position - (Vector2)target).sqrMagnitude > 0.001f)
        {
            if ((interruptCondition != null && interruptCondition()) || moveAttempts >= 50)
            {
                print("Failed move attempt");
                moveRoutine = null;
                yield break; // stop movement early
            }

            Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            moveAttempts++;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target); // Snap to exact center

        yield return new WaitForSeconds(UnityEngine.Random.Range(minPause, maxPause));

        moveRoutine = null;
    }

    public Vector3? GetNextTarget(Transform enemyTransform, Vector2 movementVector)
    {
        forwardTile = SetForwardTile(Vector3Int.FloorToInt(enemyTransform.position), movementVector);
        // If wall is in the way, cancel
        if (!CheckTileOpen(movementVector, forwardTile))
        {
            print("Tile not open!!!!!!!!!!");
            return null;
        }
        // Otherwise pick the next tile in the chosen direction
        return enemyTransform.position + (Vector3)movementVector;
    }

    public Vector3? GetNextTarget(Transform enemyTransform, Vector2 movementVector, int tileDistance)
    {
        forwardTile = SetForwardTile(Vector3Int.FloorToInt(enemyTransform.position), movementVector);

        for (int i = 1; i < tileDistance; i++)
        {
            if (!CheckTileOpen(movementVector, forwardTile))
            {
                print("Tile not open");
                return null;
            }
            forwardTile = SetForwardTile(forwardTile, movementVector);
        }

        return enemyTransform.position + (Vector3)movementVector * tileDistance;
    }


    public bool CheckTileOpen(Vector2 movementVector, Vector3Int forwardTile)
    {
        if (LevelManager.Instance.GridScanner.LevelTilemap.GetTile(forwardTile) == navigator.GetWallTile())
            return false;
        else 
            return true;
    }

}
