using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public bool isFalling {  get; set; }

    public void ApplyExternalForce(Vector2 force);

    public IEnumerator FallDown();
}
