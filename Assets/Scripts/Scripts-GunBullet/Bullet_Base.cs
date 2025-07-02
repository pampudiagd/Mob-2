using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
    public float damage;
    public float speed = 9;
    private GunData gunData;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddRelativeForce(Vector2.up * speed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Initialize(GunData data, float playerAttack)
    {
        gunData = data;
        damage = playerAttack;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collision is a wall and deletes bullet if so
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        // Check if collision has Enemy_Base script attached, and damages if it does.    
        Enemy_Base enemy = collision.GetComponent<Enemy_Base>();
        if (enemy != null)
        {
            StartCoroutine(enemy.TakeDirectDamage(damage, "gun", gunData.damageType));
            Destroy(gameObject);
        }
    }
}
