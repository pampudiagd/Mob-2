using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Enemy_Behavior_Flee : Enemy_Base
{
    public int gridMoveIncrements;
    private Behavior_Idle_Wander mover;
    private Behavior_Flee_Simple runner;
    //private IGridNav navigator;

    private Vector3 targetTilePos => LevelManager.Instance.LevelTilemap.WorldToCell(target.transform.position);

    //Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //navigator = FindObjectOfType<LevelManager>().GetComponent<TilemapNav>();
        mover = GetComponent<Behavior_Idle_Wander>();
        runner = GetComponent<Behavior_Flee_Simple>();

        SetRandomOctilinearVector();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

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

        SetRandomOctilinearVector(); // Sets movementVector to a random direction

        Vector3? target = mover.GetNextTarget(transform, movementVector, gridMoveIncrements);

        if (target == null) // Kills movement if there's a wall
        {
            mover.moveRoutine = null;
            return;
        }

        mover.moveRoutine = StartCoroutine(mover.MoveToTile(target.Value, () => myBehaviorState != BehaviorState.Idle || interrupted, moveSpeed, 2.5f, 3.5f)); // Starts the movement step while also ensuring this function won't run again 
    }

    protected override void Behavior_1()
    {
        if (myState != EnemyState.Default || runner.moveRoutine != null || myBehaviorState != BehaviorState.Targeting)
            return;

        Vector3 targetTileStep = LevelManager.Instance.LevelTilemap.GetCellCenterWorld(Vector3Int.RoundToInt(MyGridPos + (Vector3)(gridMoveIncrements * (-Helper_Directional.VectorToTargetOctilinear(targetTilePos, MyGridPos)))));

        //movementVector = Helper_Directional.VectorToTargetCardinal(targetTilePos, myGridPos, 1);
        //FaceDirection(movementVector); // Rotates toward the direction var

        runner.moveRoutine = StartCoroutine(runner.MoveToTileTargeting(targetTileStep, () => myBehaviorState != BehaviorState.Targeting || interrupted, moveSpeed, 0.5f, 1f));

    }
}
