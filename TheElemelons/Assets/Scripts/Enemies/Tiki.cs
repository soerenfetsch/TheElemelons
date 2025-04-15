using System.Collections;
using System.Collections.Generic;
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
    public bool flowerSkirt = false;
    Animator bodyAnimator;
    Animator skirtAnimator;
    [Header("Head")]
    public Vector2 headMovement;
    bool headAttached = true;
    bool headSpinning = false;
    Rigidbody2D rbHead;
    Transform headTransform;
    [Header("BodyMovement")]
    RaycastHit2D hit;
    Transform bodyTransform;
    float clockwiseFactor = 1f;
    Vector2 bodyMovement;
    public float moveSpeed = 1f;
    Rigidbody2D rbBody;
    int directionChangeCounter = 0;
    public int directionChangeStop = 2;
    [Header("CombatRangeHandling")]
    float range;
    public float OutOfRange = 30f;
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
        rbHead = transform.GetChild(1).GetComponent<Rigidbody2D>();
        skirtAnimator.SetBool("Flowers", flowerSkirt);
        currentState = EnemyStatus.Approaching;
        range = minimalRange;
        headTransform = rbHead.transform;
    }
    // Update is called once per frame
    void Update()
    {
        ActOnStatus();
        if (headAttached)
        {
            headTransform.position = bodyTransform.position + new Vector3(0, 1f, 0);
        }
    }
    void FixedUpdate()
    {
        DoMovement();
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
    }
    public void OnCircleTiki()
    {
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
        if (hit)
        {
            SwitchClockWise();
            range += Mathf.Clamp(Random.Range(-1, 1), minimalRange, maximalRange);
        }
    }
    public void OnComeBack()
    {
        if (currentRangeState == RangeState.Far)
        {
            bodyMovement = playerTransform.position - bodyTransform.position;
        }
        else if (currentRangeState == RangeState.Close)
        {
            bodyMovement = -playerTransform.position + bodyTransform.position;
        }
        else if (currentRangeState == RangeState.InRange)
        {
            ChangeTikiState(TikiState.Circle);
        }
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
        Vector2 moving_by = GetMovement() * GetWalkSpeed() * Time.fixedDeltaTime * comeBackFactor;
        rbBody.MovePosition(rbBody.position + moving_by);
        float rayLength = 0.5f;
        LayerMask mask = LayerMask.GetMask("Player", "Walls");
        hit = Physics2D.Raycast(bodyTransform.position, bodyMovement, rayLength, mask);
        UnityEngine.Debug.DrawRay(bodyTransform.position, bodyMovement * rayLength, Color.yellow);

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
            DeathEffect.SpawnDeathEffect(deathEffect, transform.position, deathEffectDelay);
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
        if (currentTikiState == TikiState.Circle && directionChangeCounter > directionChangeStop && Random.RandomRange(0f, 1f) > 0.3)
        {
            ChangeTikiState(TikiState.ComeBack);
        }
    }
}
