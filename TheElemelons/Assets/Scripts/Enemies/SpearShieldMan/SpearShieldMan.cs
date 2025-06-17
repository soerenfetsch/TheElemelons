using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the behavior of the SpearShieldMan, a versatile enemy that can be equipped
/// with a spear, a shield, or both.
/// </summary>
/// <remarks>
/// This enemy uses a state machine to switch between idling, chasing, and attacking the player.
/// A key feature is its "critical" state, where it will throw its spear upon taking significant damage.
/// It relies on several external components, including a LineOfSightPathfinder and combat-related prefabs.
/// </remarks>
public class SpearShieldMan : Enemy
{
    [Header("Components")]
    /// <summary>The prefab for the shield GameObject.</summary>
    public GameObject shieldPrefab;
    /// <summary>The prefab for the spear GameObject.</summary>
    public GameObject spearPrefab;
    /// <summary>The runtime instance of the spear, if equipped.</summary>
    public GameObject spearInstance;
    /// <summary>The runtime instance of the shield, if equipped.</summary>
    public GameObject shieldInstance;

    // --- Cached Component References ---
    private Transform transformComponent;
    private Rigidbody2D rb2d;
    private BoxCollider2D colliderBox;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private FlashEffect flashEffect;

    [Header("Pathfinding")]
    /// <summary>Reference to the pathfinding system used to navigate towards the player.</summary>
    public LineOfSightPathfinder pathfinder;

    [Header("Behavior")]
    /// <summary>The speed at which the enemy moves when approaching the player.</summary>
    public float moveSpeed = 1f;
    /// <summary>The radius within which the enemy will start chasing the player.</summary>
    public float chaseRadius = 10f;
    /// <summary>The radius within which the enemy will attempt to attack the player.</summary>
    public float attackRadius = 2f;

    // --- Internal State Flags ---
    /// <summary>Tracks if the enemy is currently alive. Set to false upon death to prevent further actions.</summary>
    private bool isAlive = true;
    /// <summary>Tracks if the enemy has entered its critical health phase, enabling the spear throw ability.</summary>
    private bool isCritical = false;

    [Header("Combat")]
    /// <summary>Base damage for a standard spear melee attack.</summary>
    public float baseDamageSpear = 1f;
    /// <summary>Base damage for a shield bash attack.</summary>
    public float baseDamageShield = 0.5f;
    /// <summary>Damage dealt by the spear when it is thrown in the critical health phase.</summary>
    public float throwDamageSpear = 1.5f;
    /// <summary>The speed at which the spear travels when thrown.</summary>
    public float throwSpeedSpear = 5f;
    /// <summary>The lifetime in seconds of the thrown spear projectile.</summary>
    public float throwTimeSpear = 2f;
    /// <summary>The base force applied to the player when hit.</summary>
    public float thrust = 1f;
    /// <summary>The duration in seconds the player is knocked back when hit by the spear.</summary>
    public float knockTimeSpear = 0.2f;
    /// <summary>The duration in seconds the player is knocked back when hit by the shield.</summary>
    public float knockTimeShield = 0.7f;
    /// <summary>The duration of the flash effect when the enemy is hit.</summary>
    public float flashDuration = 0.2f;
    /// <summary>The minimum time in seconds between consecutive attacks.</summary>
    public float attackCooldown = 1f;

    /// <summary>The time at which the last attack was performed, used for cooldown calculations.</summary>
    private float lastAttackTime = 0f;

    [Header("Effects")]
    /// <summary>The particle effect prefab to spawn upon death.</summary>
    public GameObject deathEffect;
    /// <summary>A delay before the death effect is spawned, allowing for death animations.</summary>
    public float deathEffectDelay = 0.5f;

    [Header("Animation")]
    /// <summary>Stores the name of the last played animation clip to prevent replaying the same animation.</summary>
    private string lastAnimationClip = null;

