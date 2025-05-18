using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands.Merge.Restorer.Finder;
using log4net.Config;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
public enum WitchState
{
    Walk,
    Hover,
    Bald
}
public enum WitchMoves
{
    AnyMove,
    SpecialMove,
    Dash,
    Medusa,
    Snek,
    Boulder,
    GreenPoof,
    RedPoof,
    Vortex,
    Offense,
    Circle,
    Evasion,
    RamPlayer,
    None
}
public class Witch : Enemy
{
    public List<float> dice;
    int diceCount = 3;
    public float intervalThrowdice = 3f;
    public WitchState currentWitchState = WitchState.Walk;
    [Header("Health phases")]
    int healthPhase = 1;
    [Header("CombatRangeHandling")]
    public float range = 10f;
    public float aggroRange = 80f;
    public float minimalRange = 6f;
    public float maximalRange = 18f;
    public float rangeTolerance = 1f;
    RangeState currentRangeState = RangeState.OutOfRange;
    [Header("Buff/Debuff handling")]
    float moveFactor = 1f;
    [Header("Movement")]
    RaycastHit2D hit;
    int clockwiseFactor = 1;
    int clockSwitchCounter = 0;
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
    BoxCollider2D solidCollider;
    [Header("Medusa")]
    public float medusaTime = 3f;
    [Header("Boulder")]
    public float boulderDamage = 2f;
    public float boulderThrust = 6f;
    public float boulderKnockTime = 1.5f;
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
    float medusaCooldown = 20f;
    float medusaProbability = 0.9f;
    public GameObject medusaGameObject;
    float offensiveSpellCooldown = 3f;
    float offensiveSpellProbability = 0.5f;
    float offensiveSpellCastTime = 1.5f;
    float evasionSpellCooldown = 7f;
    float evasionSpellProbability = 0.5f;
    float baldCooldown = 15f;
    float baldProbability = 0.85f;
    float baldCastTime = 2f;
    bool baldTimerisRunning = false;
    float redPoofProbability = 0.15f;
    public GameObject redPoofGameObject;
    float redPoofCooldown = 20f;
    float greenPoofProbability = 0.8f;
    float greenPoofCooldown = 10f;
    public GameObject greenPoofGameObject;
    float vortexProbability = 0.3f;
    float vortexCooldown = 10f;
    public GameObject vortexGameObject;
    float boulderProbability = 0.4f;
    float boulderCastTime = 1.5f;
    public GameObject boulderGameObject;
    float specialMoveProbability = 0.8f;
    float specialMoveCooldown = 1f;
    float ramPlayerProbability = 0.5f;
    float ramPlayerCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        attackFactor = 1;
        defenseFactor = 1;
        playerTransform = GameObject.Find("Player").transform;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = GetComponent<Animator>();
        hairAnimator = transform.GetChild(0).GetComponent<Animator>();
        wandAnimator = transform.GetChild(1).GetComponent<Animator>();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
        solidCollider = GetComponent<BoxCollider2D>();
        movement = new Vector2(0, 0);
        InitializeNodes();
        InitilializeMoveStats();
        InitializeDice();
    }
    public void InitializeDice()
    {
        dice = new List<float>();
        for (int i = 0; i < 2 * diceCount; i++)
        {
            dice.Add(1f);
        }
    }
    public void InitilializeMoveStats()
    {
        //candash etc remove
        moveStatsDict = new Dictionary<WitchMoves, WitchMoveInfos>();
        // anymove
        WitchMoveInfos anyMoveInfos = new WitchMoveInfos(1, 0);
        moveStatsDict.Add(WitchMoves.AnyMove, anyMoveInfos);
        // specialmove
        WitchMoveInfos specialMoveInfos = new WitchMoveInfos(specialMoveProbability, specialMoveCooldown);
        moveStatsDict.Add(WitchMoves.SpecialMove, specialMoveInfos);
        //dash
        WitchMoveInfos dashInfos = new WitchMoveInfos(dashProbability, dashCooldown, doMethod: DoDash);
        moveStatsDict.Add(WitchMoves.Dash, dashInfos);
        //medusa
        WitchMoveInfos medusaInfos = new WitchMoveInfos(medusaProbability, medusaCooldown, castTime: 1.5f, moveGameObject: medusaGameObject, doMethod: DoMedusa, parentCategory: WitchMoves.SpecialMove);
        moveStatsDict.Add(WitchMoves.Medusa, medusaInfos);
        //snek
        WitchMoveInfos baldInfos = new WitchMoveInfos(baldProbability, baldCooldown, false, castTime: baldCastTime, parentCategory: WitchMoves.SpecialMove);
        moveStatsDict.Add(WitchMoves.Snek, baldInfos);
        //evasion
        WitchMoveInfos evasionSpellInfos = new WitchMoveInfos(evasionSpellProbability, evasionSpellCooldown, parentCategory: WitchMoves.SpecialMove);
        moveStatsDict.Add(WitchMoves.Evasion, evasionSpellInfos);
        //offensive
        WitchMoveInfos offensiveSpellInfos = new WitchMoveInfos(offensiveSpellProbability, offensiveSpellCooldown, castTime: offensiveSpellCastTime, parentCategory: WitchMoves.SpecialMove);
        moveStatsDict.Add(WitchMoves.Offense, offensiveSpellInfos);
        //circle
        WitchMoveInfos circleInfos = new WitchMoveInfos(circleProbability, 0);
        moveStatsDict.Add(WitchMoves.Circle, circleInfos);
        //redPoof
        WitchMoveInfos redPoofInfos = new WitchMoveInfos(redPoofProbability, redPoofCooldown, moveGameObject: redPoofGameObject, doMethod: DoRedPoof, parentCategory: WitchMoves.Evasion);
        moveStatsDict.Add(WitchMoves.RedPoof, redPoofInfos);
        //greenPoof
        WitchMoveInfos greenPoofInfos = new WitchMoveInfos(greenPoofProbability, greenPoofCooldown, moveGameObject: greenPoofGameObject, doMethod: DoGreenPoof, parentCategory: WitchMoves.Evasion);
        moveStatsDict.Add(WitchMoves.GreenPoof, greenPoofInfos);
        //vortex
        WitchMoveInfos vortexInfos = new WitchMoveInfos(vortexProbability, vortexCooldown, castTime: offensiveSpellCastTime, moveGameObject: vortexGameObject, doMethod: DoVortex, parentCategory: WitchMoves.Offense);
        moveStatsDict.Add(WitchMoves.Vortex, vortexInfos);
        //boulder
        WitchMoveInfos boulderInfos = new WitchMoveInfos(boulderProbability, offensiveSpellCooldown, castTime: boulderCastTime, doMethod: DoBoulder, moveGameObject: boulderGameObject, parentCategory: WitchMoves.Offense);
        moveStatsDict.Add(WitchMoves.Boulder, boulderInfos);
        //ram
        WitchMoveInfos ramPlayerInfos = new WitchMoveInfos(ramPlayerProbability, ramPlayerCooldown, dice: diceCount, doMethod: DoRamPlayer);
        moveStatsDict.Add(WitchMoves.RamPlayer, ramPlayerInfos);
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
    bool ReadDice(WitchMoves move, int diceToRead = -1)
    {
        if ((diceToRead < 0 && moveStatsDict[move].dice < moveStatsDict[move].probability)
        || dice[diceToRead] < moveStatsDict[move].probability)
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
                RaycastHit2D hit1 = Physics2D.Raycast(transform.position, rayDir, dashDistance, mask);
                if (!hit1)
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
    Vector3 SelectRandomNode()
    {
        nodeCollection = nodeCollection.OrderBy(x => UnityEngine.Random.value).ToList<Vector3>();
        return nodeCollection[0];
    }
    IEnumerator DoDashRoutine(float dashDistance, bool awayFromPlayer = false, int numberOfDashes = 1)
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
        moveStatsDict[WitchMoves.Evasion].canDo = false;
    }
    void DoDash()
    {
        float dashDistance;
        int numberOfDashes;
        bool away;
        if (currentRangeState == RangeState.Far)
        {
            dashDistance = Mathf.Abs(UnityEngine.Random.Range(-range, range) + range);
            numberOfDashes = UnityEngine.Random.Range(1, dashMaxnumber + 2);
            away = false;
        }
        else
        {
            dashDistance = UnityEngine.Random.Range(-range, range) / 2 + range;
            numberOfDashes = UnityEngine.Random.Range(1, dashMaxnumber + 1);
            away = true;
        }

        StartCoroutine(DoDashRoutine(dashDistance, awayFromPlayer: away, numberOfDashes));
    }
    void DoGreenPoof()
    {
        DoPoof(WitchMoves.GreenPoof);
    }
    void DoRedPoof()
    {
        DoPoof(WitchMoves.RedPoof);
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
        NewHitRayCast();
        ThrowDice();
        CalcRangeState();
        ActOnStatus();
        DoMovement();
    }
    void Update()
    {
        ManageBodyAnimators();
    }
    void NewHitRayCast()
    {
        // check hit
        float rayLength = 2f;
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, 10f) * movement, rayLength, mask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, -10f) * movement, rayLength, mask);
        hit = hit0 ? hit0 : (hit1 ? hit1 : (hit2 ? hit2 : hit0));
    }
    void ThrowDice()
    {
        for (int i = 0; i < diceCount; i++)
        {
            if (Time.frameCount % intervalThrowdice * (i + 1) == 0)
            {
                dice[i] = UnityEngine.Random.Range(0, 1);
                dice[i + diceCount] = UnityEngine.Random.Range(0, 1);
            }
        }
    }
    public WitchMoves Chose(List<WitchMoves> choices)
    {
        float sum = 0f;
        int infBetCount = 0;
        Dictionary<WitchMoves, float> bets = new Dictionary<WitchMoves, float>();
        foreach (WitchMoves move in choices)
        {
            if (moveStatsDict[move].canDo && ReadDice(move))
            {
                sum += moveStatsDict[move].probability;
                bets.Add(move, sum);
            }
            else
            {
                bets.Add(move, Mathf.Infinity);
                infBetCount++;
            }
        }
        if (infBetCount < bets.Count)
        {
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
            ExecuteMove(minKey);
            return minKey;
        }
        return WitchMoves.None;
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
    }
    Vector2 CalcCircleDir()
    {
        return (Vector2)(Quaternion.Euler(0f, 0f, clockwiseFactor * 90f) * (playerTransform.position - transform.position));
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
        else if (moveStatsDict[WitchMoves.AnyMove].canDo)
        {
            if (currentWitchState == WitchState.Walk && CanInput())
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

    }
    WitchMoves ChoseSpellFromPriorityBatches(List<List<WitchMoves>> moveBatches, List<float> priorityFactors)
    {
        float sum = 0;
        int infBetCount = 0;
        List<float> bets = new List<float>();
        for (int i = 0; i < moveBatches.Count; i++)
        {
            int canDoCount = 0;
            for (int j = 0; j < moveBatches[i].Count; j++)
            {
                if (moveStatsDict[moveBatches[i][j]].canDo && ReadDice(moveBatches[i][j]))
                {
                    sum += moveStatsDict[moveBatches[i][j]].probability * priorityFactors[i];
                    canDoCount++;
                }
            }
            if (canDoCount > 0)
            {
                bets.Add(sum);
            }
            else
            {
                bets.Add(Mathf.Infinity);
                infBetCount++;
            }
        }
        WitchMoves chosen;
        if (infBetCount < priorityFactors.Count)
        {
            float diceThrow = UnityEngine.Random.Range(0, sum);
            for (int i = 0; i < bets.Count; i++)
            {
                bets[i] = Mathf.Abs(bets[i] - diceThrow);
            }
            List<WitchMoves> batchChoice = moveBatches[bets.IndexOf(bets.Min())];
            chosen = Chose(batchChoice);
        }
        else
        {
            chosen = WitchMoves.None;
        }
        return chosen;
    }
    void ExecuteMove(WitchMoves move)
    {
        if (move != WitchMoves.None && moveStatsDict[move].canDo && moveStatsDict[WitchMoves.AnyMove].canDo)
        {
            // reset candos and cooldowns
            moveStatsDict[move].canDo = false;
            StartCoroutine(DoCooldown(move));
            WitchMoves parentCategory = moveStatsDict[move].parentCategory;
            if (parentCategory != WitchMoves.AnyMove)
            {
                moveStatsDict[parentCategory].canDo = false;
                StartCoroutine(DoCooldown(parentCategory));
                WitchMoves grandparentCategory = moveStatsDict[parentCategory].parentCategory;
                if (grandparentCategory != WitchMoves.AnyMove)
                {
                    moveStatsDict[grandparentCategory].canDo = false;
                    StartCoroutine(DoCooldown(grandparentCategory));
                }
            }
            moveStatsDict[move].doMethod();
            StartCoroutine(HandleCast(move));
        }
    }
    public void OnWalkStatus()
    { }
    public void OnHoverStatus()
    {
        if (currentRangeState == RangeState.Close && CanInput())
        {
            if (moveStatsDict[WitchMoves.SpecialMove].canDo && ReadDice(WitchMoves.SpecialMove) &&
                ((moveStatsDict[WitchMoves.Medusa].canDo && ReadDice(WitchMoves.Medusa)) ||
                (moveStatsDict[WitchMoves.Evasion].canDo && ReadDice(WitchMoves.Evasion)) ||
                (moveStatsDict[WitchMoves.Dash].canDo && ReadDice(WitchMoves.Dash))))
            {
                // pick witchMove
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.Dash };
                List<WitchMoves> choices2 = new List<WitchMoves> { WitchMoves.Medusa };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1, choices2 };
                List<float> priorities = new List<float> { 1.5f, 1f, 2.5f };
                ChoseSpellFromPriorityBatches(choices, priorities);
            }
            else
            {
                // Run away
                movement = transform.position - playerTransform.position;
                if (hit)
                {
                    SwitchClockWise();
                }
                if (ReadDice(WitchMoves.Circle, 1))
                {
                    movement += CalcCircleDir();
                }
            }
        }
        else if (currentRangeState == RangeState.InRange && CanInput())
        {
            if (moveStatsDict[WitchMoves.SpecialMove].canDo && ReadDice(WitchMoves.SpecialMove) &&
                ((moveStatsDict[WitchMoves.Evasion].canDo && ReadDice(WitchMoves.Evasion)) ||
                (moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense))))
            {
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 3f, 1f };
                ChoseSpellFromPriorityBatches(choices, priorities);
            }
            else
            {
                if (hit)
                {
                    SwitchClockWise();
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
            if (moveStatsDict[WitchMoves.SpecialMove].canDo && ReadDice(WitchMoves.SpecialMove) &&
                ((moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense)) ||
                (moveStatsDict[WitchMoves.Snek].canDo && ReadDice(WitchMoves.Snek))))
            {
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.Snek };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 1f, 5f };
                ChoseSpellFromPriorityBatches(choices, priorities);
            }
            else
            {
                movement = playerTransform.position - transform.position;
                if (hit)
                {
                    SwitchClockWise();
                }
                if (ReadDice(WitchMoves.Circle, 2))
                {
                    movement += CalcCircleDir();
                }
                else
                {
                    movement += PickStraightDir();
                }
            }
        }
    }
    void SwitchClockWise()
    {
        clockwiseFactor *= -1;
        ++clockSwitchCounter;
        float variation = UnityEngine.Random.Range(-3, 3);
        range = Mathf.Clamp(range + variation, minimalRange, maximalRange);
        if (clockwiseFactor > 4 && ReadDice(WitchMoves.RamPlayer))
        {
            clockSwitchCounter = 0;
            ExecuteMove(WitchMoves.RamPlayer);
        }
    }
    public void DoPoof(WitchMoves poof)
    {
        if (poof != WitchMoves.GreenPoof && poof != WitchMoves.RedPoof)
        {
            return;
        }
        // select tp point
        Instantiate(moveStatsDict[poof].moveGameObject, transform.position, Quaternion.Euler(0f, 0f, 0f));
        transform.position = SelectTPNode(UnityEngine.Random.Range(5f, 10f));
    }
    void DoRamPlayer()
    {
        StartCoroutine(DoRamPlayerRoutine());
    }
    IEnumerator DoRamPlayerRoutine()
    {
        moveStatsDict[WitchMoves.AnyMove].canDo = false;
        doingDash = true;
        movement = playerTransform.position - transform.position;
        float distance = movement.magnitude * 2;
        //raycast to cut distance
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hitInWay = Physics2D.Raycast(playerTransform.position, Quaternion.Euler(0f, 0f, 0f) * movement, distance, mask);
        if (hitInWay)
        {
            distance = (hitInWay.transform.position - transform.position).magnitude - 2f;
        }
        float dashTime = distance * dashSpeed;
        solidCollider.enabled = false;
        yield return new WaitForSeconds(dashTime);
        solidCollider.enabled = true;
        doingDash = false;
        moveStatsDict[WitchMoves.AnyMove].canDo = true;
    }
    void DoVortex()
    {
        for (int i = 0; i < healthPhase; i++)
        {
            CombatManager.InstantiateSpell(vortexGameObject, gameObject, transform.position + (Vector3)movement.normalized * 2f, Quaternion.Euler(0, 0, 0));
        }
    }
    public IEnumerator DoMedusaRoutine()
    {
        hairAnimator.SetBool("Medusa", true);
        Instantiate(moveStatsDict[WitchMoves.Medusa].moveGameObject, playerTransform.position, Quaternion.Euler(0f, 0f, 0f));
        yield return new WaitForSeconds(medusaTime);
        hairAnimator.SetBool("Medusa", false);
    }
    void DoMedusa()
    {
        StartCoroutine(DoMedusaRoutine());
    }
    void DoBoulder()
    {
        int boulderCount = (int)Mathf.Clamp(healthPhase * 2 + UnityEngine.Random.Range(-2, healthPhase + 1), 1, Mathf.Infinity);
        for (int i = 0; i < boulderCount; i++)
        {
            Vector3 boulderPos = SelectRandomNode() + new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);
            Vector3 diffFromPlayer = playerTransform.position - boulderPos;
            if (diffFromPlayer.magnitude > 15f)
            {
                boulderPos = boulderPos + diffFromPlayer.normalized * UnityEngine.Random.Range(0, 30f);
            }
            CombatManager.InstantiateSpell(moveStatsDict[WitchMoves.Boulder].moveGameObject, gameObject, boulderPos, Quaternion.Euler(0, 0, 0));
        }
    }
    IEnumerator HandleCast(WitchMoves spell)
    {
        movement = playerTransform.position - transform.position;
        wandAnimator.SetBool("WandOut", true);
        if (moveStatsDict[spell].castTime > 0f)
        {
            isCasting = true;
            yield return new WaitForSeconds(moveStatsDict[spell].castTime);
            isCasting = false;
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        wandAnimator.SetBool("WandOut", false);
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
        float percentage = health.GetHealth() / health.GetMaxHealth();
        healthPhase = 1;
        for (int i = 5; i > 0; i--)
        {
            if (percentage < i / 6)
            {
                healthPhase++;
            }
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
    public Action doMethod;
    public WitchMoves parentCategory;
    public WitchMoveInfos(float probability, float cooldown, bool canDo = true, int dice = 0, float castTime = 0,
    GameObject moveGameObject = null, Action doMethod = null, WitchMoves parentCategory = WitchMoves.AnyMove)
    {
        this.probability = probability;
        this.cooldown = cooldown;
        this.canDo = canDo;
        this.dice = dice;
        this.castTime = castTime;
        this.moveGameObject = moveGameObject;
        this.doMethod = doMethod;
        this.parentCategory = parentCategory;
    }
}