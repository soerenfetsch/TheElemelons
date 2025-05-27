using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class TreeLog : Enemy
{
    [Header("Components")]
    private Transform transformComponent;
    private Rigidbody2D rb2d;
    private BoxCollider2D colliderBox;
    private AttackCollider attackCollider;
    private Animator animator;
    private FlashEffect flashEffect;

    [Header("Pathfinding")]
    public LineOfSightPathfinder pathfinder; // Reference to the pathfinding system.

    [Header("Behavior")]
    private Vector2 homePosition;
    private bool isAlive = true;
    public float moveSpeed = 1f;
    public float chaseRadius = 5f;
    public float attackRadius = 0.2f;

    [Header("Combat")]
    public float baseDamage = 1f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
    public float flashDuration = 0.2f;

    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;


    public void Initialize(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.pathfinder = pathfinder;
        movement = Vector2.zero;
        this.player = playerTransform.GetComponent<Player>();
        InitializeCombatComponents();
    }

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
    public override void ChangeState(EnemyStatus newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// Acts on the current status of the enemy.
    /// This method handles the behavior of the enemy based on its current state. The enemy can be in one of the following states:
    /// Idle, Approaching, Attacking, Retreating, or Staggered. The behavior includes checking the player's distance, moving towards
    /// the player, attacking, retreating to the home position, and handling the stagger state.
    /// </summary>
    public override void ActOnStatus()
    {
        if ((playerTransform.position - transform.position).sqrMagnitude < 15)
        {
            Debug.Log("current status: " + currentState);
        }
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
                    ChangeState(EnemyStatus.Idle);
                    animator.SetBool("wakeUp", false);
                }
                break;
            case EnemyStatus.Staggered:
                break;
        }
    }
    public override void DoMovement()
    {
        movement = movement.normalized;
        rb2d.MovePosition((Vector2)transformComponent.position + movement * moveSpeed * Time.deltaTime);
    }
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
    /// There is no mana behavior for the TreeLog enemy, so this method does nothing.
    /// </summary>
    public override void ActOnMana()
    {
        // Do nothing
    }

    /// <summary>
    /// When the TreeStump gets knocked back, it enters the staggered state for a certain amount of time.
    /// </summary>
    /// <param name="knockTime"></param>
    /// <returns></returns>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }

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
    /// Check if the player is within a certain radius of the enemy (just positions)
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool CheckPlayerWithinRadius(float radius)
    {
        return Vector2.Distance(GetFeetPosition(), playerTransform.position) <= radius;
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

        direction.Normalize();
        movement = direction;
    }

    private void MoveTowardHome()
    {
        // Find the best direction to move toward the home using pathfinding (collision at feet level)
        Vector2 direction = RaycastPathfinder.FindBestDirection(GetFeetPosition(), homePosition);
        movement = direction;
    }

    private bool CheckAtHome()
    {
        return Vector2.Distance(GetFeetPosition(), homePosition) < 0.2f;
    }

    private Vector2 GetFeetPosition()
    {
        return new Vector2(transformComponent.position.x, colliderBox.bounds.min.y);
    }

    private IEnumerator HandleStagger(float staggerTime)
    {
        ChangeState(EnemyStatus.Staggered);
        animator.SetTrigger("TrStagger");
        flashEffect?.Flash(flashDuration);
        yield return new WaitForSeconds(staggerTime);
        animator.SetTrigger("TrStopStagger");
        ChangeState(EnemyStatus.Idle);
    }

    private void HandleAttack(Rigidbody2D playerRb)
    {
        if (playerRb == null || isMovementBlocked) return;

        Vector2 attackDirection = (playerRb.transform.position - transform.position).normalized;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, baseDamage, thrust, knockTime);
    }

    private void EnableAttackCollider()
    {
        attackCollider.EnableAttack();
    }

    private void DisableAttackCollider()
    {
        attackCollider.DisableAttack();
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
    public override void DoObstructSight(bool onOff)
    {
        ObstructSight(onOff);
    }
}