    /// <summary>
    /// Initializes the enemy with essential references, intended for dynamic spawning.
    /// </summary>
    /// <param name="player">The player's Transform component.</param>
    /// <param name="pathfinder">A reference to the scene's pathfinding system.</param>
    public void Initialize(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.pathfinder = pathfinder;
        this.player = playerTransform.GetComponent<Player>();
        InitializeCombatComponents();
    }

    /// <summary>
    /// Caches all necessary component references and sets up the initial state.
    /// </summary>
    public void InitializeComponents()
    {
        // --- Cache own components for performance ---
        transformComponent = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        colliderBox = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();
        flashEffect = GetComponent<FlashEffect>();

        // --- Find critical scene objects ---
        // NOTE: GameObject.Find() is slow and can be unreliable.
        // It's better to pass these references in via an initializer or assign them in the Inspector.
        playerTransform = GameObject.Find("Player").transform;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();

        // Randomly determine the enemy's equipment loadout.
        RandomizeSpearShield();

        // --- Null checks for essential components to prevent runtime errors ---
        if (deathEffect == null)
        {
            Debug.LogWarning("SpearShieldMan: No death effect prefab found on the GameObject.");
        }
        if (pathfinder == null)
        {
            Debug.LogWarning("SpearShieldMan: No pathfinding system found on the GameObject.");
        }
        if (playerTransform == null)
        {
            Debug.LogWarning("SpearShieldMan: No player transform found on the GameObject.");
        }
        if (flashEffect == null)
        {
            Debug.LogWarning("SpearShieldMan: No flash effect found on the GameObject.");
        }

        currentState = EnemyStatus.Idle;
    }

    /// <summary>
    /// Randomizes whether the SpearShieldMan has a spear, shield, or both upon initialization.
    /// </summary>
    [ContextMenu("Init SpearShield")]
    public void RandomizeSpearShield()
    {
        // 0: spear, 1: shield, 2: both
        int randomValue = UnityEngine.Random.Range(0, 3);
        switch (randomValue)
        {
            case 0:
                // Instantiate the spear prefab as a child of the SpearShieldMan
                spearInstance = Instantiate(spearPrefab, transform.position, Quaternion.identity, transform);
                break;
            case 1:
                // Instantiate the shield prefab as a child of the SpearShieldMan
                shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
                break;
            case 2:
                // Instantiate both spear and shield prefabs as children of the SpearShieldMan
                spearInstance = Instantiate(spearPrefab, transform.position, Quaternion.identity, transform);
                shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
                break;
        }
    }

    /// <summary>
    /// Sets up combat-related properties and subscribes to attack events from weapons.
    /// </summary>
    private void InitializeCombatComponents()
    {
        attackFactor = 1f;
        defenseFactor = 1f;
        elementType = ElementType.Earth;

        // Subscribe to the damage-dealing events from the instantiated weapon(s).
        if (spearInstance != null)
        {
            spearInstance.GetComponentInChildren<ShieldSpearCollider>().OnAttackTriggered += DealDamageWithSpear;
            spearInstance.GetComponent<Spear>().OnAttackTriggered += DealDamageWithSpearThrow;
            spearInstance.GetComponent<Spear>().enabled = false; // Ensure the spear throw logic is disabled initially.
        }
        if (shieldInstance != null)
        {
            shieldInstance.GetComponentInChildren<ShieldSpearCollider>().OnAttackTriggered += DealDamageWithShield;
        }
    }

    /// <summary>
    /// Initializes all components and references when the object enters the scene.
    /// </summary>
    void Start()
    {
        InitializeComponents();
        InitializeCombatComponents();
        playerTransform = GameObject.Find("Player").transform;
        player = playerTransform.GetComponent<Player>();
        if (playerTransform == null)
        {
            Debug.Log("Tiki couldn't find the player at Start(), please fix");
        }
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
    }

    /// <summary>
    /// Handles physics-based updates, including movement and state checks.
    /// </summary>
    private void FixedUpdate()
    {
        if (CanAct())
        {
            ActOnStatus();
        }
        DoMovementWithChecks();
        ActOnHealth();
        ActOnMana();
    }

