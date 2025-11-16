using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Entry : MonoBehaviour
{
    public bool floorStart = false;

    public static Vector3 playerSpawnCoordinate;
    public EntryDirection playerSpawnDirection; // Inspector Variable to manually choose direction

    private Vector2 playerSpawnVector;
    private Vector2Int gridPos;
    private Transform arrow;

    public enum EntryDirection
    {
        up,
        down,
        left,
        right
    };

    // Start is called before the first frame update
    void Awake()
    {
        if (floorStart)
        {
            gridPos = (Vector2Int)LevelManager.Instance.LevelTilemap.WorldToCell(transform.position);
            playerSpawnCoordinate = LevelManager.Instance.LevelTilemap.GetCellCenterWorld(Vector3Int.FloorToInt((Vector3)(playerSpawnVector + gridPos)));
        }

    }

    private void OnValidate()
    {
        arrow = transform.GetChild(0);

        playerSpawnVector = playerSpawnDirection switch
        {
            EntryDirection.up => Vector2.up,
            EntryDirection.down => Vector2.down,
            EntryDirection.left => Vector2.left,
            EntryDirection.right => Vector2.right,
            _ => Vector2.zero
        };

        float angle = Mathf.Atan2(playerSpawnVector.y, playerSpawnVector.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0f, 0f, angle - 90);

    }


}
