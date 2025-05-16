using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WitchState
{
    Walk,
    Hover,
    Bald
}
public class Witch : Enemy
{
    float dice;
    public float intervalThrowdice = 3f;
    public WitchState currentWitchState = WitchState.Walk;
    [Header("CombatRangeHandling")]
    public float range = 10f;
    public float aggroRange = 30f;
    public float minimalRange = 6f;
    public float maximalRange = 18f;
    public float rangeTolerance = 1f;
    RangeState currentRangeState = RangeState.OutOfRange;
    [Header("Movement")]
    Vector2 movement;
    float moveSpeed = 3f;
    float hoverSpeedFactor = 1.3f;
    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;
    FlashEffect flashEffectBody;
    public float flashDuration = 0.2f;
    [Header("Bound Components")]
    Transform playerTransform;
    LineOfSightPathfinder pathfinder;
    Rigidbody2D rb;
    Animator bodyAnimator;
    Animator hairAnimator;
    Animator wandAnimator;
    PlayerMovement playerMovement;
    [Header("Medusa")]
    public float medusaTime = 3f;
    [Header("SpellCooldowns")]
    bool canMedusa = true;
    float medusaCooldown = 6f;
    float medusaProbability = 0.9f;
    bool canCastOffensiveSpell = true;
    float offensiveSpellCooldown = 3f;
    float offensiveSpellProbability = 0.5f;
    bool canCastSurvivalSpell = true;
    float survivalSpellCooldown = 3f;
    float survivalSpellProbability = 0.5f;
    bool canBald = false;
    float baldCooldown = 10f;
    bool baldTimerisRunning = false;
    float baldProbability = 0.85f;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = GetComponent<Animator>();
        hairAnimator = transform.GetChild(0).GetComponent<Animator>();
        wandAnimator = transform.GetChild(1).GetComponent<Animator>();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
        movement = new Vector2(0, 0);
    }

    void FixedUpdate()
    {
        if (Time.frameCount % intervalThrowdice == 0)
        {
            dice = UnityEngine.Random.Range(0, 1);
        }
        ActOnStatus();
        DoMovement();
        ManageBodyAnimators();
    }
    void ManageBodyAnimators()
    {
        bodyAnimator.SetFloat("Horizontal", movement.x);
        hairAnimator.SetFloat("Horizontal", movement.x);
        wandAnimator.SetFloat("Horizontal", movement.x);
        bool walking = false;
        bool hovering = false;
        bool baldy = false;
        if (currentWitchState == WitchState.Walk)
        {
            walking = true;
        }
        else if (currentWitchState == WitchState.Hover)
        {
            hovering = true;
        }
        else if (currentWitchState == WitchState.Bald)
        {
            baldy = true;
        }
        bodyAnimator.SetBool("Walk", walking);
        bodyAnimator.SetBool("Hover", hovering);
        wandAnimator.SetBool("Hover", hovering);
        bodyAnimator.SetBool("Bald", baldy);
    }
    public void DoMovement()
    {
        movement.Normalize();
        Vector2 moving_by = movement * GetMoveSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moving_by);
    }
    public float GetMoveSpeed()
    {
        if (currentWitchState == WitchState.Walk)
        {
            return moveSpeed;
        }
        else if (currentWitchState == WitchState.Hover || currentWitchState == WitchState.Bald)
        {
            return moveSpeed * hoverSpeedFactor;
        }
        return moveSpeed;
    }
    /// <summary>
    /// Abstract method to define behavior based on the current state.
    /// </summary>
    public override void ActOnStatus()
    {
        CalcRangeState();
        if (currentRangeState == RangeState.OutOfRange)
        {
            ChangeState(EnemyStatus.Idle);
        }
        else
        {
            ChangeState(EnemyStatus.Approaching);
        }
        if (currentState == EnemyStatus.Idle)
        {
            OnIdleStatus();
        }
        else if (currentWitchState == WitchState.Walk)
        {
            OnWalkStatus();
        }
        else if (currentWitchState == WitchState.Hover)
        {
            OnHoverStatus();
        }
        else if (currentWitchState == WitchState.Bald)
        {
            OnBaldStatus();
        }
    }
    public void OnWalkStatus()
    { }
    public void OnHoverStatus()
    {
        if (currentRangeState == RangeState.Close)
        {
            if (canMedusa && dice < medusaProbability)
            {
                StartCoroutine(DoMedusa());
            }
            else if (canCastSurvivalSpell && dice < survivalSpellProbability && !hairAnimator.GetBool("Medusa"))
            {
                // evade with poof
            }
            else
            {
                // run to range, shoot
            }

        }
        else if (currentRangeState == RangeState.InRange)
        {
            if (canCastOffensiveSpell && dice < offensiveSpellProbability)
            {
                // cast
            }
            else if (canCastSurvivalSpell && dice < survivalSpellProbability) // check no wall behind player 
            {
                // evade around player               
            }
            else
            {
                // walk around
            }
        }
        else if (currentRangeState == RangeState.Far)
        {
            if (canBald && dice < baldProbability)
            {
                // go bald
            }
            else if (canCastOffensiveSpell && dice < offensiveSpellProbability)
            {
                // cast offense
            }
            else
            {
                // walk around
            }
        }
    }
    public IEnumerator DoMedusa()
    {
        canMedusa = false;
        hairAnimator.SetBool("Medusa", true);
        yield return new WaitForSeconds(medusaTime);
        hairAnimator.SetBool("Medusa", false);
        StartCoroutine(MedusaCooldown());
    }
    IEnumerator MedusaCooldown()
    {
        yield return new WaitForSeconds(medusaCooldown);
        canMedusa = true;
    }
    public void ChangeWitchState(WitchState newState)
    {
        currentWitchState = newState;
    }
    public void OnBaldStatus()
    { }
    public void OnIdleStatus()
    {
        movement = Vector2.zero;
    }
    public void CalcRangeState()
    {
        if (playerTransform != null)
        {
            float dist = Mathf.Abs((playerTransform.position - transform.position).magnitude);
            if (dist > aggroRange)
            {
                currentRangeState = RangeState.OutOfRange;
                return;
            }
            float error = Mathf.Abs(dist - range) - rangeTolerance;
            if (dist > range && error > 0)
            {
                if (!baldTimerisRunning && currentRangeState != RangeState.Far)
                {
                    StartCoroutine(StartBaldingTimer());
                }
                currentRangeState = RangeState.Far;
            }
            else if (dist < range && error > 0)
            {
                currentRangeState = RangeState.Close;
            }
            else if (error <= 0)
            {
                currentRangeState = RangeState.InRange;
            }
        }
    }

    IEnumerator StartBaldingTimer()
    {
        baldTimerisRunning = true;
        yield return new WaitForSeconds(baldCooldown);
        if (currentRangeState == RangeState.Far)
        {
            canBald = true;
        }
        baldTimerisRunning = false;
    }
    /// <summary>
    /// Abstract method to define behavior based on the enemy's health.
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
    /// Abstract method to enforce state transitions in derived classes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        currentState = newState;
    }
    /// <summary>
    /// define behavior based on the enemy's mana.
    /// does nothing
    /// </summary>
    public override void ActOnMana()
    { }
    /// <summary>
    /// When the Tiki gets knocked back, it enters the staggered state for a certain amount of time.
    /// </summary>
    /// <param name="knockTime"></param>
    /// <returns></returns>
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }
    /// <summary>
    /// changes state to stagger, updates animator and plays flash sequence
    /// then changes state back to idle
    /// </summary>
    /// <param name="staggerTime"></param>
    /// <returns></returns>
    private IEnumerator HandleStagger(float staggerTime)
    {
        ChangeState(EnemyStatus.Staggered);
        flashEffectBody?.Flash(flashDuration);
        yield return new WaitForSeconds(staggerTime);
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
        mana.manaPerRegen *= factor;
    }
}
