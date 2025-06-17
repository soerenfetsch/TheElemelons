using System.Collections;
using UnityEngine;

/// <summary>
/// Status states for the Snek enemy.
/// </summary>
public enum SnekStatus
{
    Crawl,
    Dash,
    Attack
}

/// <summary>
/// Controls the Snek enemy's behavior, including movement, dashing, tongue attacks, and state transitions.
/// Handles wall avoidance, attack cooldowns, and interaction with the Witch boss.
/// </summary>
public class Snek : Enemy
{
    [Header("Bound Components")]
    /// <summary>Prefab for the death effect.</summary>
    public GameObject deathEffect;
    // Delay before the death effect is played.
    float deathEffectDelay = 0.5f;
    /// <summary>Flash effect for the Snek.</summary>
    public FlashEffect flashEffect;
    /// <summary>Duration of the flash effect.</summary>
    public float flashDuration = 0.2f;
    // Pathfinding system reference.
    LineOfSightPathfinder pathfinder;
    // Rigidbody2D for movement.
    Rigidbody2D rb;
    // Animator for the Snek.
    Animator animator;
    // Reference to the Witch (if spawned by Witch).
    GameObject witch;

    [Header("Ranges")]
    // Aggro range for detecting the player.
    float aggroRange = 30f;
    // Range for dashing towards the player.
    float dashRange = 10f;
    // Range for tongue attack.
    float tongueRange = 5f;

    [Header("Flags")]
    // Whether the tongue attack can be used.
    bool canTongue = true;
    // Cooldown for tongue attack.
    float tongueCooldown = 6f;
    // Duration of the attack state.
    float attackTime = 3f;
    /// <summary>Current Snek status.</summary>
    public SnekStatus currentSnekStatus = SnekStatus.Crawl;
    // Whether the Snek has finished spawning.
    bool spawned = true;

    [Header("Damage")]
    // Base damage dealt to the player.
    float baseDamage = 0.5f;
    // Knockback duration.
    float knockTime = 0.3f;
    // Knockback force.
    float thrust = 5f;

    [Header("Movement")]
    // Raycast hit for wall detection.
    RaycastHit2D hit;
    // Movement speed factor (affected by buffs/debuffs).
    float moveFactor = 1f;
    // Speed while dashing.
    float dashSpeed = 7f;
    // Speed while attacking.
    float attackSpeed = 10f;
    // Speed while crawling.
    float crawlSpeed = 4f;

    /// <summary>
    /// Initializes the Snek, sets up references, and handles spawning if created by the Witch.
    /// </summary>
    void Start()
    {
        elementType = ElementType.Earth;
        attackFactor = 1;
        defenseFactor = 1;
        playerTransform = GameObject.Find("Player").transform;
        player = playerTransform.GetComponent<Player>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        movement = Vector2.zero;
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        witch = GameObject.Find("Witch");
        if (witch != null)
        {
            spawned = false;
            StartCoroutine(Spawn());
        }
    }

    /// <summary>
    /// If snake was spawned by Witch, spawn at correct point with correct orientation.
    /// Start at Witch scale and gradually become scale 1.
    /// </summary>
    IEnumerator Spawn()
    {
        aggroRange = 50f;
        float orientation = witch.GetComponent<Animator>().GetFloat("Horizontal");
        if (orientation < 0)
        {
            transform.position = witch.transform.GetChild(3).GetChild(0).transform.position;
            movement = Vector2.left;
        }
        else
        {
            transform.position = witch.transform.GetChild(3).GetChild(1).transform.position;
            movement = Vector2.right;
        }
        float scale = witch.transform.localScale.x;
        transform.localScale = new Vector3(scale, scale, 0);
        float fadeTimer = 3f;
        float fadeDuration = fadeTimer;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            scale = alpha + 1;
            transform.localScale = new Vector3(scale, scale, 0);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        spawned = true;
    }

