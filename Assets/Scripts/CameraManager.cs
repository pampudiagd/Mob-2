using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera gameCam;

    private void Awake()
    {

    }

    public void RoomChangeSetCamera(Room_Metadata currentRoomScript, Transform followTarget)
    {
        if (currentRoomScript.allowCameraMovement)
        {
            gameCam.GetComponent<CinemachineConfiner2D>().enabled = true;
            gameCam.Follow = followTarget;
            gameCam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = currentRoomScript.gameObject.transform.GetComponentInChildren<PolygonCollider2D>();
        }
        else
        {
            gameCam.GetComponent<CinemachineConfiner2D>().enabled = false;
            gameCam.Follow = null;
            gameCam.transform.position = new Vector3(currentRoomScript.roomGlobalPos.x, currentRoomScript.roomGlobalPos.y + 0.5f, gameCam.transform.position.z);
        }

    }

}
