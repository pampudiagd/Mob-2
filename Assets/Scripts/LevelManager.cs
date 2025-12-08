using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Tilemap levelTilemap; // The tile layer
    public Tilemap LevelTilemap => levelTilemap;

    public IGridNav gridNav;
    [SerializeField] private GameObject playerPrefab;
    public GameObject playerInstance;

    public int currentKills = 0;

    public static LevelManager Instance { get; private set; }
    public GridScanner GridScanner { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //GridScanner = GetComponentInChildren<GridScanner>();

        gridNav = gameObject.AddComponent<TilemapNav>();
    }

    // Start is called before the first frame update
    void Start()
    {

        print(Tile_Entry.playerSpawnCoordinate);
        playerInstance = Instantiate(playerPrefab, Tile_Entry.playerSpawnCoordinate, Quaternion.identity, gameObject.transform);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
