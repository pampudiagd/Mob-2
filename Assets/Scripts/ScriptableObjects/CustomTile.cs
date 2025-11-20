using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewCustomTile", menuName = "Tiles/Custom Tile")]
public class CustomTile : Tile
{
    public bool isWalkable = true;
    public bool allowFlyOver = true;
    public bool allowRespawn = true;
}
