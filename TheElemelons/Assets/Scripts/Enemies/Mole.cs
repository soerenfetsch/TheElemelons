using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the Mole enemy's behavior, including digging, attacking, movement, and state transitions.
/// Handles pathfinding, ripple attacks, invincibility while underground, and animation state changes.
/// </summary>
public class Mole : Enemy
{
    [Header("Components")]
    /// <summary>Reference to the Mole's arm GameObject.</summary>
    public GameObject arm;
    /// <summary>Prefab for the ripple attack effect.</summary>
    public GameObject rippleEffectPrefab;

    // Cached component references
    private Transform transformComponent;
    private Rigidbody2D rb2d;
    private BoxCollider2D colliderBox;
    private Animator animator;
    private FlashEffect flashEffect;
    private FlashEffect armFlashEffect;
    Animator armAnimators;

    [Header("Pathfinding")]
    /// <summary>Reference to the pathfinding system.</summary>
    public LineOfSightPathfinder pathfinder;

    [Header("Behavior")]
    /// <summary>Movement speed of the Mole.</summary>
    public float moveSpeed = 1f;
    /// <summary>Radius within which the Mole will chase the player.</summary>
    public float chaseRadius = 10f;
    /// <summary>Radius within which the Mole will attack the player.</summary>
    public float attackRadius = 2f;
    /// <summary>Interval between digging out of the ground.</summary>
    public float digOutInterval = 15f;

    // State flags
    private bool isDugIn = true;
    private bool canDigOut = true;
    private bool isAlive = true;
    private bool isDiggingUp = false;

    [Header("Combat")]
    /// <summary>Base damage dealt by the Mole.</summary>
    public float baseDamage = 1f;
    /// <summary>Knockback force applied by the Mole.</summary>
    public float thrust = 1f;
    /// <summary>Duration of knockback.</summary>
    public float knockTime = 0.2f;
    /// <summary>Cooldown between ripple attacks.</summary>
    public float rippleCooldown = 2f;
    /// <summary>Duration of the flash effect.</summary>
    public float flashDuration = 0.2f;

    // Time of the last ripple attack
    private float lastRippleTime = 0f;

    [Header("Effects")]
    /// <summary>Prefab for the death effect.</summary>
    public GameObject deathEffect;
    /// <summary>Delay before the death effect is played.</summary>
    public float deathEffectDelay = 0.5f;

    [Header("Animation")]
    // Name of the last animation clip played.
    private string lastAnimationClip = null;


    /// <summary>
    /// Initializes the Mole with references to the player and pathfinder.
    /// </summary>
    public void Initialize(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.player = playerTransform.GetComponent<Player>();
        this.pathfinder = pathfinder;

        InitializeCombatComponents();
    }

    /// <summary>
    /// Initializes component references and sets up the Mole's state.
    /// </summary>
    public void InitializeComponents()
    {
        transformComponent = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        colliderBox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        armAnimators = arm.GetComponent<Animator>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();
        flashEffect = GetComponent<FlashEffect>();
        armFlashEffect = arm.GetComponent<FlashEffect>();
        playerTransform = GameObject.Find("Player").transform;
        player = playerTransform.GetComponent<Player>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();

        if (arm == null)
        {
            Debug.LogWarning("Mole: No arm found on the GameObject.");
        }
        else
        {
            arm.SetActive(false); // Disable the arm by default
        }
        if (rippleEffectPrefab == null)
        {
            Debug.LogWarning("Mole: No ripple effect prefab found on the GameObject.");
        }
        if (deathEffect == null)
        {
            Debug.LogWarning("Mole: No death effect prefab found on the GameObject.");
        }
        if (pathfinder == null)
        {
            Debug.LogWarning("Mole: No pathfinding system found on the GameObject.");
        }
        if (playerTransform == null)
        {
            Debug.LogWarning("Mole: No player transform found on the GameObject.");
        }
        if (flashEffect == null)
        {
            Debug.LogWarning("Mole: No flash effect found on the GameObject.");
        }
        if (armFlashEffect == null)
        {
            Debug.LogWarning("Mole: No arm flash effect found on the GameObject.");
        }

        currentState = EnemyStatus.Idle;
    }

