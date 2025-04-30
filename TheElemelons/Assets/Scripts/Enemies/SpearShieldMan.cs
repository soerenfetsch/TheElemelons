using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearShieldMan : Enemy
{
    [Header("Components")]
    public GameObject shieldPrefab;
    public GameObject spearPrefab;

    private GameObject spearInstance;
    private GameObject shieldInstance;
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
    public float attackCooldown = 1f;

    private float lastAttackTime = 0f;

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

        RandomizeSpearShield();

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
    /// Randomizes whether the SpearShieldMan has a spear, shield, or both.
    /// </summary>
    private void RandomizeSpearShield()
    {
        int randomValue = UnityEngine.Random.Range(0, 3); // 0: spear, 1: shield, 2: both
        switch (randomValue)
        {
            case 0:
                // Instantiate the spear prefab as a child of the SpearShieldMan
                spearInstance = Instantiate(spearPrefab, transformComponent.position, Quaternion.identity, transformComponent);
                break;
            case 1:
                // Instantiate the shield prefab as a child of the SpearShieldMan
                shieldInstance = Instantiate(shieldPrefab, transformComponent.position, Quaternion.identity, transformComponent);
                break;
            case 2:
                // Instantiate both spear and shield prefabs as children of the SpearShieldMan
                spearInstance = Instantiate(spearPrefab, transformComponent.position, Quaternion.identity, transformComponent);
                shieldInstance = Instantiate(shieldPrefab, transformComponent.position, Quaternion.identity, transformComponent);
                break;
        }
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
    /// If the player is within the chase radius, it will start moving towards the player.
    /// When the player is within the attack radius, it will start attacking the player.
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
                AttackWithRandomWeaponIfReady();
                if (CheckPlayerOutsideRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Approaching);
                }
                break;
            case EnemyStatus.Retreating:
                break;
            case EnemyStatus.Staggered:
                break;
        }
    }

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

    private IEnumerator AttackWithSpear()
    {
        spearInstance.GetComponent<Animator>()?.SetTrigger("Attack");
        spearInstance.GetComponent<AttackCollider>()?.EnableAttack();
        yield return new WaitForSeconds(1f); // Wait for the spear attack animation to finish
        spearInstance.GetComponent<AttackCollider>()?.DisableAttack();
    }

    private IEnumerator AttackWithShield()
    {
        shieldInstance.GetComponent<Animator>()?.SetTrigger("Attack");
        shieldInstance.GetComponent<AttackCollider>()?.EnableAttack();
        yield return new WaitForSeconds(1f); // Wait for the shield attack animation to finish
        shieldInstance.GetComponent<AttackCollider>()?.DisableAttack();
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
                SetAnimationFloat("Vertical", 0f);
                SetAnimationFloat("Horizontal", 1f);
            }
            else if (direction.x < 0)
            {
                SetAnimationFloat("Vertical", 0f);
                SetAnimationFloat("Horizontal", -1f);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimationFloat("Vertical", 1f);
                SetAnimationFloat("Horizontal", 0f);
            }
            else if (direction.y < 0)
            {
                SetAnimationFloat("Vertical", -1f);
                SetAnimationFloat("Horizontal", 0f);
            }
        }
    }

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

    private void HandleAttack(Rigidbody2D playerRb)
    {
        if (playerRb == null) return;

        Vector2 attackDirection = (playerRb.transform.position - transform.position).normalized;
        CombatManager.ExecuteAttack(gameObject, playerRb, elementType, attackDirection, baseDamage, thrust, knockTime);
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
