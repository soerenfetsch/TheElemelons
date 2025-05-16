using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands.Merge.Restorer.Finder;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public enum WitchState
{
    Walk,
    Hover,
    Bald
}
public enum WitchMoves
{
    Dash,
    Medusa,
    Snek,
    Boulder,
    GreenPoof,
    RedPoof,
    Vortex,
    Offense,
    Circle,
    Evasion
}
public class Witch : Enemy
{
    public List<float> dice;
    int diceCount = 3;
    public float intervalThrowdice = 3f;
    public WitchState currentWitchState = WitchState.Walk;
    [Header("CombatRangeHandling")]
    public float range = 10f;
    public float aggroRange = 30f;
    public float minimalRange = 6f;
    public float maximalRange = 18f;
    public float rangeTolerance = 1f;
    RangeState currentRangeState = RangeState.OutOfRange;
    [Header("Buff/Debuff handling")]
    float moveFactor = 1f;
    [Header("Movement")]
    Vector2 randomupdownleftright;
    RaycastHit2D hit;
    int clockwiseFactor = 1;
    float circleProbability = 0.5f;
    Vector3 destination;
    List<Vector3> nodeCollection;
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
    [Header("Dash")]
    int dashMaxnumber = 3;
    float dashtime = 1;
    public float dashSpeed = 15f;
    bool doingDash = false;
    [Header("Spell- and MoveCooldowns, can and prob")]
    bool isCasting = false;
    public Dictionary<WitchMoves, WitchMoveInfos> moveStatsDict;
    float rechargeProb = 0.4f;
    float dashCooldown = 2f;
    float dashProbability = 0.5f;
    float medusaCooldown = 15f;
    float medusaProbability = 0.9f;
    public GameObject medusaGameObject;
    float offensiveSpellCooldown = 3f;
    float offensiveSpellProbability = 0.5f;
    float offensiveSpellCastTime = 1f;
    float evasionSpellCooldown = 10f;
    float evasionSpellProbability = 0.5f;
    float baldCooldown = 15f;
    float baldProbability = 0.85f;
    float baldCastTime = 1f;
    bool baldTimerisRunning = false;
    float redPoofProbability = 0.15f;
    public GameObject redPoofGameObject;
    float redPoofCooldown = 20f;
    float greenPoofProbability = 0.8f;
    public GameObject greenPoofGameObject;
    float vortexProbability = 0.5f;
    float boulderProbability = 0.25f;
    float boulderCastTime = 1.5f;

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
        InitializeNodes();
        InitilializeMoveStats();
        InitializeDice();
    }
    public void InitializeDice()
    {
        dice = new List<float>();
        for (int i = 0; i < diceCount; i++)
        {
            dice.Add(1f);
        }
    }
    public void InitilializeMoveStats()
    {
        //candash etc remove
        moveStatsDict = new Dictionary<WitchMoves, WitchMoveInfos>();
        //dash
        WitchMoveInfos dashInfos = new WitchMoveInfos(dashProbability, dashCooldown);
        moveStatsDict.Add(WitchMoves.Dash, dashInfos);
        //medusa
        WitchMoveInfos medusaInfos = new WitchMoveInfos(medusaProbability, medusaCooldown, castTime: 1.5f, moveGameObject: medusaGameObject);
        moveStatsDict.Add(WitchMoves.Medusa, medusaInfos);
        //snek
        WitchMoveInfos baldInfos = new WitchMoveInfos(baldProbability, baldCooldown, false, castTime: baldCastTime);
        moveStatsDict.Add(WitchMoves.Snek, baldInfos);
        //evasion
        WitchMoveInfos evasionSpellInfos = new WitchMoveInfos(evasionSpellProbability, evasionSpellCooldown);
        moveStatsDict.Add(WitchMoves.Evasion, evasionSpellInfos);
        //offensive
        WitchMoveInfos offensiveSpellInfos = new WitchMoveInfos(offensiveSpellProbability, offensiveSpellCooldown, castTime: offensiveSpellCastTime);
        moveStatsDict.Add(WitchMoves.Offense, offensiveSpellInfos);
        //circle
        WitchMoveInfos circleInfos = new WitchMoveInfos(circleProbability, 0);
        moveStatsDict.Add(WitchMoves.Circle, circleInfos);
        //redPoof
        WitchMoveInfos redPoofInfos = new WitchMoveInfos(redPoofProbability, redPoofCooldown, moveGameObject: greenPoofGameObject);
        moveStatsDict.Add(WitchMoves.GreenPoof, redPoofInfos);
        //greenPoof
        WitchMoveInfos greenPoofInfos = new WitchMoveInfos(greenPoofProbability, evasionSpellCooldown, moveGameObject: redPoofGameObject);
        moveStatsDict.Add(WitchMoves.RedPoof, greenPoofInfos);
        //vortex
        WitchMoveInfos vortexInfos = new WitchMoveInfos(vortexProbability, offensiveSpellCooldown, castTime: offensiveSpellCastTime);
        moveStatsDict.Add(WitchMoves.Vortex, vortexInfos);
        //boulder
        WitchMoveInfos boulderInfos = new WitchMoveInfos(boulderProbability, offensiveSpellCooldown, castTime: boulderCastTime);
        moveStatsDict.Add(WitchMoves.Boulder, boulderInfos);

    }
    void InitializeNodes()
    {
        nodeCollection = new List<Vector3>();
        GameObject nodeCollectionObject = GameObject.Find("NodeCollection");
        int childCount = nodeCollectionObject.transform.childCount;
        for (int child = 0; child < childCount; child++)
        {
            nodeCollection.Add(nodeCollectionObject.transform.GetChild(child).transform.position);
        }
    }
    bool ReadDice(WitchMoves move, int diceToRead = 0)
    {
        if (dice[diceToRead] < moveStatsDict[move].probability)
        {
            return true;
        }
        return false;
    }
    void SelectDashNode(float dashDistance, bool awayFromPlayer = false)
    {
        nodeCollection = nodeCollection.OrderBy(x => UnityEngine.Random.value).ToList<Vector3>();
        foreach (var node in nodeCollection)
        {
            Vector2 rayDir = node - transform.position;
            if (Mathf.Abs(rayDir.magnitude) < dashDistance)
            {
                float rayLength = dashDistance;
                LayerMask mask = LayerMask.GetMask("Player", "Walls");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, dashDistance, mask);
                if (!hit)
                {
                    if (!awayFromPlayer || Vector3.Angle(rayDir, playerTransform.position - transform.position) > 90f)
                    {
                        destination = node;
                        return;
                    }
                }
            }
        }
    }
    Vector3 SelectTPNode(float distance)
    {
        nodeCollection = nodeCollection.OrderBy(x => UnityEngine.Random.value).ToList<Vector3>();
        Vector3 choice = nodeCollection[0];
        float choiceDist = Mathf.Infinity;
        foreach (var node in nodeCollection)
        {
            float dist = Mathf.Abs((playerTransform.position - node).magnitude);
            if (Mathf.Abs(dist - distance) < choiceDist)
            {
                choiceDist = dist;
                choice = node;
            }
            if (dist < 2f)
            {
                return choice;
            }
        }
        return choice;
    }
    IEnumerator DoDash(float dashDistance, bool awayFromPlayer = false, int numberOfDashes = 1)
    {
        moveStatsDict[WitchMoves.Dash].canDo = false;
        doingDash = true;
        for (int i = 0; i < numberOfDashes; i++)
        {
            SelectDashNode(dashDistance, awayFromPlayer);
            movement = destination - transform.position;
            yield return new WaitForSeconds(dashtime);
        }
        doingDash = false;
        StartCoroutine(DoCooldown(WitchMoves.Dash));
    }
    IEnumerator DoCooldown(WitchMoves move)
    {
        float rechargeDice = 1f;
        yield return new WaitForSeconds(moveStatsDict[move].cooldown);
        while (!(rechargeDice < rechargeProb))
        {
            rechargeDice = UnityEngine.Random.Range(0, 1);
            yield return new WaitForSeconds(1f);
        }
        moveStatsDict[move].canDo = true;
    }
    void FixedUpdate()
    {
        ThrowDice();
        ActOnStatus();
        DoMovement();
    }
    void Update()
    {
        ManageBodyAnimators();
    }
    void ThrowDice()
    {
        for (int i = 0; i < diceCount; i++)
        {
            if (Time.frameCount % intervalThrowdice * (i + 1) == 0)
            {
                dice[i] = UnityEngine.Random.Range(0, 1);
            }
        }
    }
    public WitchMoves Chose(List<WitchMoves> choices)
    {
        float sum = 0f;
        Dictionary<WitchMoves, float> bets = new Dictionary<WitchMoves, float>();
        foreach (WitchMoves move in choices)
        {

            Debug.Log(sum);
            sum += moveStatsDict[move].probability;
            Debug.Log(sum);
            bets.Add(move, sum);

        }
        float diceThrow = UnityEngine.Random.Range(0, sum);
        foreach (WitchMoves key in choices)
        {
            bets[key] = Mathf.Abs(bets[key] - diceThrow);
        }
        WitchMoves minKey = choices[0];
        float minValue = Mathf.Infinity; ;
        foreach (WitchMoves key in choices)
        {
            if (bets[key] < minValue)
            {
                minValue = bets[key];
                minKey = key;
            }
        }
        return minKey;
    }
    bool CanInput()
    {
        return !doingDash && !isCasting;
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
        if (!isCasting)
        {
            movement.Normalize();
            Vector2 moving_by = movement * GetMoveSpeed() * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moving_by);
        }

        // check hit
        float rayLength = 0.5f;
        LayerMask mask = LayerMask.GetMask("Player", "Walls");
        hit = Physics2D.Raycast(transform.position, movement, rayLength, mask);
    }
    Vector2 CalcCircleDir()
    {
        return (Vector2)(Quaternion.Euler(0f, 0f, clockwiseFactor * 90f) * (Vector3)movement);
    }
    Vector2 PickStraightDir()
    {
        if (dice[2] < 0.25)
        {
            return Vector2.up * clockwiseFactor;
        }
        else if (dice[2] < 0.5)
        {
            return Vector2.right * clockwiseFactor;
        }
        else if (dice[2] < 0.75)
        {
            return Vector2.left * clockwiseFactor;
        }
        else
        {
            return Vector2.down * clockwiseFactor;
        }
    }
    public float GetMoveSpeed()
    {
        if (doingDash)
        {
            return dashSpeed;
        }
        if (currentWitchState == WitchState.Walk)
        {
            return moveSpeed * moveFactor;
        }
        else if (currentWitchState == WitchState.Hover || currentWitchState == WitchState.Bald)
        {
            return moveSpeed * hoverSpeedFactor * moveFactor;
        }
        return moveSpeed * moveFactor;
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
        else if (currentWitchState == WitchState.Walk && CanInput())
        {
            OnWalkStatus();
        }
        else if (currentWitchState == WitchState.Hover && CanInput())
        {
            OnHoverStatus();
        }
        else if (currentWitchState == WitchState.Bald && CanInput())
        {
            OnBaldStatus();
        }
    }
    public void OnWalkStatus()
    { }
    public void OnHoverStatus()
    {
        if (currentRangeState == RangeState.Close && CanInput())
        {
            if (moveStatsDict[WitchMoves.Medusa].canDo && ReadDice(WitchMoves.Medusa))
            {
                // Medusa
                HandleCast(WitchMoves.Medusa);
                StartCoroutine(DoMedusa());
            }
            else if (moveStatsDict[WitchMoves.Evasion].canDo && ReadDice(WitchMoves.Evasion) && !hairAnimator.GetBool("Medusa"))
            {
                List<WitchMoves> choices = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                foreach (WitchMoves move in choices)
                {
                    if (!moveStatsDict[move].canDo)
                    {
                        choices.Remove(move);
                    }
                }

                if (choices.Count > 0)
                {
                    WitchMoves choice = Chose(choices);
                    HandleCast(choice);
                    DoPoof(choice);
                }
            }
            else if (moveStatsDict[WitchMoves.Dash].canDo && ReadDice(WitchMoves.Dash))
            {
                // Dash
                float dashDistance = UnityEngine.Random.Range(-range, range) / 2 + range;
                int numberOfDashes = UnityEngine.Random.Range(1, dashMaxnumber + 1);
                StartCoroutine(DoDash(dashDistance, true, numberOfDashes));
            }
            else
            {
                // Run away
                movement = transform.position - playerTransform.position;
                if (hit)
                {
                    clockwiseFactor *= -1;
                }
                if (ReadDice(WitchMoves.Circle, 1))
                {
                    movement += CalcCircleDir();
                }
            }
        }
        else if (currentRangeState == RangeState.InRange && CanInput())
        {
            if (moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense))
            {
                List<WitchMoves> choices = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder };
                WitchMoves choice = Chose(choices);
                HandleCast(choice);
                // cast and stand still
            }
            else if (moveStatsDict[WitchMoves.Evasion].canDo && ReadDice(WitchMoves.Evasion)) // check no wall behind player 
            {
                List<WitchMoves> choices = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                WitchMoves choice = Chose(choices);
                HandleCast(choice);
                // cast
            }
            else
            {
                if (hit)
                {
                    clockwiseFactor *= -1;
                }
                if (ReadDice(WitchMoves.Circle, 1))
                {
                    movement = CalcCircleDir();
                }
                else
                {
                    movement = PickStraightDir();
                }
            }
        }
        else if (currentRangeState == RangeState.Far && CanInput())
        {
            if (moveStatsDict[WitchMoves.Snek].canDo && ReadDice(WitchMoves.Snek))
            {
                // go bald
            }
            else if (moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense))
            {
                // cast offense
            }
            else
            {

                if (hit)
                {
                    clockwiseFactor *= -1;
                }
                if (ReadDice(WitchMoves.Circle, 1))
                {
                    movement = CalcCircleDir();
                }
                else
                {
                    movement = PickStraightDir();
                }
            }
        }
    }
    public void DoPoof(WitchMoves poof)
    {
        if (poof != WitchMoves.GreenPoof && poof != WitchMoves.RedPoof)
        {
            return;
        }
        // select tp point
        transform.position = SelectTPNode(UnityEngine.Random.Range(5f, 10f));
        Instantiate(moveStatsDict[poof].moveGameObject, playerTransform.position, Quaternion.Euler(0f, 0f, 0f));
        moveStatsDict[WitchMoves.Evasion].canDo = false;
        moveStatsDict[poof].canDo = false;
        StartCoroutine(DoCooldown(poof));
        StartCoroutine(DoCooldown(WitchMoves.Evasion));
    }
    public IEnumerator DoMedusa()
    {
        moveStatsDict[WitchMoves.Medusa].canDo = false;
        hairAnimator.SetBool("Medusa", true);
        Instantiate(moveStatsDict[WitchMoves.Medusa].moveGameObject, playerTransform.position, Quaternion.Euler(0f, 0f, 0f));
        yield return new WaitForSeconds(medusaTime);
        hairAnimator.SetBool("Medusa", false);
        StartCoroutine(DoCooldown(WitchMoves.Medusa));
    }
    IEnumerator HandleCast(WitchMoves spell)
    {
        movement = playerTransform.position - transform.position;
        if (moveStatsDict[spell].castTime > 0f)
        {
            isCasting = true;
            wandAnimator.SetBool("WandOut", true);
            yield return new WaitForSeconds(moveStatsDict[spell].castTime);
            wandAnimator.SetBool("WandOut", false);
            isCasting = false;
        }
        else
        {
            wandAnimator.SetBool("WandOut", true);
            yield return new WaitForSeconds(1f);
            wandAnimator.SetBool("WandOut", false);
        }
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
            moveStatsDict[WitchMoves.Snek].canDo = true;
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
        moveFactor *= factor;
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
public class WitchMoveInfos
{
    public float probability;
    public float cooldown;
    public bool canDo;
    public int dice;
    public float castTime;
    public GameObject moveGameObject;
    public WitchMoveInfos(float probability, float cooldown, bool canDo = true, int dice = 0, float castTime = 0, GameObject moveGameObject = null)
    {
        this.probability = probability;
        this.cooldown = cooldown;
        this.canDo = canDo;
        this.dice = dice;
        this.castTime = castTime;
        this.moveGameObject = moveGameObject;
    }
}