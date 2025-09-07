using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatEntity : MonoBehaviour, IDamageable
{
    public virtual float attack { get; set; }
    public virtual float healthMax { get; set; }
    public virtual float moveSpeed { get; set; }
    
    public float healthCurrent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public abstract IEnumerator TakeDirectDamage(float amount, string damageSource, DamageType damageType, Vector2 sourcePos);

    public abstract void TakePassiveDamage(float amount, DamageType damageType);

    public abstract void ReceiveKnockback(Vector2 sourcePos);
}
