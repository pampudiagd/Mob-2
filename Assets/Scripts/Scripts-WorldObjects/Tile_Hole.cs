using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Hole : MonoBehaviour
{
    public float dragSpeed = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FallBox"))
        {
            print("TOUCHED SOMEONE");
            IMovable moveScript = collision.GetComponentInParent<IMovable>();
            StartCoroutine(moveScript.FallDown());
        }
    }
}
