using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatEntity : MonoBehaviour, IDamageable, IMovable
{
    protected Vector2 externalForce;
    public virtual float attack { get; set; }
    public virtual float healthMax { get; set; }
    public virtual float moveSpeed { get; set; }
    
    public float healthCurrent;

    protected bool allowTriggerCheck;

    public bool isFalling { get; set; }

    public virtual bool damageInvulnerable { get; set; }

    public bool canFly = false;

    //[SerializeField] public bool allowForcedMovement = true;

    public virtual bool IsInvulnerable => damageInvulnerable;

    protected Vector3Int MyGridPos => LevelManager.Instance.LevelTilemap.WorldToCell(transform.position);

    public abstract IEnumerator TakeDirectDamage(float amount, WeaponSource damageSource, DamageType damageType, Vector2 sourcePos);

    public abstract void TakePassiveDamage(float amount, DamageType damageType);

    public void ApplyExternalForce(Vector2 force)
    {
        externalForce += force;
    }

    public abstract IEnumerator FallDown();
}
