using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearShieldMan : Enemy
{
    [Header("Components")]
    public GameObject shield;
    public GameObject spear;

    private Transform transformComponent;
    private Rigidbody2D rb2d;
    private BoxCollider2D colliderBox;
    private Animator animator;
    private FlashEffect flashEffect;

    [Header("Pathfinding")]
    public Transform playerTransform; // To store the player's transform for pathfinding.
    public LineOfSightPathfinder pathfinder; // Reference to the pathfinding system.

    [Header("Behavior")]
    public float moveSpeed = 1f;
    public float chaseRadius = 10f;
    public float attackRadius = 2f;

    private bool isAlive = true;

    [Header("Combat")]
    public float baseDamage = 1f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
    public float flashDuration = 0.2f;

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

    public void InitializeComponents()
    {
        transformComponent = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        colliderBox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();
        flashEffect = GetComponent<FlashEffect>();

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

    private void InitializeCombatComponents()
    {
        attackFactor = 1f;
        defenseFactor = 1f;
        elementType = ElementType.Earth;
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
        if (currentState == newState) return;
        Debug.Log($"SpearShieldMan: Changing state from {currentState} to {newState}");
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
                if (CheckPlayerReachableWithinRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Approaching);
                    ChangeAnimationClip("Walking");
                    break;
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
                    ChangeState(EnemyStatus.Idle);
                    ChangeAnimationClip("Idle");
                }
                break;
            case EnemyStatus.Attacking:
                AttackWithWeapon();
                if (CheckPlayerOutsideRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Retreating);
                }
                break;
            case EnemyStatus.Retreating:
                break;
            case EnemyStatus.Staggered:
                break;
        }
    }

    // TODO: Implement the attack logic for the SpearShieldMan enemy.
    private void AttackWithWeapon()
    {
        throw new NotImplementedException();
    }

    private void ChangeAnimationClip(string v)
    {
        if (lastAnimationClip != v)
        {
            animator.Play(v);
            lastAnimationClip = v;
        }
    }

    /// <summary>
    /// Acts on the current health status of the enemy.
    /// If the SpearShieldMan's health drops below 25% of its max health, it will attempt to throw its spear at the player.
    /// This method checks if the enemy's health is zero and if the enemy is still alive. If both conditions are met,
    /// it sets the enemy's alive status to false, spawns a death effect at the enemy's position, and destroys the enemy game object.
    /// </summary>
    public override void ActOnHealth()
    {
        // TODO: Implement the spear throw behavior for the SpearShieldMan enemy.
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
        string lastAnimation = lastAnimationClip;

        ChangeAnimationClip("Stagger");
        flashEffect?.Flash(flashDuration);

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
