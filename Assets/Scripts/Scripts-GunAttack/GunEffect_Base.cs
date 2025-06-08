using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffect_Base : MonoBehaviour
{
    public float damage;
    public GameObject myBullet;
    private GameObject tempBullet;
    private GunData myGunData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void Initialize(GunData data)
    {
        myGunData = data;
        damage = data.damage;
    }

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
            bullet.Initialize(myGunData);
        Debug.Log("Bullet fired");
    }

}
