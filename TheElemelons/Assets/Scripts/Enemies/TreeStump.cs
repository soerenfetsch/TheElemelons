using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the TreeLog enemy's behavior, including patrolling, chasing, attacking, retreating, and handling state transitions.
/// Integrates pathfinding, attack logic, and animation state changes.
/// </summary>
public class TreeLog : Enemy
{
    [Header("Components")]
    // Reference to the transform component.
    private Transform transformComponent;
    // Rigidbody2D for movement.
    private Rigidbody2D rb2d;
    // Collider for the TreeLog.
    private BoxCollider2D colliderBox;
    // AttackCollider for handling attack logic.
    private AttackCollider attackCollider;
    // Animator for controlling animations.
    private Animator animator;
    // FlashEffect for visual feedback.
    private FlashEffect flashEffect;

    [Header("Pathfinding")]
    /// <summary>Reference to the pathfinding system.</summary>
    public LineOfSightPathfinder pathfinder;

    [Header("Behavior")]
    // Home position for retreating.
    private Vector2 homePosition;
    // Whether the TreeLog is alive.
    private bool isAlive = true;
    /// <summary>Movement speed of the TreeLog.</summary>
    public float moveSpeed = 1f;
    /// <summary>Radius within which the TreeLog will chase the player.</summary>
    public float chaseRadius = 5f;
    /// <summary>Radius within which the TreeLog will attack the player.</summary>
    public float attackRadius = 0.2f;

    [Header("Combat")]
    /// <summary>Base damage dealt by the TreeLog.</summary>
    public float baseDamage = 1f;
    /// <summary>Knockback force applied by the TreeLog.</summary>
    public float thrust = 1f;
    /// <summary>Duration of knockback.</summary>
    public float knockTime = 0.2f;
    /// <summary>Duration of the flash effect.</summary>
    public float flashDuration = 0.2f;

    [Header("Effects")]
    /// <summary>Prefab for the death effect.</summary>
    public GameObject deathEffect;
    /// <summary>Delay before the death effect is played.</summary>
    public float deathEffectDelay = 0.5f;

    /// <summary>
    /// Initializes the TreeLog with references to the player and pathfinder.
    /// </summary>
    public void Initialize(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.pathfinder = pathfinder;
        movement = Vector2.zero;
        this.player = playerTransform.GetComponent<Player>();
        InitializeCombatComponents();
    }

    /// <summary>
    /// Initializes component references and sets up the TreeLog's state.
    /// </summary>
    public void InitializeComponents()
    {
        transformComponent = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        colliderBox = GetComponent<BoxCollider2D>();
        attackCollider = GetComponentInChildren<AttackCollider>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();
        flashEffect = GetComponent<FlashEffect>();
        playerTransform = GameObject.Find("Player").transform;
        player = playerTransform.GetComponent<Player>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        currentState = EnemyStatus.Idle;
        homePosition = transformComponent.position;
    }

