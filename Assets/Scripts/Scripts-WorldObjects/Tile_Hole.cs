using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Hole : MonoBehaviour
{
    public float dragSpeed = 5f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("FallBox"))
        {
            IMovable moveScript = collision.GetComponentInParent<IMovable>();

            if ((collision.transform.position - transform.position).sqrMagnitude > 0.1f)
            {
                float dist = Vector2.Distance(transform.position, collision.transform.position);
                float strength = dragSpeed / Mathf.Max(dist, 0.8f);
                Vector2 dragDir = (transform.position - collision.transform.position).normalized;
                moveScript.ApplyExternalForce(dragDir * strength);
            }
            else
            {
                StartCoroutine(moveScript.FallDown());
            }







            //if (collision.transform.parent.CompareTag("Enemy"))
            //{

            //}
        }
    }
}
