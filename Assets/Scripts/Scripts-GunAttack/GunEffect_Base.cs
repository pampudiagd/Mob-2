using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffect_Base : MonoBehaviour
{
    public float damage;
    public GameObject myBullet;
    private GameObject tempBullet;
    private GunData myGunData;
    protected Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void Initialize(GunData data, Player playerScript)
    {
        myGunData = data;
        player = playerScript;
        damage = data.damage;
    }

    private float CalculateDamage() => GlobalConstants.globalDamageMod * (player.attack + damage);

    private void OnEnable()
    {
        SpawnBullet();
    }

    // Creates bullet 
    public void SpawnBullet()
    {
        // Instances bullet and grabs its script, then passes along GunData
        tempBullet = Instantiate(myBullet, transform.position + transform.up, transform.rotation);
        Bullet_Base bullet = tempBullet.GetComponent<Bullet_Base>();
        if (bullet != null)
            bullet.Initialize(myGunData.damageType, CalculateDamage(), TargetTag.Enemy);
        Debug.Log("Bullet fired");
    }

}