    /// <summary>
    /// Initializes combat-related factors and sets the Mole as invincible.
    /// </summary>
    private void InitializeCombatComponents()
    {
        attackFactor = 1f;
        defenseFactor = 1f;
        elementType = ElementType.Earth;

        MakeInvincible(); // Start as invincible
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

    /// <summary>
    /// Changes the Mole's state to the specified new state.
    /// </summary>
    public override void ChangeState(EnemyStatus newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    /// <summary>
    /// Acts on the current status of the enemy.
    /// Handles behavior based on state: idle (dug in), approaching, attacking, retreating, or staggered.
    /// </summary>
    public override void ActOnStatus()
    {
        switch (currentState)
        {
            case EnemyStatus.Idle:
                if (isDugIn && CheckPlayerWithinRadius(chaseRadius))
                {
                    movement = Vector2.zero;
                    ChangeState(EnemyStatus.Approaching);
                    ChangeAnimationClip("Walking");
                    break;
                }
                if (canDigOut)
                {
                    canDigOut = false;
                    StartCoroutine(PopOutandDigIn());
                }
                break;
            case EnemyStatus.Approaching:
                MoveTowardPlayer();
                if (CheckPlayerWithinRadius(attackRadius))
                {
                    DigUp();
                }
                else if (CheckPlayerUnreachableWithinRadius(chaseRadius) && !isDiggingUp)
                {
                    movement = Vector2.zero;
                    ChangeState(EnemyStatus.Idle);
                    ChangeAnimationClip("Idle");
                }
                break;
            case EnemyStatus.Attacking:
                TryAttackWithRipples();
                if (CheckPlayerOutsideRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Retreating);
                    DigDown();
                }
                break;
            case EnemyStatus.Retreating:
                break;
            case EnemyStatus.Staggered:
                break;
        }
    }

    /// <summary>
    /// Changes the animation clip if it is different from the last played.
    /// </summary>
    private void ChangeAnimationClip(string v)
    {
        if (lastAnimationClip != v)
        {
            animator.Play(v);
            lastAnimationClip = v;
        }
    }

    /// <summary>
    /// Coroutine to pop out and dig in after a delay.
    /// </summary>
    private IEnumerator PopOutandDigIn()
    {
        DigUp();
        yield return new WaitForSeconds(digOutInterval);
        canDigOut = true;
    }

    /// <summary>
    /// Checks if the Mole's health is zero and handles death.
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
    /// No mana behavior for the Mole enemy.
    /// </summary>
    public override void ActOnMana()
    {
        // Do nothing
    }

    /// <summary>
    /// Handles knockback and stagger state for the Mole.
    /// </summary>
    /// <param name="knockTime">Duration of the stagger.</param>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }

    /// <summary>
    /// Changes the walk animation based on movement direction.
    /// </summary>
    private void ChangeWalkAnimation(Vector2 direction)
    {
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
    /// Checks if the player is within a certain radius of the enemy (position only).
    /// </summary>
    private bool CheckPlayerWithinRadius(float radius)
    {
        float distance = Vector2.Distance(GetFeetPosition(), playerTransform.position);
        return distance <= radius;
    }

    /// <summary>
    /// Checks if the player is outside a certain radius of the enemy (position only).
    /// </summary>
    private bool CheckPlayerOutsideRadius(float radius)
    {
        return Vector2.Distance(GetFeetPosition(), playerTransform.position) > radius;
    }

    /// <summary>
    /// Checks if the player is reachable within a certain radius using pathfinding.
    /// </summary>
    private bool CheckPlayerReachableWithinRadius(float radius)
    {
        bool reachable = pathfinder.CheckPlayerInSight(GetFeetPosition(), radius) != Vector2.zero
                      || pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) != Vector2.zero;
        return reachable;
    }

    /// <summary>
    /// Checks if the player is unreachable within a certain radius using pathfinding.
    /// </summary>
    private bool CheckPlayerUnreachableWithinRadius(float radius)
    {
        return pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) == Vector2.zero;
    }

    /// <summary>
    /// Moves the Mole toward the player using pathfinding.
    /// </summary>
    private void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

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
    /// Gets the position of the Mole's feet (bottom of collider).
    /// </summary>
    private Vector2 GetFeetPosition()
    {
        return new Vector2(transformComponent.position.x, colliderBox.bounds.min.y);
    }

    /// <summary>
    /// Handles the stagger state and animation for the Mole.
    /// </summary>
    private IEnumerator HandleStagger(float staggerTime)
    {
        EnemyStatus previousState = currentState;
        ChangeState(EnemyStatus.Staggered);
        string lastAnimation = lastAnimationClip;

        ChangeAnimationClip("Stagger");
        flashEffect?.Flash(flashDuration);
        if (arm.activeSelf)
            armFlashEffect?.Flash(flashDuration);

        yield return new WaitForSeconds(staggerTime);
        ChangeState(previousState);
        ChangeAnimationClip(lastAnimation);
    }

    /// <summary>
    /// Makes the Mole invincible (used while underground).
    /// </summary>
    private void MakeInvincible()
    {
        isInvincible = true;
    }

    /// <summary>
    /// Makes the Mole vulnerable (used when above ground).
    /// </summary>
    private void MakeVulnerable()
    {
        isInvincible = false;
    }

