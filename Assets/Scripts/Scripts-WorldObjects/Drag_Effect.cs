using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Causes unwanted movement when falling into a hole; REWORK


public class Drag_Effect : MonoBehaviour
{
    public static Dictionary<GameObject, Drag_Effect> ActiveHoles = new();

    public float dragSpeed = 5f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("FallBox"))
        {
            Drag_Effect currentDrag;
            if (!ActiveHoles.TryGetValue(collision.gameObject, out currentDrag))
                ActiveHoles[collision.gameObject] = this;
            else
            {
                float myDist = Vector2.Distance(transform.position, collision.transform.position);
                float otherDist = Vector2.Distance(currentDrag.transform.position, collision.transform.position);
                if (myDist < otherDist)
                    ActiveHoles[collision.gameObject] = this;
            }

            if (ActiveHoles[collision.gameObject] == this)
            {
                IMovable moveScript = collision.GetComponentInParent<IMovable>();

                float dist = Vector2.Distance(transform.position, collision.transform.position);
                float strength = dragSpeed / Mathf.Max(dist, 0.5f);
                Vector2 dragDir = (transform.position - collision.transform.position).normalized;
                moveScript.ApplyExternalForce(dragDir * strength);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove if exiting the currently assigned hole
        if (ActiveHoles.TryGetValue(collision.gameObject, out Drag_Effect currentHole) && currentHole == this)
        {
            ActiveHoles.Remove(collision.gameObject);
        }
    }
}
