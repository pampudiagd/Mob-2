using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
    [SerializeField] private TargetTag hitTag;
    [SerializeField] private float damage;
    [SerializeField] private DamageType type;
    [SerializeField] private WeaponSource weapon;

    public float speed = 9;
    
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

    public virtual void Initialize(DamageType damageType, float damageTotal, TargetTag tagType)
    {
        hitTag = tagType;
        type = damageType;
        damage = damageTotal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collision is a wall and deletes bullet if so
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Shield-Player"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag(hitTag.ToString()))
        {
            print("MATCHED");
            // Check if collision has an IDamageable script attached, and damages if it does.    
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                MonoBehaviour targetMB = target as MonoBehaviour;
                if (targetMB != null)
                    targetMB.StartCoroutine(target.TakeDirectDamage(damage, weapon, type, this.gameObject.GetComponent<Rigidbody2D>().position));
                Destroy(gameObject);
            }
        }
    }
}
