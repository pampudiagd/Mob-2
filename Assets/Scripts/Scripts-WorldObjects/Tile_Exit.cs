using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile_Exit : MonoBehaviour
{
    public bool floorEnd = false;

    [Tooltip("Entry Tile to place player at.")]
    public Tile_Entry TargetStartTile;

    public bool isLocked = true;

    public int killRequirement = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.Instance.currentKills >= killRequirement)
            isLocked = false;

        if (!isLocked)
            transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            if (TargetStartTile == null)
                Debug.LogError("No Entry Tile assigned!");
            else
            {
                LevelManager.Instance.currentKills = 0;
                collision.transform.position = TargetStartTile.playerWarpCoordinate;
            }
    }
}
