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

    // !!!!! Placeholder! Value should be obtained from an Area script!!!!!!!!
    public int areaRoomCount = 10;
    public GameObject[] roomList;
    //public List<GameObject> roomList = new List<GameObject>();

    public int currentKills = 0;
    public int currentFloorID = 0;

    public Camera mainCam;

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
        mainCam = FindObjectOfType<Camera>();
        AreaStart();
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

    public void AreaStart()
    {

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Change to find objects with script component

        roomList = GameObject.FindGameObjectsWithTag("Room");
        for (int i = 0; i < roomList.Length; i++)
        {
            print(roomList[i]);
        }
    }

    // Called upon touching an Exit Tile
    public void RoomEntered()
    {
        currentKills = 0;
        currentFloorID++;
        RoomTransition();
    }

    public void RoomTransition()
    {
        mainCam.transform.position = new Vector3(roomList[currentFloorID].transform.position.x, roomList[currentFloorID].transform.position.y, mainCam.transform.position.z);
    }

}
