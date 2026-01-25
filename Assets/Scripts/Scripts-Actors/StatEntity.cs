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

    // The actor's position relative to the current room, flattened into the current tilemap's coordinates, and finally adjusted to align with the center of the tile
    protected Vector3 MyGridPos => LevelManager.Instance.LevelTilemap.GetCellCenterLocal(LevelManager.Instance.LevelTilemap.LocalToCell(transform.localPosition));

    public abstract IEnumerator TakeDirectDamage(float amount, WeaponSource damageSource, DamageType damageType, Vector2 sourcePos);

    public abstract void TakePassiveDamage(float amount, DamageType damageType);

    public void ApplyExternalForce(Vector2 force)
    {
        externalForce += force;
    }

    public abstract IEnumerator FallDown();
}
