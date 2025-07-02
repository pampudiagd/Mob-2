using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    IEnumerator TakeDirectDamage(float amount, string damageSource, DamageType damageType);
    void TakePassiveDamage(float amount, DamageType damageType);
}
