using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class Player : StatEntity, IDamageable
{
    [Header("Base Stats")]
    public PlayerData myBaseStats;

    [Header("Current Stats")]
    public int ammoCount = 4;
    public int ammoCharge = 0;
    private int ammoChargeMax = 4;

    //public override float moveSpeed { get; set; } = 10;
    public float rollMod = 5;
    public float rollSpeed = 40f;
    public float rollInvulWindow = 0.07f;
    private float invulTimer;

    public float energy = 0;
    public int lives = 0;

    public float globalDamageMod = 1; // Modifier for incoming/outgoing damage

    private List<StatModifier> healthMods = new List<StatModifier>();
    private List<StatModifier> energyMods = new List<StatModifier>();
    private List<StatModifier> attackMods = new List<StatModifier>();
    private List<StatModifier> ammoMods = new List<StatModifier>();
    private List<StatModifier> speedMods = new List<StatModifier>();

    public override float attack => CalculateStat(myBaseStats.basePower, attackMods);
    public override float healthMax => CalculateStat(myBaseStats.baseMaxHealth, healthMods);
    public float energyMax => CalculateStat(myBaseStats.baseMaxEnergy, energyMods);
    public float ammoMax => CalculateStat(myBaseStats.baseMaxAmmo, ammoMods);
    public override float moveSpeed => CalculateStat(myBaseStats.baseSpeed, speedMods);


    [Header("Bools")]
    public bool isAttacking = false;
    public bool isRolling = false;
    public bool isInvulnerable = false;

    private float moveDelayAttack = 0.4f; // Seconds before player can move after an attack
    private float moveDelayRoll = 0.05f; // Seconds before player can move after a roll
    private float moveTimer; // Timer that will be set to a moveDelay variable

    private float rollCooldown = 0.5f;
    private float rollTimer;

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
    private Vector2 rollInput;

    [Header("Event Listeners")]
    public EnemyEvent enemyEvent;

    [Header("Event Emitters")]
    public PlayerEvent playerEvent;

    //[Header("UI")]
    //Variables to store HeartsVisual.cs and AmmoVisual.cs for UI
    //HeartsVisual heartsVisualCS;
    //AmmoVisual ammoVisualCS;

    // Start is called before the first frame update
    void Start()
    {
        healthCurrent = myBaseStats.baseMaxHealth;
        moveSpeed = myBaseStats.baseSpeed;
        rollSpeed = myBaseStats.BaseRollSpeed;
        rollInvulWindow = myBaseStats.baseRollInvulWindow;

        mySprite = GetComponent<SpriteRenderer>();
        hurtBox = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        EquipSword(Resources.Load<SwordData>("Sword Stats/Ice Sword")); // Grabs sword from filepath and instantiates its related object
        EquipGun(Resources.Load<GunData>("Gun Stats/Base Gun")); // Grabs gun from filepath and instantiates its related object

        //This is for the UI, and while .Find seems to sometimes pose problems, it should work OK if put in Start()Add commentMore actions
        //GameObject heartsVisualObject = GameObject.Find("HeartsVisual");
        //GameObject ammoVisualObject = GameObject.Find("AmmoVisual");

        //if (heartsVisualObject != null)
        //{
        //    Add commentMore actions
        //    heartsVisualCS = heartsVisualObject.GetComponent<HeartsVisual>(); //now we can reference HeartsVisual.cs
        //}

        //if (ammoVisualObject != null)
        //{
        //    ammoVisualCS = ammoVisualObject.GetComponent<AmmoVisual>(); //now we can reference AmmoVisual.cs
        //}

        Debug.Log(mySwordData.swordName);
    }

    void OnEnable()
    {
        enemyEvent.onEnemyDeath.AddListener(OnEnemyDeath);
        enemyEvent.onEnemyHit.AddListener(OnEnemyHit);
    }

    void OnDisable()
    {
        enemyEvent.onEnemyDeath.RemoveListener(OnEnemyDeath);
        enemyEvent.onEnemyHit.RemoveListener(OnEnemyHit);
    }

    void Update()
    {
        // Prevents movement/attacks during a roll's execution
        if (isRolling)
            return;

        ReadInput();

        if (moveTimer > 0)
            StallTimer(ref moveTimer);
        if (rollTimer > 0)
            StallTimer(ref rollTimer);
    }

    // Occurs every 0.2 seconds (50 per second) (Independent of framerate)
    private void FixedUpdate()
    {
        MoveAndRotate();
        if (isInvulnerable)
            InvulTimer();
    }

    // Takes the player's input and calls methods/stores it
    private void ReadInput()
    {
        // Get raw input (no smoothing, instant start/stop)
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isAttacking && rollTimer <= 0)
            StartCoroutine(Roll());
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
            StartCoroutine(SwordAttackCoroutine());
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && ammoCount > 0)
            StartCoroutine(GunAttackCoroutine());
    }

    // Moves the player based on current inputs
    private void MoveAndRotate()
    {
        // Saves the last non-stationary movement vector to allow rolling from a stand-still
        if (input != Vector2.zero)
            rollInput = input;

        // Ends execution if standing still
        if (input == Vector2.zero) return;

        // Rotates towards 8 directions
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Prevents the movement step from running during an attack, allowing the player to only rotate like in GB Zelda
        if (moveTimer > 0) return;

        Vector2 moveInput = input;
        // Clamp to 8 directions only
        if (moveInput.x != 0 && moveInput.y != 0)
            moveInput *= 0.7071f; // Normalize diagonal movement (1/sqrt(2))

        Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    // Grants player a burst of speed while locking their movement and actions and granting a brief window of invulnerability
    IEnumerator Roll()
    {
        isRolling = true;
        rollTimer = rollCooldown;
        isInvulnerable = true;

        float elapsed = 0f;
        float duration = rollMod / rollSpeed; // Total roll duration
        Vector2 start = rb.position;
        Vector2 target = start + rollInput.normalized * rollMod; // Distance of roll
        invulTimer = rollInvulWindow;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            rb.MovePosition(Vector2.Lerp(start, target, t));

            yield return new WaitForFixedUpdate(); // Wait for next FixedUpdate
        }

        rb.MovePosition(target); // Ensure precise final position

        isRolling = false;
        moveTimer = moveDelayRoll;
    }

    // Timer used to end invulnerability from rolling
    private void InvulTimer() 
    {
        invulTimer -= Time.fixedDeltaTime;
        if (invulTimer <= 0f)
            isInvulnerable = false;
    }

    // Positions and enables sword, calls its attack, then disables
    IEnumerator SwordAttackCoroutine()
    {
        isAttacking = true;
        moveTimer = moveDelayAttack;

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
        moveTimer = moveDelayAttack;

        //Shows ammo change in UI
        //if (ammoVisualCS != null)
        //{
        //    ammoVisualCS.UpdateAmmo(ammoCount);
        //}

        myGunObject.transform.position = transform.position + transform.up;
        myGunObject.transform.rotation = transform.rotation;
        myGunObject.gameObject.SetActive(true);

        // Call gun effects here

        yield return new WaitForSeconds(0.2f);

        myGunObject.gameObject.SetActive(false);

        isAttacking = false;
    }

    // Timer used to re-enable player movement
    void StallTimer(ref float timer)
    {
        timer -= Time.deltaTime;
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
            sword.Initialize(mySwordData, this);

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
            gun.Initialize(myGunData, this);

        myGunObject.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with enemy!");
        }
    }

    // Called for direct sources of damage. (Enemy attacks/contact and harmful terrain)
    // Will not apply during roll's invulnerable frames and grants temporary intangibility after applying
    public override IEnumerator TakeDirectDamage(float amount, string damageSource, DamageType damageType)
    {
        if (isInvulnerable)
        {
            Debug.Log("Immune");
            yield break;
        }

        healthCurrent -= amount * globalDamageMod;
        Debug.Log($"Player took {amount * globalDamageMod} direct damage.");

        //Show health loss in UIAdd commentMore actions
        //if (heartsVisualCS != null)
        //{
        //    heartsVisualCS.UpdateHearts(myHealth);
        //}

        if (healthCurrent <= 0)
        {
            // Do player death
        }
        else
        {
            playerEvent.RaisePlayerDamaged();
            hurtBox.enabled = false;
            yield return StartCoroutine(BlinkSprite());
            hurtBox.enabled = true;
        }
    }

    // Called for indirect sources of damage (Status effects)
    // Will apply regardless of rolling and does not grant intangibility after applying
    public override void TakePassiveDamage(float amount, DamageType damageType)
    {
        healthCurrent -= amount;
        Debug.Log($"Player took {amount} passive damage.");

        //Show health loss in UIAdd commentMore actions
        //if (heartsVisualCS != null)
        //{
        //    heartsVisualCS.UpdateHearts(myHealth);
        //}

        if (healthCurrent <= 0)
        {
            // Do player death
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

    // Adds 1 to AmmoCharge
    public void GainAmmoCharge()
    {
        if (ammoCount >= ammoMax)
            return;

        ammoCharge++;

        Debug.Log($"Current ammo charge is {ammoCharge} / {ammoChargeMax}.");

        if (ammoCharge >= ammoChargeMax)
        {
            ammoCharge = 0;
            ammoCount++;
            Debug.Log("Made a full ammo!");
        }

        //Shows ammo change in UI
        //if (ammoVisualCS != null)
        //{
        //    ammoVisualCS.UpdateAmmo(ammoCount);
        //}
    }

    // Runs when recieving a signal upon an enemy dying
    private void OnEnemyDeath()
    {
        

    }

    // Runs when recieving a signal upon an enemy taking damage
    private void OnEnemyHit()
    {
        GainAmmoCharge();
    }

    // Calculates the given stat with any modifier perks on the player. Sums additive modifiers, then applies them. Sums multiplicative modifiers, then applies them.
    private float CalculateStat(float baseValue, List<StatModifier> modifiers)
    {
        float result = baseValue;

        // Apply Additive Modifiers
        float additiveSum = modifiers
            .Where(mod => mod.type == ModifierType.Additive) // Finds list elements with matching type
            .Sum(mod => mod.value); // Sums any matching elements it finds
        result += additiveSum;

        // Apply Multiplicative Modifiers
        float multiplicativeSum = modifiers
            .Where(mod => mod.type == ModifierType.Multiplicative)
            .Sum(mod => mod.value);
        result *= (1 + multiplicativeSum);

        return result;
    }

    public void AddAttackModifier(StatModifier modifier)
    {
        attackMods.Add(modifier);
    }

    public void RemoveAttackModifier(StatModifier modifier)
    {
        attackMods.Remove(modifier);
    }
}
