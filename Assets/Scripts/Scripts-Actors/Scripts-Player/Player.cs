using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;

public class Player : StatEntity, IKnockable
{
    [Header("Base Stats")]
    public PlayerData myBaseStats;

    [Header("Current Stats")]
    public int ammoCount = 4;
    public int ammoCharge = 0;
    private int ammoChargeMax = 4;

    public float rollMod = 5;
    public float rollSpeed = 40f;

    public int lives = 0;

    //public float globalDamageMod = 1; // Modifier for incoming/outgoing damage

    [Header("Micro-Progression")]
    public int energyCurrentBarValue = 0; // Number of segments for the current bar. (if lvl 1 -> energyTotal) (if lvl 2 -> energyTotal - energyBarMax) (if lvl 3 -> special display)
    public int energyTotal = 0; // Total segments of energy the player CURRENTLY has. Will be <= 40
    public int energyLevel = 1; // Current level of player's energy. (energyTotal / energyBarMax) Will be 1 - 3 ([1]: energyTotal <= 19, [2]: 20 <= energyTotal <= 39, [3]: energyTotal == 40)
    public int comboCount = 0;

    [Header("Macro-Progression")]
    public int scoreCurrent = 0;
    public int scoreTotal = 0;
    public int level = 1;

    private Dictionary<StatType, List<StatModifier>> modifiers
        = new()
        {
            { StatType.Health, new List<StatModifier>() },
            { StatType.Energy, new List<StatModifier>() },
            { StatType.Attack, new List<StatModifier>() },
            { StatType.Ammo, new List<StatModifier>() },
            { StatType.Speed, new List<StatModifier>() }
        };


    public override float attack => CalculateStat(myBaseStats.basePower, modifiers[StatType.Attack]);
    public override float healthMax => CalculateStat(myBaseStats.baseMaxHealth, modifiers[StatType.Health]);
    public int energyBarMax => (int)CalculateStat(myBaseStats.baseMaxEnergy, modifiers[StatType.Energy]); // # of segments to fill single bar. Will be <= 20. Multiply by 2 to find max energy the player can hold
    public float ammoMax => CalculateStat(myBaseStats.baseMaxAmmo, modifiers[StatType.Ammo]);
    public override float moveSpeed => CalculateStat(myBaseStats.baseSpeed, modifiers[StatType.Speed]);

    [Header("Bools")]
    public bool isAttacking = false;
    public bool isRolling = false;
    public bool rollInvulnerable = false;
    public bool wantsToRoll = false;
    public bool allowInput = true;
    public override bool IsInvulnerable => damageInvulnerable || rollInvulnerable;


    [Header("Timer Starting Values")]
    private float comboStart = 5; // Length of combo timer in seconds
    private float moveDelayAttack = 0.4f; // Seconds before player can move after an attack
    private float moveDelayRoll = 0.05f; // Seconds before player can move after a roll
    private float rollCooldown = 0.5f;
    private float rollInvulWindow; // Window of invulnerability during roll

    [Header("Timers")]
    [SerializeField] private float comboTimer = 0;
    [SerializeField] private float moveTimer; // Timer that will be set to a moveDelay variable
    [SerializeField] private float rollTimer;
    [SerializeField] private float invulTimer;

    [Header("Item Data")]
    public SwordData mySwordData; // Determines and holds the data of the current sword
    private GameObject mySwordObject; // Current sword object for interactions(collision, special effects), informed by SwordData

    public GunData myGunData; // Determines and holds the data of the current gun
    private GameObject myGunObject; // Current gun object for handling gun's unique effects/bullets, informed by GunData

    private SpriteRenderer mySprite;
    public GameObject mySpriteChild;
    public CircleCollider2D hurtBox; // Component that detects collisions with damage-sources
    private Rigidbody2D rb; // Component that allows player to be stopped by walls
    private Animator myAnimator; // Component that handles animations
    private KnockHandler knockHandler;
    private Vector2 input;
    private Vector2 rollInput;

    [Header("Event Listeners")]
    public EnemyEvent enemyEvent;

    [Header("Event Emitters")]
    public PlayerEvent playerEvent;

    //[Header("UI")]
    //Variables to store HeartsVisual.cs and AmmoVisual.cs for UI
    HeartsVisual heartsVisualCS;
    AmmoVisual ammoVisualCS;

    private void Awake()
    {
        knockHandler = gameObject.GetComponent<KnockHandler>();
        knockHandler.OnKnockbackStarted += HandleKnockStart;
        knockHandler.OnKnockbackEnded += HandleKnockEnd;
    }

