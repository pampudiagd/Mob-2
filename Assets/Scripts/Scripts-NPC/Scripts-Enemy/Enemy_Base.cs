using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : MonoBehaviour
{
    public EnemyEvent enemyEvent;

    [Header("Enemy Stats")]
    public EnemyData statSheet;
    protected EnemyData stats; // Clone of statSheet to allow editing of individual enemies' stats

    public float currentHealth = 5;
    public bool mortal = true;
    
    protected Rigidbody2D rb;

    [Header("Behavior")]
    public Direction direction = Direction.Up;
    public GameObject target;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }

    // protected = accessed by this class and subclasses
    // virtual = overridable
    // Awake() = class used for initialization, before the game starts
    protected virtual void Awake()
    {
        if (statSheet == null)
        {
            Debug.LogError($"{gameObject.name} has no stats assigned!");
            return;
        }

        stats = Instantiate(statSheet);
        currentHealth = stats.maxHealth;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.AddRelativeForce(GetDirectionVector(direction) * stats.moveSpeed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Override in Enemy_Behavior scripts
    // Should be the enemy's default behavior upon entering a room
    protected virtual void Behavior_1()
    {

    }

    public void AssignTarget(GameObject gameObject)
    {
        target = gameObject;
    }

    protected Vector2 GetDirectionVector(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            Direction.UpRight => Vector2.up + Vector2.right,
            Direction.UpLeft => Vector2.up + Vector2.left,
            Direction.DownRight => Vector2.down + Vector2.right,
            Direction.DownLeft => Vector2.down + Vector2.left,
            _ => Vector2.zero
        };
    }

    // Called by sword/bullet scripts
    public virtual void TakeDamage(float amount, string damageSource, string damageType)
    {
        currentHealth -= amount;
        if (damageSource == "sword")
            enemyEvent.RaiseEnemyHit();
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

        enemyEvent.RaiseEnemyDeath();
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
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
