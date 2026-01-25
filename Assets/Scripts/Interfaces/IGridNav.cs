using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGridNav
{
    bool IsWalkable(Vector3 gridPos, bool canFly);

    public bool IsRespawnSafe(Vector3Int gridPos);

    bool IsHoleTile(Vector3Int gridPos);

    public TileBase GetWallTile();
    public TileBase GetHoleTile();
}