    /// <summary>
    /// Changes the enemy's current state, preventing redundant state changes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    /// <summary>
    /// Main state machine logic that dictates the enemy's behavior based on its current state.
    /// </summary>
    /// <remarks>
    /// Handles transitions between Idle, Approaching, Attacking, and other states based on player distance and reachability.
    /// </remarks>
    public override void ActOnStatus()
    {
        switch (currentState)
        {
            case EnemyStatus.Idle:
                // If the player is in range and reachable, start approaching.
                if (CheckPlayerWithinRadius(chaseRadius) && CheckPlayerReachableWithinRadius(chaseRadius))
                {
                    movement = Vector2.zero;
                    ChangeState(EnemyStatus.Approaching);
                    ChangeAnimationClip("Walking");
                    break;
                }
                break;
            case EnemyStatus.Approaching:
                // If in attack range, switch to attacking.
                if (CheckPlayerInAttackRange())
                {
                    ChangeState(EnemyStatus.Attacking);
                    ChangeAnimationClip("Idle");
                    break;
                }
                // If the player becomes unreachable, go back to idle.
                else if (CheckPlayerUnreachableWithinRadius(chaseRadius))
                {
                    movement = Vector2.zero;
                    ChangeState(EnemyStatus.Idle);
                    ChangeAnimationClip("Idle");
                    break;
                }
                MoveTowardPlayer();
                break;
            case EnemyStatus.Attacking:
                // Perform attacks if cooldown is ready.
                AttackWithRandomWeaponIfReady();
                // If player moves out of attack range, go back to approaching.
                if (!CheckPlayerInAttackRange())
                {
                    ChangeState(EnemyStatus.Approaching);
                    ChangeAnimationClip("Walking");
                }
                break;
            case EnemyStatus.Retreating:
                // Behavior not yet implemented.
                break;
            case EnemyStatus.Staggered:
                // In this state, the enemy cannot act. Actions are managed by the HandleStagger coroutine.
                break;
        }
    }

