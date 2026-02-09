using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Base : MonoBehaviour
{
    protected StatEntity actor;
    protected bool canFly = false;

    // Start is called before the first frame update
    void Start()
    {
        actor = GetComponent<Enemy_Base>();
        canFly = actor.canFly;
    }

    // Returns true if the level's tilemap at the position(forwardTile) is a tile that can be crossed by the actor
    public bool CheckTileOpen(Vector3 targetTileWorld)
    {
        if (LevelManager.Instance.gridNav.IsWalkable(targetTileWorld, canFly))
            return true;
        else
            return false;
    }

    //public bool CheckTileHole(Vector3Int targetTile)
    //{
    //    if (LevelManager.Instance.gridNav.IsHoleTile(targetTile))
    //        return true;
    //    else 
    //        return false;
    //}

}
