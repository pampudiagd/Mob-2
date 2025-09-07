using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGridNav
{
    bool IsWalkable(Vector3 worldPos, bool canFly);

    public TileBase GetWallTile();
    public TileBase GetHoleTile();
}
