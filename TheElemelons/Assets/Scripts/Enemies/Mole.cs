using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : Enemy
{
    [Header("Components")]
    public GameObject arm;
    public GameObject rippleEffect;
    private Transform transformComponent;
    private Rigidbody2D rb2d;
    private BoxCollider2D colliderBox;
    private AttackCollider attackCollider;
    private Animator animator;
    private Animator rippleAnimator;

    [Header("Pathfinding")]
    public Transform playerTransform; // To store the player's transform for pathfinding.
    public LineOfSightPathfinder pathfinder; // Reference to the pathfinding system.

    [Header("Behavior")]
    private bool isDugIn = true;
    private bool canDigOut = true;
    private bool isAlive = true;
    private float digOutInterval = 15f;
    public float moveSpeed = 1f;
    public float chaseRadius = 10f;
    public float attackRadius = 2f;

    [Header("Combat")]
    public float baseDamage = 1f;
    public float thrust = 1f;
    public float knockTime = 0.2f;

    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;


    public void Initialize(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.pathfinder = pathfinder;

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

        if (arm == null)
        {
            Debug.LogWarning("Mole: No arm found on the GameObject.");
        }
        else
        {
            arm.SetActive(false); // Disable the arm by default
        }

        if (rippleEffect == null)
        {
            Debug.LogWarning("Mole: No ripple effect found on the GameObject.");
        }
        else
        {
            rippleAnimator = rippleEffect.GetComponent<Animator>();
        }

        currentState = EnemyStatus.Idle;
    }

    private void InitializeCombatComponents()
    {
        attackFactor = 1f;
        defenseFactor = 1f;
        elementType = ElementType.Earth;

        if (attackCollider != null)
        {
            attackCollider.OnAttackTriggered += HandleAttack;
        }
        else
        {
            Debug.LogWarning("Mole: No AttackCollider found on the GameObject.");
        }
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
        ActOnStatus();
        ActOnHealth();
        ActOnMana();
    }

    public override void ChangeState(EnemyStatus newState)
    {
        Debug.Log($"Mole: Changing state from {currentState} to {newState}");
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
                if (CheckPlayerReachableWithinRadius(chaseRadius) && isDugIn)
                {
                    ChangeState(EnemyStatus.Approaching);
                    animator.SetBool("WakeUp", true);
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
                    ChangeState(EnemyStatus.Attacking);
                    StartAttacking();
                    animator.SetBool("Attacking", true);
                }
                else if (CheckPlayerUnreachableWithinRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Idle);
                    animator.SetBool("WakeUp", false);
                }
                break;
            case EnemyStatus.Attacking:
                if (CheckPlayerOutsideRadius(attackRadius))
                {
                    StopAttacking();
                    ChangeState(EnemyStatus.Approaching);
                    animator.SetBool("Attacking", false);
                }
                break;
            case EnemyStatus.Retreating:
                break;
            case EnemyStatus.Staggered:
                break;
        }
    }

    private IEnumerator PopOutandDigIn()
    {
        animator.SetTrigger("Check");
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
        rb2d.MovePosition((Vector2)transformComponent.position + direction * moveSpeed * Time.deltaTime);
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
        bool previousArmState = arm.activeSelf;
        arm.SetActive(false);
        animator.SetTrigger("Stagger");
        yield return new WaitForSeconds(staggerTime);
        ChangeState(previousState);
        arm.SetActive(previousArmState);
    }

    private void HandleAttack(Rigidbody2D playerRb)
    {
        if (playerRb == null) return;

        Vector2 attackDirection = (playerRb.transform.position - transform.position).normalized;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, baseDamage, thrust, knockTime);
    }

    private void MakeInvincible()
    {
        isInvincible = true;
    }

    private void MakeVulnerable()
    {
        isInvincible = false;
    }

    private void StartAttacking()
    {
        rippleAnimator.SetBool("Attacking", true);
        EnableAttackCollider();
    }

    private void StopAttacking()
    {
        rippleAnimator.SetBool("Attacking", false);
        DisableAttackCollider();
    }

    private void EnableAttackCollider()
    {
        attackCollider.EnableAttack();
    }

    private void DisableAttackCollider()
    {
        attackCollider.DisableAttack();
    }

    /// <summary>
    /// Called when the mole digs up. This method is triggered by the animation event.
    /// It enables the arm and allows the attack to be triggered.
    /// The mole becomes vulnerable when digging up.
    /// </summary>
    /// <returns></returns>
    public void OnDigUp()
    {
        Debug.Log("Mole: Digging up!");
        if (currentState == EnemyStatus.Attacking)
        {
            arm.SetActive(true); // Enable the arm when digging up
        }
        MakeVulnerable(); // Make the mole vulnerable when digging up
        isDugIn = false; // Set dug in state to false
    }

    /// <summary>
    /// Called when the mole digs down. This method is triggered by the animation event.
    /// It disables the arm and prevents the attack from being triggered.
    /// The mole becomes invincible when digging down.
    /// </summary>
    /// <returns></returns>
    public void OnDigDown()
    {
        Debug.Log("Mole: Digging down!");
        arm.SetActive(false); // Disable the arm when digging down
        MakeInvincible(); // Make the mole invincible when digging down
        isDugIn = true; // Set dug in state to true
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
}
