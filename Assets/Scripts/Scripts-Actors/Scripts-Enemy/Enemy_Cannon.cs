using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Enemy_Cannon : Enemy_Base
{
    [SerializeField] private GameObject myBullet;
    private GameObject newBullet;
    [SerializeField] private float shotDelay = 1f;

    private Vector3[] shotDirections;

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
        shotDirections = new Vector3[]{ transform.up, transform.right, -transform.up, -transform.right };
        
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
        if (myCannonType == CannonType.Single)
            Shoot();
        else
        {
            for (int i = 0; i < shotDirections.Length; i++)
                ShootQuad(i);
        }
        yield return new WaitForSeconds(shotDelay/2);
        if (myCannonType == CannonType.QaudSpin)
        {
            transform.Rotate(0f, 0f, 45f);
            shotDirections = new Vector3[] { transform.up, transform.right, -transform.up, -transform.right };
        }
        yield return new WaitForSeconds(shotDelay/2);
        isAttacking = false;
    }

    private void Shoot()
    {
        newBullet = Instantiate(myBullet, transform.position + transform.up, transform.rotation);
        newBullet.GetComponent<Bullet_Base>().Initialize(DamageType.Normal, GlobalConstants.globalDamageMod * myBaseStats.basePower, TargetTag.Player);
    }

    private void ShootQuad(int current)
    {
        newBullet = Instantiate(myBullet, transform.position + shotDirections[current], Helper_Directional.VectorToQuaternion(shotDirections[current]));
        newBullet.GetComponent<Bullet_Base>().Initialize(DamageType.Normal, GlobalConstants.globalDamageMod * myBaseStats.basePower, TargetTag.Player);
    }
}
