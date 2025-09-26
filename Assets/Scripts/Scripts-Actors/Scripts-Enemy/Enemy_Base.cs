using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Base : StatEntity, IDamageable, IKnockable
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
    public bool isAttacking = false;
    public bool isTargetInAtkRng = false;
    public bool isTargetSeen = false;
    [SerializeField] protected EnemyState myState = EnemyState.Default;
    //private float knockbackSpeed = GlobalConstants.knockbackSpeed;
    //private float knockDistance = GlobalConstants.knockMagnitudeModifier;
    //private int activeKnockbacks = 0;

    [Header("Behavior")]
    public Direction direction = Direction.Up;
    public GameObject target;

    private KnockHandler knockHandler;

    public override float moveSpeed => baseStats.moveSpeed;

    protected Vector3Int myGridPos => gridScanner.levelTilemap.WorldToCell(transform.position);

    protected LayerMask environmentLayer;

    private SpriteRenderer mySprite;
    public GameObject mySpriteChild;

    protected enum BehaviorState
    {
        Idle,
        Targeting,
        Attacking
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

        knockHandler = GetComponent<KnockHandler>();
        environmentLayer = LayerMask.GetMask("Environment-Solid");
        myBaseStats = Instantiate(baseStats);
        healthCurrent = myBaseStats.baseMaxHealth;

    }

    protected virtual void Start()
    {
        gridScanner = FindObjectOfType<Grid>().GetComponent<GridScanner>();
        rb = GetComponent<Rigidbody2D>();
        mySprite = mySpriteChild.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    // Override in Enemy_Behavior scripts
    // Should be the enemy's default behavior upon entering a room
    protected virtual void Behavior_0() { }

    protected virtual void Behavior_1() { }

    protected virtual IEnumerator Behavior_Attack()
    {
        yield return null;
    }

    public void AssignTarget(GameObject gameObject)
    {
        target = gameObject;
    }

    // Takes a Direction enum and rotates to face that direction
    protected void FaceDirection(Direction direction)
    {
        Vector2 dir = Helper_Directional.DirectionToVector(direction);
        float angle = (Mathf.Atan2(dir.y, dir.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    protected void FaceDirection(Vector2 direction)
    {
        float angle = (Mathf.Atan2(direction.y, direction.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    // Called by sword/bullet scripts
    public override IEnumerator TakeDirectDamage(float amount, string damageSource, DamageType damageType, Vector2 sourcePos)
    {
        Debug.Log("Coroutine started");
        try
        {
            if (IsInvulnerable)
            {
                Debug.Log("Am invulnerable");
                yield break;
            }

            damageInvulnerable = true;
            StartCoroutine(BlinkSprite());
            healthCurrent -= amount;
            if (damageSource == "sword")
            {
                enemyEvent.RaiseEnemyHit();
                interrupted = true;
            }
            Debug.Log($"{gameObject.name} took {amount} damage!");

            if (knockHandler != null)
                yield return StartCoroutine(knockHandler.StartKnockback(sourcePos));


            if (healthCurrent <= 0 && mortal == true)
            {
                Die();
            }

            Debug.Log("After yield");
            damageInvulnerable = false;
        }
        finally
        {
            Debug.Log("Coroutine stopped early");
        }
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


        // Rewrite for collisions with enemies as well!



        //Debug.Log(" TOUCHING" + other.tag);
        //Debug.Log(other.name + "   " + target.name);
        if (other.CompareTag("Player"))
        {
            TouchedPlayer(other.gameObject);
            Debug.Log($"{gameObject.name} touched the player!");
        }

    }

    protected void Interrupt()
    {
        interrupted = true;
    }

    public virtual void OnPlayerDetected()
    {
        myBehaviorState = BehaviorState.Targeting;
        //Debug.Log("Begin Targetting Behavior");
    }

    public virtual void OnPlayerLost()
    {
        //Debug.Log("LEFT DETECTION ZONE");
        myBehaviorState = BehaviorState.Idle;
    }

    public virtual void OnAttackTriggered()
    {
        Debug.Log(gameObject.name + " triggered attack.");
        myBehaviorState = BehaviorState.Attacking;
        isTargetInAtkRng = true;
        isAttacking = true;
    }

    public virtual void OnPlayerOutRange()
    {
        //Debug.Log("Player left attack zone.");
        isTargetInAtkRng = false;
    }

    protected virtual void TouchedPlayer(GameObject player)
    {
        Player playerScript = player.GetComponent<Player>();

        playerScript.StartCoroutine(playerScript.TakeDirectDamage(myBaseStats.contactDamage, myBaseStats.damageSource, myBaseStats.damageType, this.gameObject.GetComponent<Rigidbody2D>().position));
    }

    public void SetState(EnemyState state) => myState = state;

    // Quickly enables/disables enemy's sprite
    private IEnumerator BlinkSprite()
    {
        Debug.Log("Enemy INVULNERABLE");
        while (damageInvulnerable)
        {
            mySprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            mySprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
