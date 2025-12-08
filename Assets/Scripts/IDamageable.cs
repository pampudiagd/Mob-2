using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    IEnumerator TakeDirectDamage(float amount, WeaponSource damageSource, DamageType damageType, Vector2 sourcePos);
    void TakePassiveDamage(float amount, DamageType damageType);
}
