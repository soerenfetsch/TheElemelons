using System.Collections;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter.Xml;
using Codice.CM.Client.Differences;
using Ionic.BZip2;
using UnityEngine;

public enum TikiState
{
    HealFriends,
    Panic,
    Circle,
    ComeBack,
}
public enum RangeState
{
    OutOfRange,
    Far,
    InRange,
    Close
}
public class Tiki : Enemy
{
    public Transform playerTransform;
    public TikiState currentTikiState;
    Animator bodyAnimator;
    [Header("Skirt")]
    bool flowerSkirt = false;
    float flowerProb = 0.5f;
    Animator skirtAnimator;
    [Header("Ellipse flight")]
    Vector2 headTarget;
    public float steepness;
    public float headSpeed;
    Vector2 startPosition;
    int clockDir;
    bool targetReached = false;
    float currentDistanceTraveled = 0f;
    public float correctionFactor = 0.5f;
    Vector2 directionInitial;
    Vector2 perpendicularDirection;
    [Header("Head")]
    public float spinningHeadCooldown = 2.5f;
    public Vector2 headMovement;
    bool headAttached = true;
    bool headSpinning = false;
    public float headSpinSpeed = 3f;
    Rigidbody2D rbHead;
    Transform headTransform;
    public GameObject headCenter;
    [Header("Pathfinding")]
    public LineOfSightPathfinder pathfinder; // Reference to the pathfinding system.
    [Header("BodyMovement")]
    Collider2D bodyCollider2D;
    RaycastHit2D hit;
    Transform bodyTransform;
    int clockwiseFactor = 1;
    Vector2 bodyMovement;
    public float moveSpeed = 1f;
    Rigidbody2D rbBody;
    int directionChangeCounter = 0;
    public int directionChangeStop = 10;
    [Header("CombatRangeHandling")]
    public float range;
    public float AggroRange = 30f;
    public float minimalRange = 6f;
    public float maximalRange = 20f;
    public float rangeTolerance = 1f;
    public RangeState currentRangeState;
    [Header("Combat")]
    public float baseDamage = 1f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;
    [Header("Bools")]
    bool speedBuffGiven = false;
    // Start is called before the first frame update
    void Start()
    {
        bodyAnimator = transform.GetChild(0).GetComponent<Animator>();
        bodyTransform = transform.GetChild(0).transform;
        skirtAnimator = transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
        mana = GetComponent<Mana>();
        health = GetComponent<Health>();
        rbBody = transform.GetChild(0).GetComponent<Rigidbody2D>();
        flowerSkirt = Random.Range(0, 2) < flowerProb * 2;
        skirtAnimator.SetBool("Flowers", flowerSkirt);
        currentState = EnemyStatus.Approaching;
        range = minimalRange;
        GameObject head = transform.GetChild(1).gameObject;
        rbHead = head.transform.GetChild(0).GetComponent<Rigidbody2D>();
        headTransform = head.transform;
        clockwiseFactor = (int)Mathf.Sign(Random.Range(-1, 1));
        if (currentTikiState == TikiState.Circle)
        {
            StartCoroutine(SpinningHeadCooldown());
        }
    }
    void FixedUpdate()
    {
        ActOnStatus();
        HandleHead();
        DoMovement();
    }
    public void HandleHead()
    {
        if (headAttached)
        {
            headTransform.position = HeadOnShouldersPosition();
        }
        if (headSpinning)
        {
            headTransform.Rotate(Vector3.forward, headSpinSpeed * Time.fixedDeltaTime);
        }
        else
        {
            headTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (!headAttached)
        {
            if (!targetReached)
            {
                headTransform.position = CalcParabolePosition();
            }
            else
            {
                headTransform.position = CalcReturnToBodyPosition();
            }
        }
    }
    Vector3 HeadOnShouldersPosition()
    {
        return bodyTransform.position + new Vector3(0, 1.75f, 0);
    }
    /// <summary>
    /// Abstract method to define behavior based on the current state.
    /// </summary>
    public override void ActOnStatus()
    {
        CalcRangeState();
        if (currentState == EnemyStatus.Idle)
        {
            OnIdleStatus();
        }
        else if (currentTikiState == TikiState.Circle)
        {
            OnCircleTiki();
        }
        else if (currentTikiState == TikiState.ComeBack)
        {
            OnComeBack();
        }
        ActOnHealth();
    }
    /// <summary>
    /// Abstract method to enforce state transitions in derived classes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        directionChangeCounter = 0;
        currentState = newState;
    }
    public void ChangeTikiState(TikiState newState)
    {
        currentTikiState = newState;
        if (newState == TikiState.Circle)
        {
            StartCoroutine(SpinningHeadCooldown());
        }
    }
    IEnumerator SpinningHeadCooldown()
    {
        yield return new WaitForSeconds(spinningHeadCooldown);
        if (currentTikiState == TikiState.Circle)
        {
            StartCoroutine(StartSpinningAttack());
        }
    }
    IEnumerator StartSpinningAttack(bool linger = true)
    {
        headSpinning = true;
        if (linger)
        {
            yield return new WaitForSeconds(0.5f);
        }
        headTarget = playerTransform.position;
        headAttached = false;
        startPosition = headTransform.position;
        clockDir = clockwiseFactor;
        targetReached = false;
        currentDistanceTraveled = 0f;
        targetReached = false;
        directionInitial = headTarget - startPosition;
        directionInitial.Normalize();
        perpendicularDirection = new Vector2(-directionInitial.y, directionInitial.x) * clockDir;

    }
    Vector2 CalcParabolePosition()
    {
        Vector2 pos;
        float distanceToTarget = Vector2.Distance(headTarget, startPosition);
        // Calculate the current position on the parabola
        pos = startPosition + (directionInitial * currentDistanceTraveled);
        pos += perpendicularDirection * steepness * currentDistanceTraveled * (distanceToTarget - currentDistanceTraveled);

        // Update the distance traveled
        currentDistanceTraveled += headSpeed * Time.fixedDeltaTime;
        if (currentDistanceTraveled > distanceToTarget)
        {
            targetReached = true;
            currentDistanceTraveled = 0f;
        }
        headMovement = pos - (Vector2)headTransform.position;
        return pos;
    }

