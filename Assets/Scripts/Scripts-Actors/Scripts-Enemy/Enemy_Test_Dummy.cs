using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Test_Dummy : Enemy_Base
{
    private IGridNav navigator;
    //private AStarPathfinder pathfinder;
    //private Vector2Int targetGridPos;
    //private Behavior_Pathfind_To behaviorFollow;
    //private Behavior_Idle_Move behavior_Idle;
    private Behavior_Idle_Wander mover;
    private Behavior_Pursuit_Simple pursuer;
    private Vector2 movementVector;
    private Vector3Int forwardTile;
    private Vector3 targetTilePos => LevelManager.Instance.GridScanner.LevelTilemap.WorldToCell(target.transform.position);

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //pathfinder = new AStarPathfinder(gridScanner);
        //behaviorFollow = GetComponent<Behavior_Pathfind_To>();
        navigator = FindObjectOfType<LevelManager>().GetComponent<TilemapNav>();
        mover = GetComponent<Behavior_Idle_Wander>();
        pursuer = GetComponent<Behavior_Pursuit_Simple>();

        //behavior_Idle = new Behavior_Idle_Move(navigator);
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

    protected override void Behavior_0()
    {
        if (myState != EnemyState.Default || mover.moveRoutine != null || myBehaviorState != BehaviorState.Idle)
            return;

        SetMovementDirection(); // Sets movementVector to a random direction and faces this object toward that Vector.
        SetForwardTile(); // Assigns the tile coords of the space in front of this object, using movementVector for the direction.

        Vector3? target = mover.GetNextTarget(transform, movementVector, forwardTile);

        if (target == null) // Kills movement if there's a wall
        {
            mover.moveRoutine = null;
            return;
        }

        mover.moveRoutine = StartCoroutine(mover.MoveToTile(target.Value, () => LevelManager.Instance.GridScanner.LevelTilemap.GetTile(forwardTile) == navigator.GetWallTile() || myBehaviorState != BehaviorState.Idle || interrupted, moveSpeed)); // Starts the movement step while also ensuring this function won't run again 
    }

    protected override void Behavior_1()
    {
        if (myState != EnemyState.Default || pursuer.moveRoutine != null || myBehaviorState != BehaviorState.Targeting)
            return;

        // Rounds the target's position to the Vector3Int used by the tilemap, then sums own tilemap position with the normalized vector between the two positions.
        // Then converts that to a Vector3Int, and finally gets the world coords of the center of that tile.
        Vector3 targetTile = LevelManager.Instance.GridScanner.LevelTilemap.GetCellCenterWorld(Vector3Int.RoundToInt(myGridPos + (Vector3)Helper_Directional.VectorToTargetOctilinear(targetTilePos, myGridPos)));
        
        direction = Helper_Directional.VectorToDirection(Helper_Directional.VectorToTargetCardinal(targetTilePos, myGridPos, 1));
        FaceDirection(direction); // Rotates toward the direction var

        pursuer.moveRoutine = StartCoroutine(pursuer.MoveToTileTargeting(targetTile, () => myBehaviorState != BehaviorState.Targeting || interrupted, moveSpeed));
    }

    protected override IEnumerator Behavior_Attack()
    {
        while (isTargetInAtkRng)
        {
            direction = Helper_Directional.VectorToDirection(Helper_Directional.VectorToTargetCardinal(targetTilePos, myGridPos));
            FaceDirection(direction); // Rotates toward the direction var

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.5f);
                //Debug.Log("Simulating attack " + i);
            }
        }
        isAttacking = false;

        if (isTargetSeen)
            myBehaviorState = BehaviorState.Targeting;
        else
            myBehaviorState = BehaviorState.Idle;
    }

    // AStar pathfinding
    //protected void Behavior_2()
    //{
    //    if (myState != EnemyState.Default || !IsCenteredOnTile() || moveRoutine != null)
    //        return;

    //    Vector2Int start = (Vector2Int)myGridPos;
    //    targetGridPos = (Vector2Int)gridScanner.levelTilemap.WorldToCell(target.transform.position); // Replace with player position

    //    List<Vector2Int> path = pathfinder.FindPath(start, targetGridPos);
    //    if (path != null)
    //        behaviorFollow.SetPath(path);
    //}

    public override void OnAttackTriggered()
    {
        base.OnAttackTriggered();
        StartCoroutine(Behavior_Attack());
    }

    public override void OnPlayerDetected()
    {
        base.OnPlayerDetected();
    }

    //private bool IsCenteredOnTile()
    //{
    //    Vector3 worldCenter = LevelManager.Instance.GridScanner.LevelTilemap.GetCellCenterWorld(myGridPos);
    //    return Vector3.Distance(transform.position, worldCenter) < 0.01f;
    //}

    // Sets direction to a random Direction, sets movementVector to the converted Vector2, and faces toward movementVector.
    private void SetMovementDirection()
    {
        direction = Helper_Directional.RandomCardinalDirection(); // Sets the direction var
        movementVector = Helper_Directional.DirectionToVector(direction); // Static class function
        FaceDirection(direction); // Rotates toward the direction var
    }

    private Vector3Int SetForwardTile() => forwardTile = Vector3Int.RoundToInt(myGridPos + (Vector3)movementVector);
}
