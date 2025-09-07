using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// Sourced from ChatGPT because I hate pathfinding algorithms
public class GridScanner : MonoBehaviour
{
    [HideInInspector]
    public Tilemap levelTilemap; // The tile layer

    [Tooltip("Insert ALL tiles that can be walked on!")]
    public List<TileBase> walkableTiles; // All tiles that can be walked on

    private Dictionary<Vector2Int, Node> grid = new(); // Stores all the scanned tiles

    void Start()
    {
        levelTilemap = GetComponentInChildren<Tilemap>();
        ScanTilemap(); // Automatically run on scene start
    }

    void ScanTilemap()
    {
        BoundsInt bounds = levelTilemap.cellBounds; // The full rectangular area of the tilemap

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0); // Unity uses 3D tile coordinates
                TileBase tile = levelTilemap.GetTile(tilePos);

                if (tile == null) continue; // Skip empty tiles

                bool walkable = walkableTiles.Contains(tile); // Checks if the current tile is walkable
                if (!walkable)
                    print(tile + " is not walkable!");
                Vector2Int gridPos = new Vector2Int(x, y); // Convert to 2D grid space

                grid[gridPos] = new Node(gridPos, walkable); // Add the node to the grid
            }
        }
    }

    public Node GetNode(Vector2Int pos) => grid.ContainsKey(pos) ? grid[pos] : null;
}