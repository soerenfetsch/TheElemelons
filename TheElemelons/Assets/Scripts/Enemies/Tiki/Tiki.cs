using System.Collections;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter.Xml;
using Codice.CM.Client.Differences;
using Ionic.BZip2;
using JetBrains.Annotations;
using UnityEngine;


public enum TikiState
{
    HealFriends,
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
    public float noHeadVulnerabilityFactor = 2f;
    public TikiState currentTikiState;
    Animator bodyAnimator;
    [Header("Spells")]
    public GameObject greenAttackGameobject;
    public GameObject healGameobject;
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
    public int clockwiseFactor = 1;
    public float moveSpeed = 1f;
    Rigidbody2D rbBody;
    int directionChangeCounter = 0;
    public int directionChangeStop = 5;
    public float panicWalkSpeedFactor = 1.5f;
    [Header("CombatRangeHandling")]
    public float range = 10f;
    public float aggroRange = 30f;
    public float minimalRange = 6f;
    public float maximalRange = 18f;
    public float rangeTolerance = 1f;
    public RangeState currentRangeState;
    [Header("Combat")]
    public float baseDamage = 1f;
    public float thrust = 1f;
    public float knockTime = 0.3f;
    FlashEffect flashEffectBody;
    FlashEffect flashEffectSkirt;
    public float flashDuration = 0.2f;

    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;
    [Header("LastStand Attack stats")]
    public int rounds = 4;
    public float timeBeforeStarts = 1f;
    public float timeBetweenShoot = 2f;
    [Header("Bools")]
    bool speedBuffGiven = false;
    bool panic = false;
    bool doingLastStand = false;
    [Header("Cooldowns")]
    bool canThrowHead = false;
    public float spinningHeadCooldown = 2.5f;
    bool canCastGreenAttack = true;
    public float greenAttackCooldown = 0.8f;
    bool canCastHeal = true;
    public float healCooldown = 7f;
    bool canCastTikiLastStand = true;
    public float tikiLastStandCooldown = 4f;
    [Header("HealFriendsStats")]
    public float healDetectRange = 15f;
    public float healingRange = 30f;
    Vector2 friendsCenter;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        if (playerTransform == null)
        {
            Debug.Log("Tiki couldn't find the player at Start(), please fix");
        }
        bodyAnimator = transform.GetChild(0).GetComponent<Animator>();
        bodyTransform = transform.GetChild(0).transform;
        skirtAnimator = transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
        mana = GetComponent<Mana>();
        health = transform.GetChild(0).GetComponent<Health>();
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
        elementType = ElementType.Earth;
        attackFactor = 1f;
        defenseFactor = 1f;
        bodyCollider2D = transform.GetChild(0).GetComponent<BoxCollider2D>();
        flashEffectBody = bodyTransform.GetComponent<FlashEffect>();
        flashEffectSkirt = bodyTransform.GetChild(0).GetComponent<FlashEffect>();
    }
    void FixedUpdate()
    {
        if (CanAct())
        {
            ActOnStatus();
        }
        HandleHead();
        DoMovementWithChecks();
    }
    /// <summary>
    /// if head is attached, should follow movement of body
    /// updates rotation if spinning
    /// if not attached, navigate head to initial player position in a parabola or back to tiki afterwards
    /// </summary>
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
    /// <summary>
    /// Calculate where head should be if attached to body
    /// </summary>
    Vector3 HeadOnShouldersPosition()
    {
        return bodyTransform.position + new Vector3(0, 1.75f, 0);
    }
    /// <summary>
    /// call methods that correspond to current state, tikistate, range and health
    /// </summary>
    public override void ActOnStatus()
    {
        ActOnHealth();
        CalcRangeState();
        if (currentRangeState == RangeState.OutOfRange)
        {
            ChangeState(EnemyStatus.Idle);
        }
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
        else if (currentTikiState == TikiState.HealFriends)
        {
            OnHealFriends();
        }
    }
    /// <summary>
    /// tiki navigates to average location of fellow enemies to be healed. 
    /// stops moving if destination is reached, casts healing spell and switches back into circle tikistate
    /// </summary>
    public void OnHealFriends()
    {
        movement = friendsCenter - (Vector2)bodyTransform.position;
        if (Mathf.Abs(movement.magnitude) < 0.2f)
        {
            movement = Vector2.zero;
            DoHeal();
            ChangeTikiState(TikiState.Circle);
        }
    }
    /// <summary>
    /// cast heal to surrounding enemies by instantiating the spell at each enemy location. resets cooldown.
    /// cant heal tikis
    /// </summary>
    public void DoHeal()
    {
        canCastHeal = false;

        LayerMask friendsMask = LayerMask.GetMask("Enemies");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bodyTransform.position, healingRange, friendsMask);
        foreach (Collider2D collider in colliders)
        {
            Health friendsHealth = collider.gameObject.GetComponent<Health>();
            if (friendsHealth != null && friendsHealth.transform.GetComponent<TikiBody>() == null)
            {
                CombatManager.InstantiateSpell(healGameobject, gameObject, friendsHealth.transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
        StartCoroutine(HealCooldown());
    }
    /// <summary>
    /// cooldown for the heal friends spell
    /// </summary>
    IEnumerator HealCooldown()
    {
        yield return new WaitForSeconds(healCooldown);
        canCastHeal = true;
    }
    /// <summary>
    /// method to enforce state transitions 
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        directionChangeCounter = 0;
        currentState = newState;
    }
    /// <summary>
    /// method to enforce tiki state transitions 
    /// </summary>
    /// <param name="newState">The new tiki state to transition to.</param>
    public void ChangeTikiState(TikiState newState)
    {
        currentTikiState = newState;
        if (newState == TikiState.Circle)
        {
            StartCoroutine(SpinningHeadCooldown());
        }
    }
    /// <summary>
    /// cooldown for the spinning head attack
    /// </summary>
    IEnumerator SpinningHeadCooldown()
    {
        yield return new WaitForSeconds(spinningHeadCooldown);
        canThrowHead = true;
    }
    /// <summary>
    /// method to begin the spinning head attack
    /// head begins spinning for short time, then detaches from body
    /// the head sets the players position at the moment the head detaches as its target (so no seeking)
    /// flies clockwise if tiki was circling clockwise at that moment, else anticlockwise
    /// </summary>
    IEnumerator StartSpinningAttack(bool linger = true)
    {
        canThrowHead = false;
        headSpinning = true;
        if (linger)
        {
            yield return new WaitForSeconds(0.5f);
        }
        headTarget = playerTransform.position;
        DetachHead();
        startPosition = headTransform.position;
        clockDir = clockwiseFactor;
        targetReached = false;
        currentDistanceTraveled = 0f;
        targetReached = false;
        directionInitial = headTarget - startPosition;
        directionInitial.Normalize();
        perpendicularDirection = new Vector2(-directionInitial.y, directionInitial.x) * clockDir;

    }
    /// <summary>
    /// navigates flying head to target position in a parabole shape
    /// only executed if target not reached yet
    /// </summary>
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
    /// <summary>
    /// cooldown for green spell
    /// </summary>
    IEnumerator GreenAttackCooldown()
    {
        canCastGreenAttack = false;
        yield return new WaitForSeconds(greenAttackCooldown);
        canCastGreenAttack = true;
    }
    /// <summary>
    /// called if flying head already reached its target
    /// calculate next position of flying head to navigate back to tiki.
    /// head reattaches to tiki when reached
    /// </summary>
    Vector2 CalcReturnToBodyPosition()
    {
        Vector2 pos;
        if (Vector3.Distance(headTransform.position, HeadOnShouldersPosition()) < 0.1)
        {
            AttachHead();
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
    /// <summary>
    /// cooldown for the last stand spell
    /// </summary>
    IEnumerator TikiLastStandCooldown()
    {
        yield return new WaitForSeconds(tikiLastStandCooldown);
        canCastTikiLastStand = true;
    }
    /// <summary>
    /// executes last stand spell. head spins, spinspeed increases, repeatedly casts green spells in 4 equidistant directions.
    /// cooldown is then reset.
    /// </summary>
    IEnumerator DoTikiLastStand()
    {
        //prepare
        canCastTikiLastStand = false;
        doingLastStand = true;
        headSpinning = true;
        headSpinSpeed *= 2;
        yield return new WaitForSeconds(timeBeforeStarts);
        //shoot
        Vector3 castDir = new Vector3(0.75f, 0, 0);
        for (int j = 0; j < rounds; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 castpoint = bodyTransform.position + castDir;
                castpoint.y += 1f;
                CombatManager.InstantiateSpell(greenAttackGameobject, gameObject, castpoint, Quaternion.Euler(castDir.x, 0, 0));
                castDir = Quaternion.Euler(0, 0, 90) * castDir;
            }
            castDir = Quaternion.Euler(0, 0, 45) * castDir;
            yield return new WaitForSeconds(timeBetweenShoot);
        }
        //end
        headSpinSpeed /= 2;
        doingLastStand = false;
        StartCoroutine(TikiLastStandCooldown());

    }
    /// <summary>
    /// decides on what to do when tiki is in the circle state
    /// rotate around player
    /// switch rotation direction if hits a wall and vary range randomly
    /// if any attack/special move is available, execute it
    /// </summary>
    public void OnCircleTiki()
    {
        // handle attacks
        if (panic && canCastTikiLastStand && headAttached)
        {
            StartCoroutine(DoTikiLastStand());
        }
        else if (canThrowHead && currentRangeState != RangeState.Close && !doingLastStand)
        {
            StartCoroutine(StartSpinningAttack());
        }
        else if (canCastGreenAttack && currentRangeState == RangeState.Close && !doingLastStand)
        {
            StartCoroutine(GreenAttackCooldown());
            Vector3 castpoint = bodyTransform.position;
            castpoint.y += 1f;
            castpoint += (playerTransform.position - castpoint).normalized * 0.75f;
            CombatManager.InstantiateSpell(greenAttackGameobject, gameObject, castpoint, Quaternion.Euler(0, 0, 0));
        }
        // handle collision
        if (hit)
        {
            SwitchClockWise();
            range = Mathf.Clamp(range + Mathf.Sign(Random.Range(-1, 1)), minimalRange, maximalRange);
        }
        // handle movement
        movement = Quaternion.Euler(0f, 0f, clockwiseFactor * 90f) * (playerTransform.position - bodyTransform.position);
        if (currentRangeState == RangeState.Far)
        {
            movement.x += playerTransform.position.x - bodyTransform.position.x;
            movement.y += playerTransform.position.y - bodyTransform.position.y;
            if (Mathf.Abs((bodyTransform.position - playerTransform.position).magnitude) > range * 1.5)
            {
                ChangeTikiState(TikiState.ComeBack);
            }
        }
        else if (currentRangeState == RangeState.Close)
        {
            movement.x += -playerTransform.position.x + bodyTransform.position.x;
            movement.y += -playerTransform.position.y + bodyTransform.position.y;
        }

    }
    /// <summary>
    /// handle tiki movement if in comeback state
    /// navigate tiki to player via pathfinder
    /// </summary>
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
    /// <summary>
    /// update movement to navigate tiki to player via pathfinder
    /// </summary>
    public void NavigateToPlayer()
    {
        Vector2 sight = pathfinder.CheckPlayerInSight(GetFeet(), aggroRange);
        if (sight != Vector2.zero)
        {
            movement = sight;
            return;
        }

        Vector2 reachable = pathfinder.CheckPlayerReachable(bodyTransform.position, aggroRange);
        if (reachable != Vector2.zero)
        {
            movement = reachable;
        }
    }
    /// <summary>
    /// calculate location of tikis feet
    /// </summary>
    Vector2 GetFeet()
    {
        return new Vector2(bodyTransform.position.x, bodyCollider2D.bounds.min.y);
    }
    /// <summary>
    /// execute movement by reading the bodyMovement vector. also send aout a raycast in the movement direction
    /// checks if can switch into healfriends state and does so if possible
    /// </summary>
    public override void DoMovement()
    {
        movement.Normalize();
        // Moves the Rigidbody2D based on the current movement vector and speed.
        float comeBackFactor = 1f;
        if (currentTikiState == TikiState.ComeBack)
        {
            comeBackFactor = 1.25f;
        }
        Vector2 moving_by = movement * GetWalkSpeed() * Time.fixedDeltaTime * comeBackFactor;
        rbBody.MovePosition(rbBody.position + moving_by);

        // check hit
        float rayLength = 0.5f;
        LayerMask mask = LayerMask.GetMask("Player", "Walls");
        hit = Physics2D.Raycast(bodyTransform.position, movement, rayLength, mask);

        if (moving_by.magnitude > 0.01)
        {
            bodyAnimator.SetBool("Walk", true);
        }
        else
        {
            bodyAnimator.SetBool("Walk", false);
        }

        // check friends
        if (!panic && canCastHeal && (headTransform.position - bodyTransform.position).magnitude < 4f &&
        currentState != EnemyStatus.Staggered && currentTikiState != TikiState.HealFriends)
        {
            LayerMask friendsMask = LayerMask.GetMask("Enemies");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(bodyTransform.position, healDetectRange, friendsMask);
            Vector3 goal = new Vector3(0, 0, 0);
            int validColliderCounter = 0;
            foreach (Collider2D collider in colliders)
            {
                Health friendsHealth = collider.gameObject.GetComponent<Health>();
                if (friendsHealth != null && (friendsHealth.GetHealth() <= friendsHealth.GetMaxHealth() / 5 ||
                (friendsHealth.GetMaxHealth() >= 3f && friendsHealth.GetHealth() <= friendsHealth.GetMaxHealth() / 2)) &&
                friendsHealth.transform.GetComponent<TikiBody>() == null)
                {
                    goal += collider.transform.position;
                    validColliderCounter++;
                }
            }
            goal /= validColliderCounter;
            if (validColliderCounter > 0)
            {
                friendsCenter = goal;
                ChangeTikiState(TikiState.HealFriends);
            }
        }
    }
    public override void OnFreeze(bool freezeOn = true)
    {
        if (freezeOn)
        {
            movement = Vector2.zero;
            bodyAnimator.speed = 0;
            skirtAnimator.speed = 0;
        }
        else
        {
            bodyAnimator.speed = 1;
            skirtAnimator.speed = 1;
        }
    }
    /// <summary>
    /// determines rangestate based on tiki body's distance from player
    /// </summary>
    public void CalcRangeState()
    {
        if (playerTransform != null)
        {
            float dist = Mathf.Abs((playerTransform.position - bodyTransform.position).magnitude);
            if (dist > aggroRange)
            {
                currentRangeState = RangeState.OutOfRange;
                return;
            }
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
    /// <summary>
    /// do nothing if idle
    /// check if not out of range anymore and switch into enemystate approaching
    /// </summary>
    public void OnIdleStatus()
    {
        if (currentRangeState != RangeState.OutOfRange)
        {
            ChangeState(EnemyStatus.Approaching);
        }
    }
    /// <summary>
    /// define behavior based on the enemy's health.
    /// health = 0 tiki dies
    /// health below maxhealth/2 movespeed increases
    /// health below maxhealth/5 tiki is in panic, meaning last stand can now also be cast
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
            BuffDebuffManager.Instance.ApplyBuffDebuff(gameObject, "TikiPanicSpeed", BuffDebuffManager.BuffDebuffType.SpeedBuff, 120f, panicWalkSpeedFactor, false);
            speedBuffGiven = true;
        }
        else if (health.GetHealth() < health.GetMaxHealth() / 3)
        {
            panic = true;
        }
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
        skirtAnimator.SetTrigger("Stagger");
        flashEffectBody?.Flash(flashDuration);
        flashEffectSkirt?.Flash(flashDuration);
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
        return movement;
    }
    public float GetWalkSpeed()
    {
        return moveSpeed;
    }
    /// <summary>
    /// invert tikis circling direction
    /// counts how often this has happened
    /// after a set amount of time, tiki can randomly switch to comeback state and the counter is reset
    /// </summary>
    public void SwitchClockWise()
    {
        clockwiseFactor *= -1;
        directionChangeCounter++;
        if (currentTikiState == TikiState.Circle && directionChangeCounter > directionChangeStop && Random.Range(0f, 1f) < 0.2)
        {
            ChangeTikiState(TikiState.ComeBack);
            directionChangeCounter = 0;
        }
    }
    public bool IsHeadSpinning()
    {
        return headSpinning;
    }
    public bool IsHeadAttached()
    {
        return headAttached;
    }
    public bool IsTargetReached()
    {
        return targetReached;
    }
    public void AttachHead()
    {
        headAttached = true;
    }
    public void DetachHead()
    {
        headAttached = false;
    }
    new protected bool ObstructedSightCantSeePlayer()
    {
        return isSightObstructed && (playerTransform.position - bodyTransform.position).magnitude > obstructedSightRange;
    }
}
