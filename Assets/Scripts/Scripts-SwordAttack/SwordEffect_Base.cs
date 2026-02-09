using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwordEffect_Base : MonoBehaviour
{
    public float damage;
    protected SwordData myData;
    protected Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Sets attributes to given values in data
    public virtual void Initialize(SwordData data, Player playerScript)
    {
        myData = data;
        player = playerScript;
        damage = data.damage;
    }

    private float CalculateDamage() => GlobalConstants.globalDamageMod * (player.attack + damage);

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Sword Trigger");

        // Check if collision has Enemy_Base script attached, and damages if it does.
        Enemy_Base enemy = collision.GetComponent<Enemy_Base>();
        if (enemy != null)
        {
            Debug.Log("Attempting to start coroutine");
            enemy.StartCoroutine(enemy.TakeDirectDamage(CalculateDamage(), myData.weapon, myData.damageType, transform.parent.gameObject.GetComponent<Rigidbody2D>().position));
        }
    }

    // For effects that happen upon pressing the attack button
    protected virtual void SwingEffect()
    {

    }

    // For effects that happen when actually hitting an enemy
    protected virtual void HitEffect()
    {

    }

    //For effects that happen when an enemy dies as a result of hitting them
    // Callback function
    protected virtual void KillEffect()
    {

    }
}