    // Start is called before the first frame update
    void Start()
    {
        healthCurrent = myBaseStats.baseMaxHealth;
        moveSpeed = myBaseStats.baseSpeed;
        rollSpeed = myBaseStats.BaseRollSpeed;
        rollInvulWindow = myBaseStats.baseRollInvulWindow;

        mySprite = mySpriteChild.GetComponent<SpriteRenderer>();
        myAnimator = mySprite.GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        EquipSword(Resources.Load<SwordData>("Sword Stats/Ice Sword")); // Grabs sword from filepath and instantiates its related object
        EquipGun(Resources.Load<GunData>("Gun Stats/Base Gun")); // Grabs gun from filepath and instantiates its related object

        //This is for the UI, and while .Find seems to sometimes pose problems, it should work OK if put in Start()
        GameObject heartsVisualObject = GameObject.Find("HeartsVisual");
        GameObject ammoVisualObject = GameObject.Find("AmmoVisual");

        if (heartsVisualObject != null)
        {
            heartsVisualCS = heartsVisualObject.GetComponent<HeartsVisual>(); //now we can reference HeartsVisual.cs
        }

        if (ammoVisualObject != null)
        {
            ammoVisualCS = ammoVisualObject.GetComponent<AmmoVisual>(); //now we can reference AmmoVisual.cs
        }
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
        knockHandler.OnKnockbackStarted -= HandleKnockStart;
        knockHandler.OnKnockbackEnded -= HandleKnockEnd;
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
        if (comboTimer > 0)
            CheckComboStatus();

    }

    // Occurs every 0.2 seconds (50 per second) (Independent of framerate)
    private void FixedUpdate()
    {
        if (!allowInput)
            return;

        if (wantsToRoll)
        {
            StartCoroutine(Roll());
            wantsToRoll = false;
        }

        if (!isRolling)
            MoveAndRotate();

        if (rollInvulnerable)
            InvulTimer();
    }

    private void LateUpdate()
    {
        mySpriteChild.transform.rotation = Quaternion.identity;
    }

