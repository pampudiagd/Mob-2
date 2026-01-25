using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Test_Dummy : Enemy_Base
{
    //private IGridNav navigator;
    private Behavior_Idle_Wander mover;
    private Behavior_Pursuit_Simple pursuer;

    public GameObject attackPlaceholder;

    private Vector3 targetTilePos => LevelManager.Instance.LevelTilemap.WorldToCell(target.transform.position);

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //navigator = FindObjectOfType<LevelManager>().GetComponent<TilemapNav>();
        mover = GetComponent<Behavior_Idle_Wander>();
        pursuer = GetComponent<Behavior_Pursuit_Simple>();

        SetRandomCardinalVector();
        FaceDirection(movementVector);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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

        SetRandomCardinalVector(); // Sets movementVector to a random direction
        FaceDirection(movementVector);

        Vector3? target = mover.GetNextTarget(MyGridPos, movementVector);
        if (target == null) // Kills movement if there's a wall
        {
            mover.moveRoutine = null;
            return;
        }

        mover.moveRoutine = StartCoroutine(mover.MoveToTile(target.Value, () => myBehaviorState != BehaviorState.Idle || interrupted, moveSpeed)); // Starts the movement step while also ensuring this function won't run again 
    }

    protected override void Behavior_1()
    {
        if (myState != EnemyState.Default || pursuer.moveRoutine != null || myBehaviorState != BehaviorState.Targeting)
            return;

        // Finds the normalized vector between self and target's position, then sums own tilemap position with the normalized vector.
        Vector3 targetTileStep = MyGridPos + (Vector3)Helper_Directional.VectorToTargetOctilinear(targetTilePos, MyGridPos);

        movementVector = Helper_Directional.VectorToTargetCardinal(targetTilePos, MyGridPos, 1);
        FaceDirection(movementVector); // Rotates toward the direction var

        pursuer.moveRoutine = StartCoroutine(pursuer.MoveToTileTargeting(targetTileStep, () => myBehaviorState != BehaviorState.Targeting || interrupted, moveSpeed));
    }

    protected override IEnumerator Behavior_Attack()
    {
        while (isTargetInAtkRng)
        {
            movementVector = Helper_Directional.VectorToTargetCardinal(targetTilePos, MyGridPos);
            FaceDirection(movementVector); // Rotates toward the direction var
            yield return new WaitForSeconds(0.25f);
            attackPlaceholder.SetActive(true);

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.25f);
                //Debug.Log("Simulating attack " + i);
            }
            attackPlaceholder.SetActive(false);
            yield return new WaitForSeconds(0.5f);
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

    //private bool IsCenteredOnTile()
    //{
    //    Vector3 worldCenter = LevelManager.Instance.GridScanner.LevelTilemap.GetCellCenterWorld(myGridPos);
    //    return Vector3.Distance(transform.position, worldCenter) < 0.01f;
    //}
}
