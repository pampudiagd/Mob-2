using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Behavior_Idle_Wander : MonoBehaviour
{
    private Rigidbody2D rb;
    private IGridNav navigator;
    //public GridScanner gridScanner;
    //public TileBase wallTile;

    public Coroutine moveRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navigator = LevelManager.Instance.gridNav;
    }

    public IEnumerator MoveToTile(Vector3 target, System.Func<bool> interruptCondition = null, float moveSpeed = 1f, float minPause = 0.3f, float maxPause = 2f)
    {
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

    public Vector3? GetNextTarget(Transform enemyTransform, Vector2 movementVector, Vector3Int forwardTile)
    {
        // If wall is in the way, cancel
        if (LevelManager.Instance.GridScanner.LevelTilemap.GetTile(forwardTile) == navigator.GetWallTile())
            return null;

        // Otherwise pick the next tile in the chosen direction
        return enemyTransform.position + (Vector3)movementVector;
    }

}