    /// <summary>
    /// Initiates an attack with a random available weapon if the attack cooldown has elapsed.
    /// </summary>
    private void AttackWithRandomWeaponIfReady()
    {
        if (lastAttackTime + attackCooldown < Time.time)
        {
            lastAttackTime = Time.time;
            if (spearInstance != null && shieldInstance != null)
            {
                // Randomly choose between spear and shield attack
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    StartCoroutine(AttackWithSpear());
                }
                else
                {
                    StartCoroutine(AttackWithShield());
                }
            }
            else if (spearInstance != null)
            {
                StartCoroutine(AttackWithSpear());
            }
            else if (shieldInstance != null)
            {
                StartCoroutine(AttackWithShield());
            }
            else
            {
                Debug.LogWarning("SpearShieldMan: No spear or shield instance found for attack.");
            }
        }
    }

    /// <summary>
    /// Coroutine for performing a melee attack with the spear.
    /// </summary>
    /// <remarks>
    /// Plays the attack animation and enables the weapon's collider for a fixed duration.
    /// </remarks>
    public IEnumerator AttackWithSpear()
    {
        ChangeAnimationClip("Attacking");
        spearInstance.GetComponent<Animator>()?.SetTrigger("Attack");
        spearInstance.GetComponentInChildren<ShieldSpearCollider>()?.EnableAttack();
        // This delay should ideally match the length of the spear attack animation.
        yield return new WaitForSeconds(1.2f);
        spearInstance.GetComponentInChildren<ShieldSpearCollider>()?.DisableAttack();
        if (GetCurrentState() == EnemyStatus.Attacking)
        {
            ChangeAnimationClip("Idle");
        }
    }

    /// <summary>
    /// Coroutine for performing a bash attack with the shield.
    /// </summary>
    /// <remarks>
    /// Plays the attack animation and enables the weapon's collider for a fixed duration.
    /// </remarks>
    public IEnumerator AttackWithShield()
    {
        ChangeAnimationClip("Attacking");
        shieldInstance.GetComponent<Animator>()?.SetTrigger("Attack");
        shieldInstance.GetComponentInChildren<ShieldSpearCollider>()?.EnableAttack();
        // This delay should ideally match the length of the shield attack animation.
        yield return new WaitForSeconds(1.2f);
        shieldInstance.GetComponentInChildren<ShieldSpearCollider>()?.DisableAttack();
        if (GetCurrentState() == EnemyStatus.Attacking)
        {
            ChangeAnimationClip("Idle");
        }
    }

    /// <summary>
    /// Event handler that executes a spear melee attack via the CombatManager.
    /// </summary>
    private void DealDamageWithSpear(Rigidbody2D playerRb)
    {
        if (playerRb == null) return;

        Vector2 attackDirection = (playerRb.transform.position - transform.position).normalized;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, baseDamageSpear, thrust, knockTimeSpear);
    }

    /// <summary>
    /// Event handler that executes a shield bash attack via the CombatManager.
    /// </summary>
    private void DealDamageWithShield(Rigidbody2D playerRb)
    {
        if (playerRb == null) return;

        Vector2 attackDirection = (playerRb.transform.position - transform.position).normalized;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, baseDamageShield, thrust, knockTimeShield);
    }

    /// <summary>
    /// Event handler that executes a thrown spear attack via the CombatManager.
    /// </summary>
    private void DealDamageWithSpearThrow(Rigidbody2D playerRb, Vector2 attackDirection)
    {
        if (playerRb == null) return;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, throwDamageSpear, thrust, knockTimeSpear);
    }

    /// <summary>
    /// Changes the current animation clip, preventing redundant calls to Animator.Play.
    /// </summary>
    /// <param name="animationName">The name of the animation clip to play.</param>
    private void ChangeAnimationClip(string animationName)
    {
        if (lastAnimationClip != animationName)
        {
            animator.Play(animationName);
            lastAnimationClip = animationName;
        }
    }

    /// <summary>
    /// Manages behavior based on the enemy's health, including the critical spear throw and death.
    /// </summary>
    public override void ActOnHealth()
    {
        // --- Critical Health Check ---
        // If health drops below 25% and we have a spear, enter the 'critical' state.
        if (health.GetHealth() <= health.GetMaxHealth() * 0.25f && isAlive && !isCritical && spearInstance != null)
        {
            // The 'isCritical' flag ensures this block only runs once.
            isCritical = true;
            // Enable the Spear component on the spear instance, allowing it to be thrown.
            spearInstance.GetComponent<Spear>().enabled = true;
        }

        // --- Death Check ---
        if (health.IsZero() && isAlive)
        {
            // The 'isAlive' flag ensures this death logic only runs once.
            isAlive = false;
            DeathEffect.SpawnDeathEffect(deathEffect, transformComponent.position, deathEffectDelay);
            Destroy(gameObject, 0);
        }

        // --- Spear Throw Action ---
        // If in the critical state with a spear, and not staggered, perform the spear throw.
        if (isCritical && spearInstance != null && currentState != EnemyStatus.Staggered)
        {
            Debug.Log("SpearShieldMan: Throwing spear at player.");
            // Prepare the spear instance for launch by detaching it from the parent's animation.
            spearInstance.GetComponent<Animator>().enabled = false;
            spearInstance.GetComponentInChildren<ShieldSpearCollider>().DisableAttack();
            spearInstance.GetComponentInChildren<ShieldSpearCollider>().enabled = false;
            // Calculate direction and throw.
            Vector2 attackDirection = (playerTransform.position - transform.position).normalized;
            spearInstance.GetComponent<Spear>()?.ThrowSpear(
                attackDirection, throwSpeedSpear, throwTimeSpear);
            // The spear has been thrown, so we no longer have a reference to it.
            spearInstance = null;
        }
    }

    /// <summary>
    /// Handles behavior based on mana. This enemy does not use mana.
    /// </summary>
    public override void ActOnMana()
    {
        // Do nothing
    }

    /// <summary>
    /// Puts the enemy into a staggered state when hit.
    /// </summary>
    /// <param name="knockTime">The duration of the stagger.</param>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }

    /// <summary>
    /// Manages sprite sorting order and animation parameters for 8-directional movement.
    /// </summary>
    /// <param name="direction">The current movement direction.</param>
    private void ChangeWalkAnimation(Vector2 direction)
    {
        int ownSortingOrder = spriteRenderer.sortingOrder;
        // Check if horizontal movement is dominant.
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                SetAnimationFloat("Vertical", 0f);
                SetAnimationFloat("Horizontal", 1f);
                // Facing right: Adjust sorting order for correct layering.
                if (shieldInstance != null)
                    shieldInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder + 1;
                if (spearInstance != null)
                    spearInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder;
            }
            else if (direction.x < 0)
            {
                SetAnimationFloat("Vertical", 0f);
                SetAnimationFloat("Horizontal", -1f);
                // Facing left: Adjust sorting order for correct layering.
                if (shieldInstance != null)
                    shieldInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder;
                if (spearInstance != null)
                    spearInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder + 1;
            }
        }
        // Check if vertical movement is dominant.
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimationFloat("Vertical", 1f);
                SetAnimationFloat("Horizontal", 0f);
                // Facing up/back: The main body should be in front of the weapons.
                if (shieldInstance != null)
                    shieldInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder - 1;
                if (spearInstance != null)
                    spearInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder - 1;
            }
            else if (direction.y < 0)
            {
                SetAnimationFloat("Vertical", -1f);
                SetAnimationFloat("Horizontal", 0f);
                // Facing down/front: The weapons should be in front of the main body.
                if (shieldInstance != null)
                    shieldInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder + 1;
                if (spearInstance != null)
                    spearInstance.GetComponent<SpriteRenderer>().sortingOrder = ownSortingOrder + 1;
            }
        }
    }

    /// <summary>
    /// A helper method to set animation float parameters on the main body and any equipped weapons.
    /// </summary>
    /// <param name="parameter">The name of the animator float parameter.</param>
    /// <param name="value">The value to set.</param>
    private void SetAnimationFloat(string parameter, float value)
    {
        animator.SetFloat(parameter, value);
        if (spearInstance != null)
        {
            spearInstance.GetComponent<Animator>().SetFloat(parameter, value);
        }
        if (shieldInstance != null)
        {
            shieldInstance.GetComponent<Animator>().SetFloat(parameter, value);
        }
    }

    /// <summary>
    /// Checks if the player is within a given radius based on simple distance.
    /// </summary>
    /// <param name="radius">The radius to check against.</param>
    /// <returns>True if the player is within the radius.</returns>
    private bool CheckPlayerWithinRadius(float radius)
    {
        return Vector2.Distance(GetFeetPosition(), playerTransform.position) <= radius;
    }

    /// <summary>
    /// Checks if the player is within the defined attack range and cone.
    /// </summary>
    /// <returns>True if the player is in a valid position to be attacked.</returns>
    private bool CheckPlayerInAttackRange()
    {
        Vector2 distanceVector = (Vector2)playerTransform.position - GetFeetPosition();
        // Get the current facing direction from the animator.
        Vector2 attackDirection = new Vector2(animator.GetFloat("Horizontal"),
                                              animator.GetFloat("Vertical"));
        // Use dot product to check if the player is in front of the enemy.
        float dotProductDistance = Vector2.Dot(distanceVector.normalized, attackDirection.normalized);
        float regularDistance = Vector2.Distance(GetFeetPosition(), playerTransform.position);
        // The player is in range if they are very close, OR if they are within the attack radius AND in front of the enemy.
        return (regularDistance <= attackRadius && dotProductDistance >= 0.8f) || (regularDistance <= 1);
    }

    /// <summary>
    /// Checks if a path to the player exists within a given radius using the pathfinding system.
    /// </summary>
    /// <param name="radius">The maximum distance to check for a path.</param>
    /// <returns>True if the player is reachable.</returns>
    private bool CheckPlayerReachableWithinRadius(float radius)
    {
        bool reachable = pathfinder.CheckPlayerInSight(GetFeetPosition(), radius) != Vector2.zero
                       || pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) != Vector2.zero;
        return reachable;
    }

    /// <summary>
    /// Checks if a path to the player does NOT exist within a given radius.
    /// </summary>
    /// <param name="radius">The radius to check against.</param>
    /// <returns>True if the player is unreachable.</returns>
    private bool CheckPlayerUnreachableWithinRadius(float radius)
    {
        return pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) == Vector2.zero;
    }

    /// <summary>
    /// Calculates the movement vector towards the player using the pathfinding system.
    /// </summary>
    private void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        // Prioritize a direct line of sight, otherwise use the A* path.
        Vector2 direction = pathfinder.CheckPlayerInSight(GetFeetPosition(), chaseRadius);
        if (direction == Vector2.zero)
        {
            direction = pathfinder.CheckPlayerReachable(GetFeetPosition(), chaseRadius);
        }
        if (direction == Vector2.zero) return;

        direction = direction.normalized;
        movement = direction;
        ChangeWalkAnimation(direction);
    }
    
    /// <summary>
    /// Applies the calculated movement vector to the Rigidbody's position.
    /// </summary>
    public override void DoMovement()
    {
        rb2d.MovePosition((Vector2)transformComponent.position + movement * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Freezes or unfreezes the enemy's movement and animations.
    /// </summary>
    public override void OnFreeze(bool freezeOn = true)
    {
        if (freezeOn)
        {
            movement = Vector2.zero;
            animator.speed = 0;
        }
        else
        {
            animator.speed = 1;
        }
    }

    /// <summary>
    /// Gets the position at the base of the enemy's collider, used for pathfinding.
    /// </summary>
    /// <returns>The world position of the enemy's feet.</returns>
    private Vector2 GetFeetPosition()
    {
        return new Vector2(transformComponent.position.x, colliderBox.bounds.min.y);
    }

    /// <summary>
    /// Coroutine to handle the stagger effect, temporarily interrupting the enemy's actions.
    /// </summary>
    /// <param name="staggerTime">The duration of the stagger effect.</param>
    private IEnumerator HandleStagger(float staggerTime)
    {
        EnemyStatus previousState = currentState;
        ChangeState(EnemyStatus.Staggered);
        string lastAnimation = lastAnimationClip;

        ChangeAnimationClip("Stagger");
        flashEffect?.Flash(flashDuration);
        if (spearInstance != null)
            spearInstance?.GetComponent<FlashEffect>()?.Flash(flashDuration);
        if (shieldInstance != null)
            shieldInstance?.GetComponent<FlashEffect>()?.Flash(flashDuration);

        yield return new WaitForSeconds(staggerTime);
        ChangeState(previousState);
        ChangeAnimationClip(lastAnimation);
    }

    /// <summary>
    /// Modifies the enemy's movement speed by a given factor.
    /// </summary>
    public override void ModifySpeed(float factor)
    {
        moveSpeed *= factor;
    }

    /// <summary>
    /// Modifies the enemy's attack damage by a given factor.
    /// </summary>
    public override void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    /// <summary>
    /// Modifies the enemy's defense by a given factor.
    /// </summary>
    public override void ModifyDefense(float factor)
    {
        defenseFactor *= factor;
    }

    /// <summary>
    /// Modifies the enemy's mana regeneration. Not used by this enemy.
    /// </summary>
    public override void ModifyManaRegen(float factor)
    {
        // Do nothing
    }

    /// <summary>
    /// Toggles a sight-obstructing collider.
    /// </summary>
    public override void DoObstructSight(bool onOff)
    {
        ObstructSight(onOff);
    }
}