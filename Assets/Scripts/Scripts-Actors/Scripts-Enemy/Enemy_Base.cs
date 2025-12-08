using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Base : StatEntity, IKnockable
{
    public EnemyEvent enemyEvent;
    public Vector3Int GridCoord;
    [Header("Enemy Stats")]
    public EnemyData baseStats;
    protected EnemyData myBaseStats; // Clone of statSheet to allow modifying individual enemies' stats without affecting every single instance of an enemy

    public bool mortal = true;
    [SerializeField] protected BehaviorState myBehaviorState = BehaviorState.Idle;

    protected Rigidbody2D rb;
    protected bool interrupted = false;
    public bool isAttacking = false;
    public bool isTargetInAtkRng = false;
    public bool isTargetSeen = false;
    [SerializeField] protected EnemyState myState = EnemyState.Default;

    [Header("Behavior")]
    public Direction direction = Direction.Up;
    protected Vector2 movementVector; // Vector2D used by enemies who move in steps
    //protected Vector3Int forwardTile;
    public GameObject target;

    private KnockHandler knockHandler;

    public override float moveSpeed => baseStats.baseSpeed;

    protected LayerMask environmentLayer;

    private Collider2D[] holeColliders;
    private Collider2D myCollider;
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

        allowTriggerCheck = true;
        if (knockHandler != null)
        {
            knockHandler.OnKnockbackStarted += LockAction;
            knockHandler.OnKnockbackEnded += UnlockAction;
        }
    }

    protected virtual void Start()
    {
        AssignTarget(LevelManager.Instance.playerInstance);
        print("POST TARGET ASSIGNING: " + target);
        FaceDirection(direction);
        rb = GetComponent<Rigidbody2D>();
        mySprite = mySpriteChild.GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        holeColliders = FindObjectsOfType<Collider2D>()
            .Where(c => c.gameObject.layer == LayerMask.NameToLayer("Enemy-Blocker"))
            .ToArray();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        GridCoord = LevelManager.Instance.LevelTilemap.WorldToCell(transform.position);
    }

    void OnDisable()
    {
        if (knockHandler != null)
        {
            knockHandler.OnKnockbackStarted -= LockAction;
            knockHandler.OnKnockbackEnded -= UnlockAction;
        }
    }

    // Override in Enemy_Behavior scripts
    // Should be the enemy's default behavior upon entering a room
    protected virtual void Behavior_0() { }

    // Enemy's behavior upon player entering their detection zone (If they have one)
    protected virtual void Behavior_1() { }

    // Enemy's behavior for directly attacking the player
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
        Vector2 movementVector = Helper_Directional.DirectionToVector(direction);
        float angle = (Mathf.Atan2(movementVector.y, movementVector.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    protected void FaceDirection(Vector2 dirVector)
    {
        float angle = (Mathf.Atan2(dirVector.y, dirVector.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
        direction = Helper_Directional.VectorToDirection(dirVector);
    }

    // Called by sword/bullet scripts
    public override IEnumerator TakeDirectDamage(float amount, WeaponSource damageSource, DamageType damageType, Vector2 sourcePos)
    {
        if (IsInvulnerable)
            yield break;

        damageInvulnerable = true;
        StartCoroutine(BlinkSprite());
        healthCurrent -= amount;
        if (damageSource == WeaponSource.Sword)
            enemyEvent.RaiseEnemyHit();

        Debug.Log($"{gameObject.name} took {amount} damage!");

        if (knockHandler != null && damageSource != WeaponSource.Gun)
        {
            DisableHoleCollision();
            SetState(EnemyState.Knockback);
            yield return StartCoroutine(knockHandler.StartKnockback(sourcePos));
            EnableHoleCollision();
        }

        if (myState == EnemyState.Knockback)
            SetState(EnemyState.Default);

        if (healthCurrent <= 0 && mortal == true)
        {
            Die();
        }

        damageInvulnerable = false;
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
        LevelManager.Instance.currentKills++;
        Destroy(gameObject);
    }

    public override IEnumerator FallDown()
    {
        if (myState == EnemyState.Falling)
            yield break;

        SetState(EnemyState.Falling);
        //allowForcedMovement = false;

        // Trigger fall animation here!

        print("Simulating fall");
        yield return StartCoroutine(PlaceholderFallAnimation());
        Die();
    }

    private IEnumerator PlaceholderFallAnimation()
    {
        for (int i = 0; i < 20; i++)
        {
            transform.Rotate(0, 0, 50);
            transform.localScale *= 0.9f;
            yield return new WaitForSeconds(0.1f);
        }
        transform.localScale = new Vector3(1, 1, 1);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {


        // Rewrite for collisions with enemies as well!



        if (other.CompareTag("Player"))
        {
            StartCoroutine(TouchedPlayer(other.gameObject));
            Debug.Log($"{gameObject.name} touched the player!");
        }

    }

    // Sets movementVector to a random vector.
    protected void SetRandomCardinalVector() => movementVector = Helper_Directional.RandomCardinalVector();

    protected void SetRandomOctilinearVector() => movementVector = Helper_Directional.RandomOctilinearVector();

    protected void LockAction() => interrupted = true;

    protected void UnlockAction() => interrupted = false;

    private void DisableHoleCollision()
    {
        foreach (var hole in holeColliders)
            Physics2D.IgnoreCollision(myCollider, hole, true);
    }

    private void EnableHoleCollision()
    {
        foreach (var hole in holeColliders)
            Physics2D.IgnoreCollision(myCollider, hole, false);
    }

    public virtual void OnPlayerDetected()
    {
        myBehaviorState = BehaviorState.Targeting;
    }

    public virtual void OnPlayerLost()
    {
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
        isTargetInAtkRng = false;
    }

    protected virtual IEnumerator TouchedPlayer(GameObject player)
    {
        if (!allowTriggerCheck)
            yield break;

        allowTriggerCheck = false;
        Player playerScript = player.GetComponent<Player>();

        yield return playerScript.StartCoroutine(playerScript.TakeDirectDamage(myBaseStats.basePower / 2, myBaseStats.damageSource, myBaseStats.damageType, this.gameObject.GetComponent<Rigidbody2D>().position));

        allowTriggerCheck = true;
    }

    public void SetState(EnemyState state) => myState = state;

    // Quickly enables/disables enemy's sprite
    private IEnumerator BlinkSprite()
    {
        while (damageInvulnerable)
        {
            mySprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            mySprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
