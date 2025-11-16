using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// NOT BEING USED ATM


// Sourced from ChatGPT because I hate pathfinding algorithms
public class GridScanner : MonoBehaviour
{ 
    private Tilemap LevelTilemap => LevelManager.Instance.LevelTilemap;

    [Tooltip("Insert ALL tiles that can be walked on!")]
    public List<TileBase> walkableTiles; // All tiles that can be walked on

    private Dictionary<Vector2Int, Node> grid = new(); // Holds all the scanned tiles

    void Start()
    {
        if (LevelTilemap == null)
        {
            Debug.LogError("LevelTilemap not assigned in LevelManager!");
            return;
        }

        ScanTilemap(); // Automatically run on scene start
    }

    // Looks through the whole tilemap and stores each tile in the Dictionary grid
    void ScanTilemap()
    {
        BoundsInt bounds = LevelTilemap.cellBounds; // The full rectangular area of the tilemap

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0); // Unity uses 3D tile coordinates
                TileBase tile = LevelTilemap.GetTile(tilePos); // Stores the tile map tile at (x,y)

                if (tile == null) continue; // Skip empty tiles

                bool walkable = walkableTiles.Contains(tile); // Checks if the current tile map tile is in walkableTiles
                if (!walkable)
                    print(tile + " is not walkable!");
                Vector2Int gridPos = new Vector2Int(x, y); // Convert to 2D grid space

                grid[gridPos] = new Node(gridPos, walkable); // Add the node to the grid
            }
        }
    }

    public Node GetNode(Vector2Int pos) => grid.ContainsKey(pos) ? grid[pos] : null;
}