using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public Tilemap tileMap;
    public IGridNav gridNav;

    // Start is called before the first frame update
    void Awake()
    {
        IGridNav gridNav = gameObject.AddComponent<TilemapNav>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