    /// <summary>
    /// Triggers the ripple attack in four directions if cooldown has elapsed.
    /// </summary>
    [ContextMenu("RippleAttack")]
    private void TryAttackWithRipples()
    {
        if (Time.time - lastRippleTime >= rippleCooldown)
        {
            lastRippleTime = Time.time;

            SpawnRipple(180f); // Up
            SpawnRipple(-90f); // Left
            SpawnRipple(0f); // Down
            SpawnRipple(90f); // Right
        }
    }

    /// <summary>
    /// Spawns a ripple attack in the specified direction.
    /// </summary>
    private void SpawnRipple(float zRotation)
    {
        Vector2 travelDirection = new Vector2(Mathf.Sin(zRotation * Mathf.Deg2Rad), -Mathf.Cos(zRotation * Mathf.Deg2Rad));
        Vector3 offset = travelDirection * 1.8f;
        Vector3 pivot = transform.position + (Vector3)Vector2.up * 0.5f;
        GameObject ripple = Instantiate(rippleEffectPrefab, pivot - offset, Quaternion.Euler(0f, 0f, zRotation));

        RippleEffect rippleScript = ripple.GetComponent<RippleEffect>();
        rippleScript.OnPlayerHit += HandleRipplePlayerHit;
    }

    /// <summary>
    /// Handles the event when a ripple hits the player.
    /// </summary>
    private void HandleRipplePlayerHit(Rigidbody2D playerRb, RippleEffect ripple)
    {
        float zRotation = ripple.transform.rotation.eulerAngles.z;
        Vector2 hitDirection = new Vector2(Mathf.Sin(zRotation * Mathf.Deg2Rad), -Mathf.Cos(zRotation * Mathf.Deg2Rad));

        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, hitDirection, baseDamage, thrust, knockTime);
    }

    /// <summary>
    /// Initiates the dig up animation and state.
    /// </summary>
    private void DigUp()
    {
        lastRippleTime = Time.time;
        isDiggingUp = true;
        ChangeAnimationClip("Dig Up");
    }

    /// <summary>
    /// Called when the mole digs up (animation event).
    /// Enables the arm, makes the mole vulnerable, and transitions to attacking state.
    /// </summary>
    public void OnDigUp()
    {
        MakeVulnerable();
        isDugIn = false;
        isDiggingUp = false;

        if (currentState == EnemyStatus.Approaching)
        {
            arm.SetActive(true);
            movement = Vector2.zero;
            ChangeState(EnemyStatus.Attacking);
            ChangeAnimationClip("Up");
        }
        else
        {
            DigDown();
        }
    }

    /// <summary>
    /// Initiates the dig down animation and disables the arm.
    /// </summary>
    private void DigDown()
    {
        arm.SetActive(false);
        ChangeAnimationClip("Dig Down");
    }

    /// <summary>
    /// Called when the mole digs down (animation event).
    /// Makes the mole invincible, sets dug in state, and transitions to idle.
    /// </summary>
    public void OnDigDown()
    {
        MakeInvincible();
        isDugIn = true;
        ChangeAnimationClip("Idle");
        movement = Vector2.zero;
        ChangeState(EnemyStatus.Idle);
    }

    /// <summary>
    /// Modifies the Mole's movement speed by a factor.
    /// </summary>
    public override void ModifySpeed(float factor)
    {
        moveSpeed *= factor;
    }

    /// <summary>
    /// Modifies the Mole's damage by a factor.
    /// </summary>
    public override void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    /// <summary>
    /// Modifies the Mole's defense by a factor.
    /// </summary>
    public override void ModifyDefense(float factor)
    {
        defenseFactor *= factor;
    }

    /// <summary>
    /// No mana regen behavior for the Mole.
    /// </summary>
    public override void ModifyManaRegen(float factor)
    {
        // Do nothing
    }

    /// <summary>
    /// Freezes or unfreezes the Mole, affecting movement and animation speed.
    /// </summary>
    public override void OnFreeze(bool freezeOn = true)
    {
        if (freezeOn)
        {
            movement = Vector2.zero;
            animator.speed = 0;
            armAnimators.speed = 0;
        }
        else
        {
            animator.speed = 1;
            armAnimators.speed = 1;
        }
    }

    /// <summary>
    /// Moves the Mole based on its current movement vector and speed.
    /// </summary>
    public override void DoMovement()
    {
        movement = movement.normalized;
        rb2d.MovePosition((Vector2)transformComponent.position + movement * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Enables or disables sight obstruction for the Mole.
    /// </summary>
    public override void DoObstructSight(bool onOff)
    {
        ObstructSight(onOff);
    }
}
