using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Test_Dummy : Enemy_Base
{
    private IGridNav navigator;
    private AStarPathfinder pathfinder;
    private Vector2Int targetGridPos;
    private Behavior_Pathfind_To behaviorFollow;
    private Behavior_Idle_Move behavior_Idle;
    private Vector2 movementVector;
    private Vector3Int forwardTile;
    private Coroutine moveRoutine;
    private int moveAttempts = 0;

    public bool isActing = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        pathfinder = new AStarPathfinder(gridScanner);
        behaviorFollow = GetComponent<Behavior_Pathfind_To>();
        navigator = FindObjectOfType<GameManager>().GetComponent<TilemapNav>();

        behavior_Idle = new Behavior_Idle_Move(gridScanner, navigator);
        SetMovementDirection();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //base.Update();
        //if (Input.GetKeyDown(KeyCode.P))
    }

    private void FixedUpdate()
    {
        if (myBehaviorState == BehaviorState.Idle)
            Behavior_0();
        else if (myBehaviorState == BehaviorState.Targeting)
            Behavior_1();
    }

    // Moves toward the target tile, allowing for the bool interrupted and walls to stop it
    private IEnumerator MoveToTile(Vector3 target)
    {
        interrupted = false;

        while ((base.rb.position - (Vector2)target).sqrMagnitude > 0.001f)
        {
            if (interrupted || gridScanner.levelTilemap.GetTile(forwardTile) == navigator.GetWallTile() || moveAttempts >= 50 || myBehaviorState != BehaviorState.Idle)
            {
                moveRoutine = null;
                moveAttempts = 0;
                yield break; // stop movement early
            }

            Vector2 newPos = Vector2.MoveTowards(base.rb.position, target, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            moveAttempts++;
            yield return new WaitForFixedUpdate();
        }

        base.rb.MovePosition(target); // Snap to exact center

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 2f));

        moveRoutine = null;
    }

    protected override void Behavior_0()
    {
        if (myState != EnemyState.Default || moveRoutine != null)
            return;

        SetMovementDirection(); // Sets movementVector to a random direction and faces this object toward that Vector.
        SetForwardTile(); // Assigns the tile coords of the space in front of this object, using movementVector for the direction.

        Vector3? target = behavior_Idle.GetNextTarget(transform, movementVector, forwardTile);

        if (target == null) // Kills movement if there's a wall
        {
            moveRoutine = null;
            return;
        }

        moveRoutine = StartCoroutine(MoveToTile(target.Value)); // Starts the movement step while also ensuring this function won't run again 
    }

    //private IEnumerator TestBehavior2()
    //{
    //    if (myState != EnemyState.Default)
    //        yield break;

    //    SetMovementDirection();

    //    isActing = true;
    //    float checkDistance = moveSpeed * Time.fixedDeltaTime;

    //    for (int i = 0; i < 70; i++)
    //    {
    //        SetForwardTile();

    //        Debug.Log("My Position is " + gridScanner.levelTilemap.WorldToCell(transform.position));
    //        Debug.Log($"Is {movementVector + (Vector2Int)gridScanner.levelTilemap.WorldToCell(transform.position)} a wall? {gridScanner.levelTilemap.GetTile(forwardTile)}");
    //        if (gridScanner.levelTilemap.GetTile(forwardTile) == navigator.GetWallTile())
    //        {
    //            isFacingWall = true;
    //            Debug.Log("THIS IS A WALL");
    //        }
    //        else
    //        {
    //            isFacingWall = false;
    //            Debug.Log("NOT A WALL HERE");
    //        }

    //        if (isFacingWall)
    //            break;
    //        else
    //            rb.MovePosition(rb.position + movementVector * moveSpeed * Time.fixedDeltaTime);

    //        yield return new WaitForSeconds(0.01f);
    //    }

    //    yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));

    //    isActing = false;
    //}

    //protected override void Behavior_1()
    //{
    //    float checkDistance = moveSpeed * Time.fixedDeltaTime;
    //    isTouchingWall = Physics2D.Raycast(rb.position, movementVector, checkDistance, environmentLayer);

    //    if (idleTimer <= 0)
    //    {
    //        isIdle = !isIdle;
    //        idleTimer = UnityEngine.Random.Range(1, idleDelay);
    //    }
    //    else
    //        idleTimer = TimerCount(idleTimer);


    //    if (myState == EnemyState.Default && !isIdle)
    //        rb.MovePosition(rb.position + movementVector * moveSpeed * Time.fixedDeltaTime);


    //    if (myState == EnemyState.Default && isTouchingWall && offWall)
    //    {
    //        //offWall = false;
    //        Debug.Log("FAILED, RETRYING");
    //        rotationTimer = 0;
    //    }


    //    if (rotationTimer <= 0)
    //    {
    //        SetMovementDirection();
    //        //isTouchingWall = false;
    //        rotationTimer = UnityEngine.Random.Range(1, rotationDelay);
    //        if (UnityEngine.Random.Range(0, 2) == 1)
    //            isIdle = !isIdle;
    //    }
    //    else
    //        rotationTimer = TimerCount(rotationTimer);
    //}

    protected override void Behavior_1()
    {
        if (myState != EnemyState.Default || moveRoutine != null)
            return;

        while ((base.rb.position - (Vector2)target.transform.position).sqrMagnitude > 0.001f)
        {
            if (interrupted || gridScanner.levelTilemap.GetTile(forwardTile) == navigator.GetWallTile() || moveAttempts >= 50)
            {
                moveRoutine = null;
                moveAttempts = 0;
                break; // stop movement early
            }

            Vector2 newPos = Vector2.MoveTowards(base.rb.position, (Vector2)target.transform.position, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            moveAttempts++;
        }


    }

    protected void Behavior_2()
    {
        if (myState != EnemyState.Default || !IsCenteredOnTile() || moveRoutine != null)
            return;

        Vector2Int start = (Vector2Int)myGridPos;
        targetGridPos = (Vector2Int)gridScanner.levelTilemap.WorldToCell(target.transform.position); // Replace with player position

        List<Vector2Int> path = pathfinder.FindPath(start, targetGridPos);
        if (path != null)
            behaviorFollow.SetPath(path);
    }

    public override void OnPlayerDetected()
    {
        base.OnPlayerDetected();

    }

    private bool IsCenteredOnTile()
    {
        Vector3 worldCenter = gridScanner.levelTilemap.GetCellCenterWorld(myGridPos);
        return Vector3.Distance(transform.position, worldCenter) < 0.01f;
    }

    // Sets direction to a random Direction, sets movementVector to the converted Vector2, and faces toward movementVector.
    private void SetMovementDirection()
    {
        direction = Helper_Directional.RandomCardinalDirection(); // Sets the direction var
        movementVector = Helper_Directional.DirectionToVector(direction); // Static class function
        FaceDirection(direction); // Rotates toward the direction var
    }

    private Vector3Int SetForwardTile() => forwardTile = Vector3Int.RoundToInt(myGridPos + (Vector3)movementVector);
}
