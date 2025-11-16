using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapNav : MonoBehaviour, IGridNav
{
    private Tilemap LevelTilemap => LevelManager.Instance.LevelTilemap;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase holeTile;

    public bool IsWalkable(Vector3Int gridPos, bool canFly)
    {
        TileBase tile = LevelTilemap.GetTile(gridPos);
        //print("Is the tile at " + gridPos + " a hole? " + (tile == holeTile));
        if (tile == null) return false;
        if (tile == wallTile && !canFly) return false;
        if (tile == holeTile && !canFly) return false;

        return true;
    }

    public TileBase GetWallTile() => wallTile;
    public TileBase GetHoleTile() => holeTile;

    public bool IsHoleTile(Vector3Int gridPos)
    {
        TileBase tile = LevelTilemap.GetTile(gridPos);
        return tile == holeTile;
    }

}
