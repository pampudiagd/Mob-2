using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public void ApplyExternalForce(Vector2 force);

    public IEnumerator FallDown();
}
