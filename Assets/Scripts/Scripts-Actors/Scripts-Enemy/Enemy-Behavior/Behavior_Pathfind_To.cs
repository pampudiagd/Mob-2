using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Behavior_Pathfind_To : Behavior_Base
{
    public float moveSpeed = 2f; // Units per second
    private Tilemap levelTilemap;

    private Queue<Vector2Int> pathQueue; // Holds the path in order
    private Vector3 targetWorldPos;
    private bool isMoving;

    private void Start()
    {
        levelTilemap = FindObjectOfType<Grid>().GetComponentInChildren<Tilemap>();
    }

    // Call this from outside with a new path
    public void SetPath(List<Vector2Int> path)
    {
        pathQueue = new Queue<Vector2Int>(path);
        if (pathQueue.Count > 0)
            MoveToNextTile();
    }

    private void Update()
    {
        if (isMoving)
        {
            // Move smoothly toward the target position
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

            // Reached target?
            if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
            {
                isMoving = false;
                MoveToNextTile();
            }
        }
    }

    private void MoveToNextTile()
    {
        if (pathQueue != null && pathQueue.Count > 0)
        {
            Vector2Int nextGridPos = pathQueue.Dequeue();
            targetWorldPos = levelTilemap.GetCellCenterWorld((Vector3Int)nextGridPos);
            isMoving = true;
        }
    }
}
