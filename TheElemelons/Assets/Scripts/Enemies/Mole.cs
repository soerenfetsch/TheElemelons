using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : Enemy
{
    [Header("Components")]
    public GameObject arm;
    public GameObject rippleEffectPrefab;

    private Transform transformComponent;
    private Rigidbody2D rb2d;
    private BoxCollider2D colliderBox;
    private Animator animator;
    private FlashEffect flashEffect;
    private FlashEffect armFlashEffect;
    Animator armAnimators;

    [Header("Pathfinding")]
    Vector2 movement;
    public Transform playerTransform; // To store the player's transform for pathfinding.
    public LineOfSightPathfinder pathfinder; // Reference to the pathfinding system.

    [Header("Behavior")]
    public float moveSpeed = 1f;
    public float chaseRadius = 10f;
    public float attackRadius = 2f;
    public float digOutInterval = 15f;

    private bool isDugIn = true;
    private bool canDigOut = true;
    private bool isAlive = true;
    private bool isDiggingUp = false;

    [Header("Combat")]
    public float baseDamage = 1f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
    public float rippleCooldown = 2f;
    public float flashDuration = 0.2f;

    private float lastRippleTime = 0f;

    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;

    [Header("Animation")]
    private string lastAnimationClip = null;


    public void Initialize(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.pathfinder = pathfinder;

        InitializeCombatComponents();
    }
    /*
        IEnumerator FindPlayerPathfinder()
        {
            while (playerTransform == null || pathfinder == null)
            {
                playerTransform = GameObject.Find("Player").transform;
                pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
                yield return new WaitForSeconds(0.01f);
            }

        }
    */
    public void InitializeComponents()
    {
        transformComponent = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        colliderBox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        armAnimators = transform.GetChild(0).GetComponent<Animator>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();
        flashEffect = GetComponent<FlashEffect>();
        armFlashEffect = arm.GetComponent<FlashEffect>();
        playerTransform = GameObject.Find("Player").transform;
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
        if (!isMovementBlocked)
        {
            ActOnStatus();
        }
        DoMovementWithChecks();
        ActOnHealth();
        ActOnMana();
    }

    public override void ChangeState(EnemyStatus newState)
    {
        if (currentState == newState) return;
        //        Debug.Log($"Mole: Changing state from {currentState} to {newState}");
        currentState = newState;
    }

    /// <summary>
    /// Acts on the current status of the enemy.
    /// This method handles the behavior of the enemy based on its current state. The enemy can be in one of the following states:
    /// Idle, Approaching, Attacking, Retreating, or Staggered. The behavior includes checking the player's distance, moving towards
    /// the player, attacking, retreating to the home position, and handling the stagger state.
    /// While status idle, the mole remains dug in the ground. For every showInterval, it will pop out of the ground. If the player
    /// is within the chase radius, it will start moving towards the player. When the player is within the attack radius, it will
    /// dig up and attack the player.
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

    private void ChangeAnimationClip(string v)
    {
        if (lastAnimationClip != v)
        {
            animator.Play(v);
            lastAnimationClip = v;
        }
    }

    private IEnumerator PopOutandDigIn()
    {
        DigUp();
        yield return new WaitForSeconds(digOutInterval);
        canDigOut = true;
    }

    /// <summary>
    /// Acts on the current health status of the enemy.
    /// This method checks if the enemy's health is zero and if the enemy is still alive. If both conditions are met,
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
    /// There is no mana behavior for the Mole enemy, so this method does nothing.
    /// </summary>
    public override void ActOnMana()
    {
        // Do nothing
    }

    /// <summary>
    /// When the Mole gets knocked back, it enters the staggered state for a certain amount of time.
    /// </summary>
    /// <param name="knockTime"></param>
    /// <returns></returns>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }

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
    /// Check if the player is within a certain radius of the enemy (just positions)
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool CheckPlayerWithinRadius(float radius)
    {
        float distance = Vector2.Distance(GetFeetPosition(), playerTransform.position);
        return distance <= radius;
    }

    /// <summary>
    /// Check if the player is outside a certain radius of the enemy (just positions)
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool CheckPlayerOutsideRadius(float radius)
    {
        return Vector2.Distance(GetFeetPosition(), playerTransform.position) > radius;
    }

    /// <summary>
    /// Check if the player is within a certain reach radius of the enemy (using pathfinding).
    /// If player is in sight within the radius or a path to the player is reachable inside the
    /// readius, returns true, else false.
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool CheckPlayerReachableWithinRadius(float radius)
    {
        bool reachable = pathfinder.CheckPlayerInSight(GetFeetPosition(), radius) != Vector2.zero
                      || pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) != Vector2.zero;
        return reachable;
    }

    /// <summary>
    /// Check if the player is outside a certain reach radius of the enemy (using pathfinding).
    /// If player is in sight outside the radius and a path to the player is not reachable inside the
    /// readius, returns true, else false.
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool CheckPlayerUnreachableWithinRadius(float radius)
    {
        return pathfinder.CheckPlayerReachable(GetFeetPosition(), radius) == Vector2.zero;
    }

    private void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        // Find the best direction to move toward the player using pathfinding (collision at feet level)
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

    private Vector2 GetFeetPosition()
    {
        return new Vector2(transformComponent.position.x, colliderBox.bounds.min.y);
    }

    private IEnumerator HandleStagger(float staggerTime)
    {
        EnemyStatus previousState = currentState;
        ChangeState(EnemyStatus.Staggered);
        string lastAnimation = lastAnimationClip;

        ChangeAnimationClip("Stagger");
        flashEffect?.Flash(flashDuration);
        // Flash the arm only if arm is active
        if (arm.activeSelf)
            armFlashEffect?.Flash(flashDuration);

        yield return new WaitForSeconds(staggerTime);
        ChangeState(previousState);
        ChangeAnimationClip(lastAnimation);
    }

    private void MakeInvincible()
    {
        isInvincible = true;
    }

    private void MakeVulnerable()
    {
        isInvincible = false;
    }

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

    private void SpawnRipple(float zRotation)
    {
        Vector2 travelDirection = new Vector2(Mathf.Sin(zRotation * Mathf.Deg2Rad), -Mathf.Cos(zRotation * Mathf.Deg2Rad));
        Vector3 offset = travelDirection * 1.8f;
        Vector3 pivot = transform.position + (Vector3)Vector2.up * 0.5f;
        GameObject ripple = Instantiate(rippleEffectPrefab, pivot - offset, Quaternion.Euler(0f, 0f, zRotation));

        RippleEffect rippleScript = ripple.GetComponent<RippleEffect>();

        // Optional: subscribe to player hit event
        rippleScript.OnPlayerHit += HandleRipplePlayerHit;
    }

    private void HandleRipplePlayerHit(Rigidbody2D playerRb, RippleEffect ripple)
    {
        // Get the hit direction from the z rotation of the ripple
        float zRotation = ripple.transform.rotation.eulerAngles.z;
        Vector2 hitDirection = new Vector2(Mathf.Sin(zRotation * Mathf.Deg2Rad), -Mathf.Cos(zRotation * Mathf.Deg2Rad));

        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, hitDirection, baseDamage, thrust, knockTime);
    }

    private void DigUp()
    {
        lastRippleTime = Time.time; // So attacking is started shortly after digging up
        isDiggingUp = true; // Set digging up state to true
        ChangeAnimationClip("Dig Up");
    }

    /// <summary>
    /// Called when the mole digs up. This method is triggered at
    /// the end of the animation event.
    /// It enables the arm and allows the attack to be triggered.
    /// The mole becomes vulnerable when digging up.
    /// </summary>
    /// <returns></returns>
    public void OnDigUp()
    {
        MakeVulnerable(); // Make the mole vulnerable when digging up
        isDugIn = false; // Set dug in state to false
        isDiggingUp = false; // Reset digging up state

        if (currentState == EnemyStatus.Approaching)
        {
            arm.SetActive(true); // Enable the arm when digging up
            ChangeState(EnemyStatus.Attacking);
            ChangeAnimationClip("Up");
        }
        else
        {
            DigDown();
        }
    }

    private void DigDown()
    {
        arm.SetActive(false); // Disable the arm when digging down
        ChangeAnimationClip("Dig Down");
    }

    /// <summary>
    /// Called when the mole digs down. This method is triggered at the
    /// end of the animation by an event.
    /// It disables the arm and prevents the attack from being triggered.
    /// The mole becomes invincible when digging down and the dug in state
    /// is set to true. The state and animation are changed to idle.
    /// </summary>
    /// <returns></returns>
    public void OnDigDown()
    {
        MakeInvincible();
        isDugIn = true;
        ChangeAnimationClip("Idle");
        ChangeState(EnemyStatus.Idle);
    }

    public override void ModifySpeed(float factor)
    {
        moveSpeed *= factor;
    }

    public override void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    public override void ModifyDefense(float factor)
    {
        defenseFactor *= factor;
    }

    public override void ModifyManaRegen(float factor)
    {
        // Do nothing
    }
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
    public override void DoMovement()
    {
        movement = movement.normalized;
        rb2d.MovePosition((Vector2)transformComponent.position + movement * moveSpeed * Time.deltaTime);
    }
}
