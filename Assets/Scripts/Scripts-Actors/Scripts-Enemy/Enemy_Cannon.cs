using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Cannon : Enemy_Base
{
    [SerializeField] private GameObject myBullet;
    private GameObject newBullet;
    [SerializeField] private float shotDelay = 1f;


    private enum CannonType
    {
        Single,
        Quad,
        QaudSpin
    }

    [SerializeField] private CannonType myCannonType = CannonType.Single;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (!isAttacking)
            StartCoroutine(Behavior_Attack());
    }

    protected override IEnumerator Behavior_Attack()
    {
        isAttacking = true;
        newBullet = Instantiate(myBullet, transform.position + transform.up, transform.rotation);
        newBullet.GetComponent<Bullet_Base>().Initialize(DamageType.Normal, GlobalConstants.globalDamageMod * myBaseStats.basePower, TargetTag.Player);
        yield return new WaitForSeconds(shotDelay);
        isAttacking = false;
    }
}
