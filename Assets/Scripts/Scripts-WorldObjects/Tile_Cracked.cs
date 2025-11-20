using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile_Cracked : MonoBehaviour
{
    public TileBase tileHole;
    public static TileBase gridHole;

    public GameObject holePrefab;
    public static GameObject holeObject;

    public float timer;
    public const float timerLength = 2f;
    private bool isTimerCounting = false;

    // Start is called before the first frame update
    private void Awake()
    {
        gridHole = tileHole;
        holeObject = holePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerCounting && timer > 0)
            TimerIncrement();
        else if (isTimerCounting)
            CollapseTile();
    }

    private void CollapseTile()
    {
        Vector3Int myGridPos = LevelManager.Instance.LevelTilemap.WorldToCell(transform.position);
        LevelManager.Instance.LevelTilemap.SetTile(myGridPos, gridHole);
        Instantiate(holeObject, LevelManager.Instance.LevelTilemap.GetCellCenterWorld(myGridPos), Quaternion.identity, gameObject.transform);
    }


    private void TimerIncrement()
    {
        timer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        timer = timerLength;
        isTimerCounting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isTimerCounting = false;
    }

}
