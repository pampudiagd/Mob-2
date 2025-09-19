using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockable
{
    void ReceiveKnockback(Vector2 sourcePos);
}