    /// <summary>
    /// Handles movement, health, range, and state logic each physics frame.
    /// </summary>
    void FixedUpdate()
    {
        if (spawned)
        {
            NewRayCast();
            ActOnHealth();
            ActOnRange();
            if (CanAct())
            {
                ActOnStatus();
            }
            DoMovementWithChecks();
        }
    }

    /// <summary>
    /// Performs short raycasts in the movement direction for wall detection.
    /// </summary>
    void NewRayCast()
    {
        float rayLength = 2f;
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, 10f) * movement, rayLength, mask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, -10f) * movement, rayLength, mask);
        hit = hit0 ? hit0 : (hit1 ? hit1 : (hit2 ? hit2 : hit0));
    }

    /// <summary>
    /// Updates the animator to reflect the Snek's current state and movement direction.
    /// </summary>
    void Update()
    {
        UpdateAnimator();
    }

    /// <summary>
    /// Updates animator parameters for movement and speed.
    /// </summary>
    void UpdateAnimator()
    {
        if (currentSnekStatus == SnekStatus.Crawl)
        {
            animator.SetBool("Speed", false);
        }
        else
        {
            animator.SetBool("Speed", true);
        }
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    /// <summary>
    /// Handles wall avoidance and movement execution.
    /// </summary>
    public override void DoMovement()
    {
        int turn = 15;
        for (int i = 0; i < 360f; i = i + turn)
        {
            if (hit)
            {
                movement = Quaternion.Euler(0f, 0f, turn) * movement;
                NewRayCast();
            }
        }
        movement.Normalize();
        Vector2 moving_by = movement * GetMoveSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moving_by);
    }

    /// <summary>
    /// Freezes or unfreezes the Snek, affecting movement and animation speed.
    /// </summary>
    public override void OnFreeze(bool freezeOn = true)
    {
        if (freezeOn)
        {
            movement = Vector2.zero;
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1;
        }
    }

    /// <summary>
    /// Gets the current movement speed based on Snek status.
    /// </summary>
    public float GetMoveSpeed()
    {
        if (currentSnekStatus == SnekStatus.Dash)
        {
            return dashSpeed * moveFactor;
        }
        else if (currentSnekStatus == SnekStatus.Attack)
        {
            return attackSpeed * moveFactor;
        }
        return crawlSpeed * moveFactor;
    }

    /// <summary>
    /// Enforces state transitions in derived classes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// Handles aggro, dash, and tongue attack logic based on distance to player.
    /// </summary>
    void ActOnRange()
    {
        if (Mathf.Abs((transform.position - playerTransform.position).magnitude) < aggroRange)
        {
            ChangeState(EnemyStatus.Approaching);
        }
        else
        {
            ChangeState(EnemyStatus.Idle);
        }
        if (currentSnekStatus != SnekStatus.Attack && currentState != EnemyStatus.Idle)
        {
            float dist = Mathf.Abs((transform.position - playerTransform.position).magnitude);
            bool isDashing = dist < dashRange;
            if (!isDashing)
            {
                currentSnekStatus = SnekStatus.Crawl;
            }
            else
            {
                currentSnekStatus = SnekStatus.Dash;
            }
            if (canTongue && dist < tongueRange && CanAct())
            {
                StartCoroutine(DoTongue());
            }
        }
    }

    /// <summary>
    /// Defines behavior based on the current state.
    /// </summary>
    public override void ActOnStatus()
    {
        if (currentState == EnemyStatus.Idle)
        {
            OnIdleStatus();
        }
        else if (currentSnekStatus == SnekStatus.Crawl)
        {
            if (Time.frameCount % 30 == 0)
            {
                OnCrawlStatus();
            }
        }
        else if (currentSnekStatus == SnekStatus.Dash)
        {
            if (Time.frameCount % 30 == 0)
            {
                OnDashStatus();
            }
        }
        else if (currentSnekStatus == SnekStatus.Attack)
        {
            OnAttackStatus();
        }
    }

    /// <summary>
    /// Handles the attack state (no movement).
    /// </summary>
    void OnAttackStatus()
    {
    }

    /// <summary>
    /// Handles dash state: moves quickly toward or away from the player.
    /// </summary>
    void OnDashStatus()
    {
        Vector2 dir;
        if (canTongue)
        {
            dir = pathfinder.CheckPlayerInSight(transform.position, aggroRange);
            if (dir == Vector2.zero)
            {
                dir = pathfinder.CheckPlayerReachable(transform.position, aggroRange);
            }
        }
        else
        {
            dir = transform.position - playerTransform.position;
        }

        Vector2 dirAbs = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
        if (dirAbs.x > dirAbs.y)
        {
            movement = new Vector2(dir.x, 0);
        }
        else
        {
            movement = new Vector2(0, dir.y);
        }
    }

    /// <summary>
    /// moves in direction which was decided at the moment it switched into this state (towards player). 
    /// Automatically switches out of state after given time (~2s).
    /// </summary>
    /// <returns></returns>
    IEnumerator DoTongue()
    {
        movement = playerTransform.position - transform.position;
        currentSnekStatus = SnekStatus.Attack;
        animator.SetTrigger("Tongue");
        yield return new WaitForSeconds(attackTime);
        animator.SetTrigger("Tongue");
        currentSnekStatus = SnekStatus.Dash;
        canTongue = false;
        yield return new WaitForSeconds(tongueCooldown);
        canTongue = true;
    }

    /// <summary>
    ///  no movement.
    /// </summary>
    public void OnIdleStatus()
    {
        movement = Vector2.zero;
    }

    /// <summary>
    /// If snake is too far away, movement speed is decreased. Moves towards player if attack cooldown isnt running, else moves away.
    /// </summary>
    public void OnCrawlStatus()
    {
        Vector2 dir = pathfinder.CheckPlayerInSight(transform.position, aggroRange);
        if (dir == Vector2.zero)
        {
            dir = pathfinder.CheckPlayerReachable(transform.position, aggroRange);
        }
        if (!canTongue)
        {
            dir *= -1;
        }
        movement = new Vector2(Mathf.Sign(dir.x), Mathf.Sign(dir.y));
    }

    /// <summary>
    /// Handles health logic and triggers death effect if health is zero.
    /// </summary>
    public override void ActOnHealth()
    {
        if (health.IsZero())
        {
            DeathEffect.SpawnDeathEffect(deathEffect, transform.position, deathEffectDelay);
            Destroy(gameObject, 0);
        }
    }

    /// <summary>
    /// Does nothing for mana logic.
    /// </summary>
    public override void ActOnMana()
    { }

    /// <summary>
    /// Handles knockback and stagger state for the Snek.
    /// </summary>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }

    /// <summary>
    /// Handles the stagger state and flash effect.
    /// </summary>
    private IEnumerator HandleStagger(float staggerTime)
    {
        EnemyStatus previousState = currentState;
        ChangeState(EnemyStatus.Staggered);
        flashEffect?.Flash(flashDuration);
        yield return new WaitForSeconds(staggerTime);
        ChangeState(previousState);
    }

    /// <summary>
    /// Modifies the Snek's movement speed by a factor.
    /// </summary>
    public override void ModifySpeed(float factor)
    {
        moveFactor *= factor;
    }

    /// <summary>
    /// Modifies the Snek's damage by a factor.
    /// </summary>
    public override void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    /// <summary>
    /// Modifies the Snek's defense by a factor.
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
    /// Deals damage (~0.5 hearts) to the player if they collide.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger
            && !isMovementBlocked)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(gameObject, other.GetComponent<Rigidbody2D>(), elementType, attackDirection, baseDamage, thrust, knockTime);
        }
    }

    /// <summary>
    /// Enables or disables sight obstruction for the Snek.
    /// </summary>
    public override void DoObstructSight(bool onOff)
    {
        ObstructSight(onOff);
    }
}