    // Takes the player's input and calls methods/stores it
    private void ReadInput()
    {
        if (!allowInput)
        {
            input = Vector2.zero;
            return;
        }

        // Get raw input (no smoothing, instant start/stop)
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isAttacking && rollTimer <= 0)
            wantsToRoll = true;
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
            StartCoroutine(SwordAttackCoroutine());
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && ammoCount > 0)
            StartCoroutine(GunAttackCoroutine());
    }

    // Moves the player based on current inputs
    private void MoveAndRotate()
    {
        // Ends execution if standing still
        if (input == Vector2.zero)
            return;

        rollInput = input;  // Saves the last non-stationary movement vector to allow rolling from a stand-still

        // Sets direction parameters for Sprite Animator
        myAnimator.SetFloat("moveX", input.x);
        myAnimator.SetFloat("moveY", input.y);

        // Rotates towards 8 directions
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Prevents the movement step from running during an attack, allowing the player to only rotate like in GB Zelda
        if (moveTimer > 0) return;

        Vector2 moveInput = input;
        // Clamp to 8 directions only
        if (moveInput.x != 0 && moveInput.y != 0)
            moveInput *= 0.7071f; // Normalize diagonal movement (1/sqrt(2))
        //print(moveInput);
        Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    // Grants player a burst of speed while locking their movement and actions and granting a brief window of invulnerability
    IEnumerator Roll()
    {
        isRolling = true;
        rollTimer = rollCooldown;
        rollInvulnerable = true;
        mySprite.color = Color.magenta;

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
        if (invulTimer <= 0f && isRolling)
        {
            rollInvulnerable = false;
            mySprite.color = Color.white;
        }
    }

    // Positions and enables sword, calls its attack, then disables
    IEnumerator SwordAttackCoroutine()
    {
        isAttacking = true;
        moveTimer = moveDelayAttack;

        mySwordObject.transform.position = transform.position + transform.up;
        mySwordObject.transform.rotation = transform.rotation;
        mySwordObject.SetActive(true);
        mySwordObject.GetComponent<BoxCollider2D>().enabled = true;

        // Call sword effects here

        yield return new WaitForSeconds(0.2f);

        mySwordObject.SetActive(false);
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
        if (ammoVisualCS != null)
        {
            ammoVisualCS.UpdateAmmo(ammoCount);
        }

        myGunObject.transform.position = transform.position + transform.up;
        myGunObject.transform.rotation = transform.rotation;
        myGunObject.SetActive(true);

        // Call gun effects here

        yield return new WaitForSeconds(0.2f);

        myGunObject.SetActive(false);

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

        mySwordObject.SetActive(false);
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

        myGunObject.SetActive(false);
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
    public override IEnumerator TakeDirectDamage(float amount, string damageSource, DamageType damageType, Vector2 sourcePos)
    {
        if (IsInvulnerable)
            yield break;

        healthCurrent -= amount * GlobalConstants.globalDamageMod;
        Debug.Log($"Player took {amount * GlobalConstants.globalDamageMod} direct damage.");

        //Show health loss in UI
        if (heartsVisualCS != null)
        {
            heartsVisualCS.UpdateHearts(healthCurrent);
        }

        if (healthCurrent <= 0)
        {
            // Do player death
        }
        else
        {
            playerEvent.RaisePlayerDamaged();
            damageInvulnerable = true;

            StartCoroutine(knockHandler.StartKnockback(sourcePos));

            yield return StartCoroutine(BlinkSprite());
            damageInvulnerable = false;
        }
    }

    // Called for indirect sources of damage (Status effects)
    // Will apply regardless of rolling and does not grant intangibility after applying
    public override void TakePassiveDamage(float amount, DamageType damageType)
    {
        healthCurrent -= amount;
        Debug.Log($"Player took {amount} passive damage.");

        //Show health loss in UI
        if (heartsVisualCS != null)
        {
            heartsVisualCS.UpdateHearts(healthCurrent);
        }

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
        if (ammoVisualCS != null)
        {
            ammoVisualCS.UpdateAmmo(ammoCount);
        }
    }

    private void CheckComboStatus()
    {
        StallTimer(ref comboTimer);
        if (comboTimer <= 0)
            comboCount = 0;
    }

    // Determines the current Energy Level and number of segments to fill in the HUD
    private void CheckEnergy()
    {
        print((int)(energyTotal / energyBarMax) + 1);

        switch ((int)(energyTotal / energyBarMax) + 1)
        {
            case 1:
                print("Energy Level 1");
                energyLevel = 1;
                energyCurrentBarValue = energyTotal;
                break;
            case 2:
                print("Energy Level 2");
                energyLevel = 2;
                energyCurrentBarValue = energyTotal - energyBarMax;
                break;
            case 3:
                print("Energy Level 3");
                energyLevel = 3;
                energyCurrentBarValue = energyBarMax;
                break;
        }
    }

    // Runs when recieving a signal upon an enemy dying
    private void OnEnemyDeath(int scoreValue)
    {
        comboCount++;
        int combo = comboCount;

        if (energyTotal < energyBarMax * 2)
            energyTotal += combo + (combo == 0 ? 1 : 0); // If the current combo length is 0 or 1, only adds a single segment, otherwise adds the current combo length
        CheckEnergy();

        scoreCurrent += scoreValue * (combo > 0 ? combo : 1); // Ternary operator (condition ? valueIfTrue : valueIfFalse)

        comboTimer = comboStart;
    }

    // Runs when recieving a signal upon an enemy taking damage
    private void OnEnemyHit()
    {
        GainAmmoCharge();
    }

    // Calculates the given stat with any modifier perks on the player. Sums modifiers, then applies them. Divide => Add => Subtract => Multiply
    private float CalculateStat(float baseValue, List<StatModifier> modifiers)
    {
        float result = baseValue;

        // Apply Divisive Modifiers
        float divisiveSum = modifiers
            .Where(mod => mod.type == ModifierType.Divisive) // Finds list elements with matching type
            .Sum(mod => mod.value); // Sums any matching elements it finds
        result /= (divisiveSum == 0f ? 1f : divisiveSum);

        // Apply Additive Modifiers
        float additiveSum = modifiers
            .Where(mod => mod.type == ModifierType.Additive) // Finds list elements with matching type
            .Sum(mod => mod.value); // Sums any matching elements it finds
        result += additiveSum;

        // Apply Subtractive Modifiers
        float subtractiveSum = modifiers
            .Where(mod => mod.type == ModifierType.Subtractive) // Finds list elements with matching type
            .Sum(mod => mod.value); // Sums any matching elements it finds
        result -= subtractiveSum;

        // Apply Multiplicative Modifiers
        float multiplicativeSum = modifiers
            .Where(mod => mod.type == ModifierType.Multiplicative)
            .Sum(mod => mod.value);
        result *= (1 + multiplicativeSum);

        return result;
    }

    public void AddModifier(StatModifier modifier)
    {
        if (modifiers.ContainsKey(modifier.targetStat))
            modifiers[modifier.targetStat].Add(modifier);
    }

    //     NEED TO RETHINK LATER (How do you know what StatModifier to remove???)
    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.ContainsKey(modifier.targetStat))
            modifiers[modifier.targetStat].Remove(modifier);
    }

    private void HandleKnockStart()
    {
        allowInput = false;
    }

    private void HandleKnockEnd()
    {
        allowInput = true;
    }
}