    /// <summary>
    /// Initializes combat-related factors and subscribes to attack events.
    /// </summary>
    private void InitializeCombatComponents()
    {
        attackFactor = 1f;
        defenseFactor = 1f;
        isInvincible = false;
        elementType = ElementType.Earth;

        if (attackCollider != null)
        {
            attackCollider.OnAttackTriggered += HandleAttack;
        }
        else
        {
            Debug.LogWarning("TreeStump: No AttackCollider found on the GameObject.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeCombatComponents();
    }

    // Fixed update to handle physics and movements correctly
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

    void Update()
    {
        ChangeWalkAnimation();
    }

    /// <summary>
    /// Changes the current state of the TreeLog.
    /// </summary>
    public override void ChangeState(EnemyStatus newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// Acts on the current status of the enemy.
    /// Handles Idle, Approaching, Attacking, Retreating, and Staggered states.
    /// </summary>
    public override void ActOnStatus()
    {
        switch (currentState)
        {
            case EnemyStatus.Idle:
                // Wait until player is within chase or attack radius
                if (CheckPlayerWithinRadius(chaseRadius))
                {
                    movement = Vector2.zero;
                    ChangeState(EnemyStatus.Approaching);
                    animator.SetBool("wakeUp", true);
                }
                break;
            case EnemyStatus.Approaching:
                MoveTowardPlayer();
                if (CheckPlayerWithinRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Attacking);
                }
                else if (CheckPlayerUnreachableWithinRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Retreating);
                }
                break;
            case EnemyStatus.Attacking:
                // Attempt to attack player by allowing the attack collider to hit the player
                EnableAttackCollider();
                MoveTowardPlayer();
                if (CheckPlayerOutsideRadius(attackRadius))
                {
                    DisableAttackCollider();
                    ChangeState(EnemyStatus.Approaching);
                }
                break;
            case EnemyStatus.Retreating:
                MoveTowardHome();
                if (CheckPlayerReachableWithinRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Approaching);
                }
                else if (CheckAtHome())
                {
                    movement = Vector2.zero;
                    ChangeState(EnemyStatus.Idle);
                    animator.SetBool("wakeUp", false);
                }
                break;
            case EnemyStatus.Staggered:
                break;
        }
    }

    /// <summary>
    /// Moves the TreeLog based on its current movement vector and speed.
    /// </summary>
    public override void DoMovement()
    {
        movement = movement.normalized;
        rb2d.MovePosition((Vector2)transformComponent.position + movement * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Freezes or unfreezes the TreeLog, affecting movement and animation speed.
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
    /// Acts on the current health status of the enemy.
    /// Checks if the enemy's health is zero and if the enemy is still alive. If both conditions are met,
    /// it sets the enemy's alive status to false, spawns a death effect at the enemy's position, and destroys the enemy game object.
    /// </summary>
    public override void ActOnHealth()
    {
        if (health.IsZero() && isAlive)
        {
            isAlive = false;
            DeathEffect.SpawnDeathEffect(deathEffect, transformComponent.position, deathEffectDelay);
            Destroy(gameObject, 0);
        }
    }

    /// <summary>
    /// Acts on the current mana status of the enemy.
    /// There is no mana behavior for the TreeLog enemy, so this method does nothing.
    /// </summary>
    public override void ActOnMana()
    {
        // Do nothing
    }

    /// <summary>
    /// When the TreeStump gets knocked back, it enters the staggered state for a certain amount of time.
    /// </summary>
    /// <param name="knockTime">Duration of the stagger.</param>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }

    // Changes the walk animation based on movement direction.
    private void ChangeWalkAnimation()
    {
        Vector2 direction = movement;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                animator.SetFloat("Vertical", 0f);
                animator.SetFloat("Horizontal", 1f);
            }
            else if (direction.x < 0)
            {
                animator.SetFloat("Vertical", 0f);
                animator.SetFloat("Horizontal", -1f);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                animator.SetFloat("Vertical", 1f);
                animator.SetFloat("Horizontal", 0f);
            }
            else if (direction.y < 0)
            {
                animator.SetFloat("Vertical", -1f);
                animator.SetFloat("Horizontal", 0f);
            }
        }
    }

    /// <summary>
    /// Check if the player is within a certain radius of the enemy (just positions).
    /// </summary>
    /// <param name="radius">Radius to check.</param>
    /// <returns>True if player is within radius, false otherwise.</returns>
    private bool CheckPlayerWithinRadius(float radius)
    {
        return Vector2.Distance(GetFeetPosition(), playerTransform.position) <= radius;
    }

    /// <summary>
    /// Check if the player is outside a certain radius of the enemy (just positions).
    /// </summary>
    /// <param name="radius">Radius to check.</param>
    /// <returns>True if player is outside radius, false otherwise.</returns>
    private bool CheckPlayerOutsideRadius(float radius)
    {
        return Vector2.Distance(GetFeetPosition(), playerTransform.position) > radius;
    }

