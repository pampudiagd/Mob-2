using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEffect_Base : MonoBehaviour
{
    public float damage;
    protected SwordData myData;
    protected Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Sets attributes to given values in data
    public virtual void Initialize(SwordData data, Player playerScript)
    {
        myData = data;
        player = playerScript;
        damage = data.damage;
    }

    private float CalculateDamage() => player.globalDamageMod * (player.attack + damage);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Sword Trigger");

        // Check if collision has Enemy_Base script attached, and damages if it does.
        Enemy_Base enemy = collision.GetComponent<Enemy_Base>();
        if (enemy != null)
        {
            Debug.Log("Attempting to start coroutine");
            enemy.StartCoroutine(enemy.TakeDirectDamage(CalculateDamage(), "sword", myData.damageType, transform.parent.gameObject.GetComponent<Rigidbody2D>().position));
            //IKnockable knockable = collision.GetComponent<IKnockable>();
            //if (knockable != null)
            //    knockable.ReceiveKnockback(transform.parent.gameObject.GetComponent<Rigidbody2D>().position);


        }
    }
}
