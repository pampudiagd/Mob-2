using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room_Metadata : MonoBehaviour
{
    public int roomID;

    public bool allowCameraMovement = false;

    public Vector3 roomGlobalPos
    {
        get {  return transform.position; }
    }

    public Tilemap RoomTilemap
    {
        get { return GetComponentInChildren<Tilemap>(); }
    }

}
