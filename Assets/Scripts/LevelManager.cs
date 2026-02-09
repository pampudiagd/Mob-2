using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public Tilemap LevelTilemap { get; private set; }
    public IGridNav gridNav;
    [SerializeField] private GameObject playerPrefab;
    public GameObject playerInstance;

    // !!!!! Placeholder! Value should be obtained from an Area script!!!!!!!!
    public int areaRoomCount = 10;
    public Room_Metadata[] roomList;
    public Room_Metadata currentRoomScript;

    private Vector3 currentRoomStartCoordinate;

    public int currentKills = 0;
    public int currentFloorID = 0;

    public static LevelManager Instance { get; private set; }
    public GridScanner GridScanner { get; private set; }

    private CameraManager cameraManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //GridScanner = GetComponentInChildren<GridScanner>();

        cameraManager = FindObjectOfType<CameraManager>();
        gridNav = gameObject.AddComponent<TilemapNav>();
        
        playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerInstance.name = "Player";
        AreaStart();
    }

    // Start is called before the first frame update
    void Start()
    {
        //print(Tile_Entry.playerSpawnCoordinate);
        for (int i = 0; i < roomList.Length; i++)
        {
            if (i != currentFloorID)
                roomList[i].gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AreaStart()
    {
        // Creates an array of every room's Room_Metadata script, sorted by roomID
        roomList = FindObjectsOfType<Room_Metadata>();
        Array.Sort(roomList, (a, b) => a.roomID.CompareTo(b.roomID));

        RoomEntered(true);
    }

    // Called upon touching an Exit Tile
    public void RoomEntered(bool isFirstRoom)
    {
        if (!isFirstRoom)
        {
            currentFloorID++;
            roomList[currentFloorID].gameObject.SetActive(true);
            roomList[currentFloorID - 1].gameObject.SetActive(false);
        }

        currentRoomScript = roomList[currentFloorID];
        LevelTilemap = currentRoomScript.RoomTilemap;

        currentKills = 0;
        RoomTransition();
    }

    public void RoomTransition()
    {
        playerInstance.transform.position = currentRoomStartCoordinate;

        print(playerInstance + " " + cameraManager);
        cameraManager.RoomChangeSetCamera(currentRoomScript, playerInstance.transform);
    }

    public void SetPlayerRoomStart(Vector3 startCoordinate)
    {
        currentRoomStartCoordinate = startCoordinate;
    }

}
