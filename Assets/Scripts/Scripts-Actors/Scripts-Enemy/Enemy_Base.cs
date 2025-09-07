using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Base : StatEntity, IDamageable
{
    public EnemyEvent enemyEvent;

    [Header("Enemy Stats")]
    public EnemyData baseStats;
    protected EnemyData myBaseStats; // Clone of statSheet to allow modifying individual enemies' stats without affecting every single instance of an enemy
    
    public bool mortal = true;
    [SerializeField] protected BehaviorState myBehaviorState = BehaviorState.Idle;
    
    protected Rigidbody2D rb;
    protected GridScanner gridScanner;
    protected bool interrupted;
    [SerializeField] protected EnemyState myState = EnemyState.Default;
    public float knockbackSpeed = 14f;
    public float knockDistance = 4f;
    private int activeKnockbacks = 0;

    [Header("Behavior")]
    public Direction direction = Direction.Up;
    public GameObject target;

    public override float moveSpeed => baseStats.moveSpeed;

    protected Vector3Int myGridPos => gridScanner.levelTilemap.WorldToCell(transform.position);

    protected LayerMask environmentLayer;

    protected enum BehaviorState
    {
        Idle,
        Targeting
    }

    // protected = accessed by this class and subclasses
    // virtual = overridable
    // Awake() = class used for initialization, before the game starts
    protected virtual void Awake()
    {
        if (baseStats == null)
        {
            Debug.LogError($"{gameObject.name} has no stats assigned!");
            return;
        }

        environmentLayer = LayerMask.GetMask("Environment-Solid");
        myBaseStats = Instantiate(baseStats);
        healthCurrent = myBaseStats.baseMaxHealth;

    }

    protected virtual void Start()
    {
        gridScanner = FindObjectOfType<Grid>().GetComponent<GridScanner>();
        rb = GetComponent<Rigidbody2D>();
        
        //rb.AddRelativeForce(GetDirectionVector(direction) * myBaseStats.moveSpeed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // ADD OTHER TIMERS HERE LATER

        //if (myState == EnemyState.Knockback)
        //{
        //    knockBackTimer -= Time.deltaTime;
        //    if (knockBackTimer <= 0f)
        //    {
        //        myState = EnemyState.Default;
        //        velocity = Vector2.zero;
        //    }
        //}
    }

    // Override in Enemy_Behavior scripts
    // Should be the enemy's default behavior upon entering a room
    protected virtual void Behavior_0(){}

    protected virtual void Behavior_1(){}

    public void AssignTarget(GameObject gameObject)
    {
        target = gameObject;
    }

    // Takes a Vector2 and rotates to face that direction
    protected void FaceDirection(Direction direction)
    {
        Vector2 dir = Helper_Directional.DirectionToVector(direction);
        float angle = (Mathf.Atan2(dir.y, dir.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    // Called by sword/bullet scripts
    public override IEnumerator TakeDirectDamage(float amount, string damageSource, DamageType damageType, Vector2 sourcePos)
    {
        healthCurrent -= amount;
        if (damageSource == "sword")
        {
            enemyEvent.RaiseEnemyHit();
            interrupted = true;
        }
        Debug.Log($"{gameObject.name} took {amount} damage!");
        ReceiveKnockback(sourcePos);

        if (healthCurrent <= 0 && mortal == true)
        {
            Die();
        }
        yield return null;
    }

    public override void TakePassiveDamage(float amount, DamageType damageType)
    {
        healthCurrent -= amount;

        Debug.Log($"{gameObject.name} took {amount} {damageType} damage!");

        if (healthCurrent <= 0 && mortal == true)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (myBaseStats.deathEffect != null)
            Instantiate(myBaseStats.deathEffect, transform.position, Quaternion.identity);

        enemyEvent.RaiseEnemyDeath(myBaseStats.pointValue);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(" TOUCHING" + other.tag);
        if (other.CompareTag("Player"))
        {
            TouchedPlayer(other.gameObject);
            Debug.Log($"{gameObject.name} touched the player!");
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (myState == EnemyState.Knockback && collision.gameObject.CompareTag("Wall"))
        //{
        //    cancelKnockback = true;
        //}
    }

    public override void ReceiveKnockback(Vector2 sourcePos)
    {
        StartCoroutine(KnockbackCoroutine(sourcePos));
    }

    private IEnumerator KnockbackCoroutine(Vector2 sourcePos)
    {
        activeKnockbacks++;
        myState = EnemyState.Knockback;
        Debug.Log("Knocking back!");
        Vector2 knockDirection = (rb.position - sourcePos).normalized;
        float remainingDistance = knockDistance;

        while (remainingDistance > 0.01f)
        {
            // Step size per frame
            float step = knockbackSpeed * Time.fixedDeltaTime;
            if (step > remainingDistance)
                step = remainingDistance;

            // Look just ahead of the step
            RaycastHit2D hit = Physics2D.Raycast(rb.position, knockDirection, step, LayerMask.GetMask("Environment-Solid"));
            Debug.DrawRay(rb.position, knockDirection * step, Color.green, 0.1f);

            if (hit.collider != null)
            {
                // Found a wall
                Vector2 wallNormal = hit.normal;
                float angle = Vector2.Angle(knockDirection, -wallNormal);

                if (angle <= 60f) // pretty head-on
                {
                    // Stop dead
                    rb.position = hit.point;
                    break;
                }
                else
                {
                    // Slide along wall tangent
                    knockDirection = Vector2.Perpendicular(wallNormal);

                    // Pick the tangent that matches original motion
                    if (Vector2.Dot(knockDirection, rb.position - sourcePos) < 0)
                        knockDirection = -knockDirection;

                    // Move to just before the wall contact
                    rb.position = hit.point - knockDirection * 0.01f;
                }
            }
            else
            {
                // Free move
                rb.MovePosition(rb.position + knockDirection * step);
                remainingDistance -= step;
            }

            yield return new WaitForFixedUpdate();
        }

        activeKnockbacks--;

        yield return new WaitForSeconds(1f);

        if (activeKnockbacks <= 0)
        {
            activeKnockbacks = 0;
            myState = EnemyState.Default;
        }

    }

    protected void Interrupt()
    {
        interrupted = true;
    }

    //OLD Tile-based knockback
    //public override void ReceiveKnockback(GameObject attacker)
    //{
    //    Debug.Log("Knocking back!");
    //    Vector3 knockDirection = (transform.position - attacker.transform.position).normalized;
    //    float knockForce = 20f;
    //    Vector3 targetTile = transform.position + knockDirection * knockForce;
    //    while ((transform.position - targetTile).sqrMagnitude > 0.001f)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, targetTile, 10 * Time.deltaTime);
    //    }
    //}

    // Handles what happens when directly touching the player. Can be overridden in derived classes for additional effects

    public virtual void OnPlayerDetected()
    {
        Debug.Log("ENTERED DETECTION ZONE");
        myBehaviorState = BehaviorState.Targeting;
    }

    public virtual void OnPlayerLost()
    {
        Debug.Log("LEFT DETECTION ZONE");
        myBehaviorState = BehaviorState.Idle;
    }

    protected virtual void TouchedPlayer(GameObject player)
    {
        Player playerScript = player.GetComponent<Player>();

        playerScript.StartCoroutine(playerScript.TakeDirectDamage(myBaseStats.contactDamage, myBaseStats.damageSource, myBaseStats.damageType, this.gameObject.GetComponent<Rigidbody2D>().position));
    }

}
