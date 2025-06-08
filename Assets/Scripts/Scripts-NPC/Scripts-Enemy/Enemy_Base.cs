using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : MonoBehaviour
{
    public EnemyDeathEvent deathEvent;

    [Header("Enemy Stats")]
    public EnemyData stats;

    public float currentHealth = 5;
    public bool mortal = true;
    public string direction = "up";

    private Rigidbody2D rb;

    //protected = accessed by this class and subclasses
    //virtual = overridable
    //Awake() = class used for initialization, before the game starts
    protected virtual void Awake()
    {
        if (stats == null)
        {
            Debug.LogError($"{gameObject.name} has no stats assigned!");
            return;
        }

        currentHealth = stats.maxHealth;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //There's probably a way to do this in one dynamic statement
        //but this should work OK for now.
        if (direction == "up")
        {
            rb.AddRelativeForce(Vector2.up * stats.moveSpeed, ForceMode2D.Impulse);
        }
        else if (direction=="down")
        {
            rb.AddRelativeForce(Vector2.down * stats.moveSpeed, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogError($"{gameObject.name} has no valid direction assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called by sword/bullet scripts
    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage!");

        if (currentHealth <= 0 && mortal == true)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (stats.deathEffect != null)
            Instantiate(stats.deathEffect, transform.position, Quaternion.identity);

        deathEvent.Raise();
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TouchedPlayer(other.gameObject);
            Debug.Log($"{gameObject.name} touched the player!");
        }
    }

    // Handles what happens when directly touching the player. Can be overridden in derived classes for additional effects
    protected virtual void TouchedPlayer(GameObject player)
    {
        Player playerScript = player.GetComponent<Player>();

        StartCoroutine(playerScript.TakeDamage(stats.contactDamage));
    }

}
