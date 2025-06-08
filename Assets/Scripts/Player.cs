using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float myHealth = 100;
    //private float myMaxHealth = 100;
    //public float myXP = 0;

    public int ammoCount = 4;
    public int ammoCharge = 0;
    private int ammoChargeMax = 4;

    public float moveSpeed = 10;

    [Header("Bools")]
    public bool canMove = true;
    public bool isAttacking = false;

    private float moveDelay = 0.4f; // Seconds before player can move after an attack
    private float moveTimer;

    //public float comboStart = 5;
    //public float comboTimer = 0;

    [Header("Item Data")]
    public SwordData mySwordData; // Determines and holds the data of the current sword
    private GameObject mySwordObject; // Current sword object for interactions(collision, special effects), informed by SwordData

    public GunData myGunData; // Determines and holds the data of the current gun
    private GameObject myGunObject; // Current gun object for handling gun's unique effects/bullets, informed by GunData

    private SpriteRenderer mySprite;
    private CircleCollider2D hurtBox; // Component that detects collisions with damage-sources
    private Rigidbody2D rb; // Component that allows player to be stopped by walls
    private Vector2 input;

    [Header("Event Listeners")]
    public EnemyDeathEvent deathEvent;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        hurtBox = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        EquipSword(Resources.Load<SwordData>("SwordData/Ice Sword")); // Grabs sword from filepath and instantiates its related object
        EquipGun(Resources.Load<GunData>("GunData/Base Gun")); // Grabs gun from filepath and instantiates its related object

        Debug.Log(mySwordData.swordName);
    }

    private void OnEnable()
    {
        deathEvent.onEnemyDeath.AddListener(OnEnemyDeath);
    }

    void OnDisable()
    {
        deathEvent.onEnemyDeath.RemoveListener(OnEnemyDeath);
    }

    void Update()
    {
        MoveAndRotate();

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
            StartCoroutine(SwordAttackCoroutine());
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && ammoCount > 0)
            StartCoroutine(GunAttackCoroutine());

        if (moveTimer > 0)
            StallMove();
    }

    // Moves the player based on current inputs
    private void MoveAndRotate()
    {
        // Get raw input (no smoothing, instant start/stop)
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Ends execution if standing still
        if (input == Vector2.zero) return;

        // Rotates towards cardinal directions
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > 0)
                transform.rotation = Quaternion.Euler(0, 0, 270);   // Right
            else
                transform.rotation = Quaternion.Euler(0, 0, 90);    // Left
        }
        else
        {
            if (input.y > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);     // Up
            else
                transform.rotation = Quaternion.Euler(0, 0, 180);   // Down
        }

        if (!canMove) return;

        // Clamp to 8 directions only
        if (input.x != 0 && input.y != 0)
            input *= 0.7071f; // Normalize diagonal movement (1/sqrt(2))

        Vector2 newPos = rb.position + input * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    // Positions and enables sword, calls its attack, then disables
    IEnumerator SwordAttackCoroutine()
    {
        isAttacking = true;
        canMove = false;
        moveTimer = moveDelay;

        mySwordObject.transform.position = transform.position + transform.up;
        mySwordObject.transform.rotation = transform.rotation;
        mySwordObject.gameObject.SetActive(true);
        mySwordObject.GetComponent<BoxCollider2D>().enabled = true;

        // Call sword effects here

        yield return new WaitForSeconds(0.2f);

        mySwordObject.gameObject.SetActive(false);
        mySwordObject.GetComponent<BoxCollider2D>().enabled = false;

        isAttacking = false;
    }

    // Positions and enables gun, calls its attack, then disables
    IEnumerator GunAttackCoroutine()
    {
        ammoCount--;
        Debug.Log($"New ammo count is {ammoCount}.");
        isAttacking = true;
        canMove = false;
        moveTimer = moveDelay;

        myGunObject.transform.position = transform.position + transform.up;
        myGunObject.transform.rotation = transform.rotation;
        myGunObject.gameObject.SetActive(true);

        // Call gun effects here

        yield return new WaitForSeconds(0.2f);

        myGunObject.gameObject.SetActive(false);

        isAttacking = false;
    }

    // Timer used to re-enable player movement
    void StallMove()
    {
        moveTimer -= Time.deltaTime;
        
        if (moveTimer <= 0)
            canMove = true;
    }

    // Instantiate and prepare sword
    public void EquipSword(SwordData swordData)
    {
        if (mySwordObject != null)
            Destroy(mySwordObject);

        // Sets mySwordData to given argument, and instantiates the prefab contained within
        mySwordData = swordData;
        mySwordObject = Instantiate(mySwordData.swordAttackPrefab, transform.position + transform.up, transform.rotation, gameObject.transform);


        // Grabs the sword prefab's script and passes along SwordData
        SwordEffect_Base sword = mySwordObject.GetComponent<SwordEffect_Base>();
        if (sword != null)
            sword.Initialize(mySwordData);

        mySwordObject.gameObject.SetActive(false);
    }

    // Instantiate and prepare gun
    public void EquipGun(GunData gunData)
    {
        if (myGunObject != null)
            Destroy(myGunObject);

        // Sets myGunData to given argument, and instantiates the prefab contained within
        myGunData = gunData;
        myGunObject = Instantiate(myGunData.GunAttackPrefab, transform.position + transform.up, transform.rotation, gameObject.transform);

        // Grabs the gun prefab's script and passes along GunData
        GunEffect_Base gun = myGunObject.GetComponent<GunEffect_Base>();
        if (gun != null)
            gun.Initialize(myGunData);

        myGunObject.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with enemy!");
        }
    }

    public IEnumerator TakeDamage(float amount)
    {
        myHealth -= amount;
        Debug.Log("Player took damage.");

        if (myHealth <= 0)
        {
            // Do player death
        }
        else
        {
            hurtBox.enabled = false;
            yield return StartCoroutine(BlinkSprite());
            hurtBox.enabled = true;
        }
    }

    // Quickly enables/disables player's sprite
    private IEnumerator BlinkSprite()
    {
        for (int i = 0; i < 5; i++)
        {
            mySprite.enabled = false;
            yield return new WaitForSeconds(0.2f);
            mySprite.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnEnemyDeath()
    {
        ammoCharge += 1;
        Debug.Log($"Current ammo charge is {ammoCharge} / {ammoChargeMax}.");

        if (ammoCharge >= ammoChargeMax)
        {
            ammoCharge = 0;
            ammoCount++;
            Debug.Log("Made a full ammo!");
        }
    }
}
