using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Tilemap levelTilemap; // The tile layer
    public Tilemap LevelTilemap => levelTilemap;

    public IGridNav gridNav;

    public static LevelManager Instance { get; private set; }
    public GridScanner GridScanner { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GridScanner = GetComponentInChildren<GridScanner>();


        gridNav = gameObject.AddComponent<TilemapNav>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