    Vector2 CalcReturnToBodyPosition()
    {
        Vector2 pos;
        if (Vector3.Distance(headTransform.position, HeadOnShouldersPosition()) < 0.1)
        {
            headAttached = true;
            headSpinning = false;
            targetReached = false;
            if (currentTikiState == TikiState.Circle)
            {
                StartCoroutine(SpinningHeadCooldown());
            }
            return (Vector2)headTransform.position;
        }
        headMovement.Normalize();
        headMovement += (Vector2)(HeadOnShouldersPosition() - headTransform.position).normalized * correctionFactor;
        headMovement.Normalize();
        headMovement *= headSpeed * Time.fixedDeltaTime;
        pos = (Vector2)headTransform.position + headMovement;
        return pos;
    }
    public void OnCircleTiki()
    {
        if (hit)
        {
            SwitchClockWise();
            //range = Mathf.Clamp(range + Random.Range(-1, 1), minimalRange, maximalRange);
        }
        bodyMovement = Quaternion.Euler(0f, 0f, clockwiseFactor * 90f) * (playerTransform.position - bodyTransform.position);
        if (currentRangeState == RangeState.Far)
        {
            bodyMovement.x += playerTransform.position.x - bodyTransform.position.x;
            bodyMovement.y += playerTransform.position.y - bodyTransform.position.y;
        }
        else if (currentRangeState == RangeState.Close)
        {
            bodyMovement.x += -playerTransform.position.x + bodyTransform.position.x;
            bodyMovement.y += -playerTransform.position.y + bodyTransform.position.y;
        }

    }
    public void OnComeBack()
    {
        if (currentRangeState == RangeState.Far)
        {
            NavigateToPlayer();
            //bodyMovement = playerTransform.position - bodyTransform.position;
        }
        else if (currentRangeState == RangeState.Close || currentRangeState == RangeState.InRange)
        {
            ChangeTikiState(TikiState.Circle);
        }
    }
    public void NavigateToPlayer()
    {
        Vector2 sight = pathfinder.CheckPlayerInSight(GetFeet(), AggroRange);
        if (sight != Vector2.zero)
        {
            bodyMovement = sight;
            return;
        }

        Vector2 reachable = pathfinder.CheckPlayerReachable(bodyTransform.position, AggroRange);
        if (reachable != Vector2.zero)
        {
            bodyMovement = reachable;
        }
    }
    Vector2 GetFeet()
    {
        return new Vector2(bodyTransform.position.x, bodyCollider2D.bounds.min.y);
    }
    public void DoMovement()
    {
        bodyMovement.Normalize();
        // Moves the Rigidbody2D based on the current movement vector and speed.
        float comeBackFactor = 1f;
        if (currentTikiState == TikiState.ComeBack)
        {
            comeBackFactor = 1.5f;
        }
        Vector2 moving_by = bodyMovement * GetWalkSpeed() * Time.fixedDeltaTime * comeBackFactor;
        rbBody.MovePosition(rbBody.position + moving_by);

        // check hit
        float rayLength = 0.5f;
        LayerMask mask = LayerMask.GetMask("Player", "Walls");
        hit = Physics2D.Raycast(bodyTransform.position, bodyMovement, rayLength, mask);
        Debug.DrawRay(bodyTransform.position, bodyMovement, Color.cyan, 1);

        if (moving_by.magnitude > 0.01)
        {
            bodyAnimator.SetBool("Walk", true);
        }
        else
        {
            bodyAnimator.SetBool("Walk", false);
        }
    }