    /// <summary>
    /// Check if the player is within a certain reach radius of the enemy (using pathfinding).
    /// Returns true if player is in sight or reachable within the radius.
    /// </summary>
    /// <param name="radius">Radius to check.</param>
    /// <returns>True if player is reachable, false otherwise.</returns>
    private bool CheckPlayerReachableWithinRadius(float radius)
    {
        bool reachable = pathfinder.CheckPlayerInSight(GetFeetPosition(), radius) != Vector2.zero
                      || pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) != Vector2.zero;
        return reachable;
    }

    /// <summary>
    /// Check if the player is outside a certain reach radius of the enemy (using pathfinding).
    /// Returns true if player is not reachable within the radius.
    /// </summary>
    /// <param name="radius">Radius to check.</param>
    /// <returns>True if player is unreachable, false otherwise.</returns>
    private bool CheckPlayerUnreachableWithinRadius(float radius)
    {
        return pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) == Vector2.zero;
    }

    // Moves the TreeLog toward the player using pathfinding.
    private void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        Vector2 direction = pathfinder.CheckPlayerInSight(GetFeetPosition(), chaseRadius);
        if (direction == Vector2.zero)
        {
            direction = pathfinder.CheckPlayerReachable(GetFeetPosition(), chaseRadius);
        }
        if (direction == Vector2.zero) return;

        direction.Normalize();
        movement = direction;
    }

    // Moves the TreeLog toward its home position using pathfinding.
    private void MoveTowardHome()
    {
        Vector2 direction = RaycastPathfinder.FindBestDirection(GetFeetPosition(), homePosition);
        movement = direction;
    }

    // Checks if the TreeLog is at its home position.
    private bool CheckAtHome()
    {
        return Vector2.Distance(GetFeetPosition(), homePosition) < 0.2f;
    }

    // Gets the position of the TreeLog's feet (bottom of collider).
    private Vector2 GetFeetPosition()
    {
        return new Vector2(transformComponent.position.x, colliderBox.bounds.min.y);
    }

    // Handles the stagger state and animation for the TreeLog.
    private IEnumerator HandleStagger(float staggerTime)
    {
        ChangeState(EnemyStatus.Staggered);
        animator.SetTrigger("TrStagger");
        flashEffect?.Flash(flashDuration);
        yield return new WaitForSeconds(staggerTime);
        animator.SetTrigger("TrStopStagger");
        ChangeState(EnemyStatus.Idle);
    }

    // Handles the attack event when the player is hit.
    private void HandleAttack(Rigidbody2D playerRb)
    {
        if (playerRb == null || isMovementBlocked) return;

        Vector2 attackDirection = (playerRb.transform.position - transform.position).normalized;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, baseDamage, thrust, knockTime);
    }

    // Enables the attack collider.
    private void EnableAttackCollider()
    {
        attackCollider.EnableAttack();
    }

    // Disables the attack collider.
    private void DisableAttackCollider()
    {
        attackCollider.DisableAttack();
    }

    /// <summary>
    /// Modifies the TreeLog's movement speed by a factor.
    /// </summary>
    public override void ModifySpeed(float factor)
    {
        moveSpeed *= factor;
    }

    /// <summary>
    /// Modifies the TreeLog's damage by a factor.
    /// </summary>
    public override void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    /// <summary>
    /// Modifies the TreeLog's defense by a factor.
    /// </summary>
    public override void ModifyDefense(float factor)
    {
        defenseFactor *= factor;
    }

    /// <summary>
    /// Does nothing for mana regen.
    /// </summary>
    public override void ModifyManaRegen(float factor)
    {
        // Do nothing
    }

    /// <summary>
    /// Enables or disables sight obstruction for the TreeLog.
    /// </summary>
    public override void DoObstructSight(bool onOff)
    {
        ObstructSight(onOff);
    }
}
