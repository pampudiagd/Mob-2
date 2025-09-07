using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapNav : MonoBehaviour, IGridNav
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase holeTile;

    public bool IsWalkable(Vector3 worldPos, bool canFly)
    {
        Vector3Int cellPos = tileMap.WorldToCell(worldPos);
        TileBase tile = tileMap.GetTile(cellPos);

        if (tile == null) return false;
        if (tile == wallTile && !canFly) return false;
        if (tile == holeTile && !canFly) return false;

        return true;
    }

    public TileBase GetWallTile() => wallTile;
    public TileBase GetHoleTile() => holeTile;

}