    public void CalcRangeState()
    {
        if (playerTransform != null)
        {
            float dist = Mathf.Abs((playerTransform.position - bodyTransform.position).magnitude);
            float error = Mathf.Abs(dist - range) - rangeTolerance;
            if (dist > range && error > 0)
            {
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

    public void OnIdleStatus()
    {

    }
    /// <summary>
    /// Abstract method to define behavior based on the enemy's health.
    /// </summary>
    public override void ActOnHealth()
    {
        if (health.IsZero())
        {
            DeathEffect.SpawnDeathEffect(deathEffect, bodyTransform.position, deathEffectDelay);
            Destroy(gameObject, 0);
        }
        else if (!speedBuffGiven && health.GetHealth() < health.GetMaxHealth() / 2)
        {
            BuffDebuffManager.Instance.ApplyBuffDebuff(gameObject, "TikiPanicSpeed", BuffDebuffManager.BuffDebuffType.SpeedBuff, 120f, 2, false);
        }
        else if (health.GetHealth() < health.GetMaxHealth() / 5)
        {
            ChangeTikiState(TikiState.Panic);
        }
    }

    /// <summary>
    /// Abstract method to define behavior based on the enemy's mana.
    /// </summary>
    public override void ActOnMana()
    { }
    public override void GetKnocked(float knockTime)
    {
        StartCoroutine(HandleStagger(knockTime));
    }
    private IEnumerator HandleStagger(float staggerTime)
    {
        ChangeState(EnemyStatus.Staggered);
        skirtAnimator.SetTrigger("Stagger");
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
    public Vector2 GetMovement()
    {
        return bodyMovement;
    }
    public float GetWalkSpeed()
    {
        return moveSpeed;
    }
    public void SwitchClockWise()
    {
        clockwiseFactor *= -1;
        directionChangeCounter++;
        /*
        if (currentTikiState == TikiState.Circle && directionChangeCounter > directionChangeStop && Random.Range(0f, 1f) > 0.3)
        {
            ChangeTikiState(TikiState.ComeBack);
        }
        */
    }
}
