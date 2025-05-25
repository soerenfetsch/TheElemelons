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
    GreenSpray,
    None
}
public class Witch : Enemy
{
    public List<float> dice;
    int diceCount = 3;
    public float intervalThrowdice = 6f;
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
    Vector2 movement { get; set; }
    float moveSpeed = 3f;
    float hoverSpeedFactor = 1.3f;
    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 0.5f;
    FlashEffect flashEffectBody;
    FlashEffect flashEffectWand;
    FlashEffect flashEffectHair;
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
    Animator sparkleEffect1;
    Animator sparkleEffect2;
    GameObject bossbar;
    BoxCollider2D playerCollider;
    [Header("Medusa")]
    public float medusaTime = 3f;
    [Header("Boulder")]
    public float boulderDamage = 1f;
    public float boulderThrust = 6f;
    public float boulderKnockTime = 0.8f;
    [Header("Dash")]
    int dashMaxnumber = 3;
    float dashtime = 1;
    float dashSpeed = 12f;
    bool doingDash = false;
    [Header("Spell- and MoveCooldowns, can and prob")]
    bool isCasting = false;
    public Dictionary<WitchMoves, WitchMoveInfos> moveStatsDict;
    float rechargeProb = 0.4f;
    float dashCooldown = 4f;
    float dashProbability = 0.2f;
    float medusaCooldown = 12f;
    float medusaProbability = 0.9f;
    public GameObject medusaGameObject;
    float offensiveSpellCooldown = 3f;
    float offensiveSpellProbability = 0.5f;
    float offensiveSpellCastTime = 1.5f;
    float evasionSpellCooldown = 7f;
    float evasionSpellProbability = 0.5f;
    float baldCooldown = 15f;
    float baldProbability = 0.85f;
    float snekCastTime = 1.3f;
    public GameObject snekObject;
    float redPoofProbability = 0.15f;
    public GameObject redPoofGameObject;
    float redPoofCooldown = 20f;
    float greenPoofProbability = 0.8f;
    float greenPoofCooldown = 10f;
    public GameObject greenPoofGameObject;
    float vortexProbability = 0.3f;
    float vortexCooldown = 10f;
    public GameObject vortexGameObject;
    float boulderProbability = 0.3f;
    float boulderCastTime = 1.5f;
    public GameObject boulderGameObject;
    float specialMoveProbability = 0.8f;
    float specialMoveCooldown = 1f;
    float ramPlayerProbability = 0.1f;
    float ramPlayerCooldown = 3f;
    float greenSprayProbability = 0.9f;
    float greenSprayCooldown = 4f;
    public GameObject greenSprayGameObject;

    // Start is called before the first frame update
    void Start()
    {
        InitializeFindPlayerPathfinder();
        InitializeGeneral();
        InitializeNodes();
        InitilializeMoveStats();
        InitializeDice();
    }
    public void InitializeGivenPlayerPathfinder(Transform player, LineOfSightPathfinder pathfinder)
    {
        playerTransform = player;
        this.pathfinder = pathfinder;
    }
    void InitializeFindPlayerPathfinder()
    {
        try
        {
            playerTransform = GameObject.Find("Player").transform;
            pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        }
        catch (System.Exception)
        {
            Debug.LogError("no player or pathfinder found");
            throw;
        }

    }
    public void InitializeGeneral()
    {
        attackFactor = 1;
        defenseFactor = 1;

        BoxCollider2D[] playerColliders = playerTransform.GetComponents<BoxCollider2D>();
        playerCollider = playerColliders[0].isTrigger ? playerColliders[0] : playerColliders[1];
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = GetComponent<Animator>();
        hairAnimator = transform.GetChild(0).GetComponent<Animator>();
        wandAnimator = transform.GetChild(1).GetComponent<Animator>();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
        solidCollider = GetComponent<BoxCollider2D>();
        health = GetComponent<Health>();
        try
        {
            bossbar = GameObject.Find("UI_Canvas/InGamePanel/BossBar");
            bossbar.SetActive(true);
            bossbar.GetComponent<BossHealthbar>().bossHealth = health;
        }
        catch (System.Exception)
        {
            Debug.LogError("Bossbar not found");
            throw;
        }
        sparkleEffect1 = transform.GetChild(2).GetChild(0).GetComponent<Animator>();
        sparkleEffect2 = transform.GetChild(2).GetChild(1).GetComponent<Animator>();
        movement = new Vector2(0, 0);
        flashEffectBody = GetComponent<FlashEffect>();
        flashEffectWand = transform.GetChild(1).GetComponent<FlashEffect>();
        flashEffectHair = transform.GetChild(0).GetComponent<FlashEffect>();
    }
    public void InitializeDice()
    {
        dice = new List<float>();
        for (int i = 0; i < 2 * diceCount; i++)
        {
            dice.Add(1f);
        }
    }
    /// <summary>
    /// Initializes a dictionary containing information on all of the witch’s abilities, 
    /// such as their cooldown time, probability, tied gameobject, time needed to cast, method that executes the ability, 
    /// whether the cooldown is running and which die should be read.
    /// </summary>
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
        WitchMoveInfos baldInfos = new WitchMoveInfos(baldProbability, baldCooldown, false, castTime: snekCastTime, parentCategory: WitchMoves.SpecialMove, moveGameObject: snekObject, doMethod: DoSnek);
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
        //greenspray
        WitchMoveInfos greenSprayInfos = new WitchMoveInfos(greenSprayProbability, greenSprayCooldown, dice: diceCount, doMethod: DoGreenSpray, parentCategory: WitchMoves.Offense, castTime: 1f, moveGameObject: greenSprayGameObject);
        moveStatsDict.Add(WitchMoves.GreenSpray, greenSprayInfos);
    }
    void InitializeNodes()
    {
        nodeCollection = new List<Vector3>();
        try
        {
            GameObject nodeCollectionObject = GameObject.Find("NodeCollection");
            int childCount = nodeCollectionObject.transform.childCount;
            for (int child = 0; child < childCount; child++)
            {
                nodeCollection.Add(nodeCollectionObject.transform.GetChild(child).transform.position);
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("no node collection found");
            throw;
        }
    }
    /// <summary>
    /// Given a specific ability, checks its probability given the WitchMoveDictionary and the number its die is currently holding 
    /// and returns whether the ability can currently be executed, not taking its cooldown in consideration.
    /// </summary>
    /// <param name="move"></param>
    /// <param name="diceToRead"></param>
    /// <returns></returns>
    bool ReadDice(WitchMoves move, int diceToRead = -1)
    {
        if ((diceToRead < 0 && dice[moveStatsDict[move].dice] < moveStatsDict[move].probability)
        || (diceToRead >= 0 && dice[diceToRead] < moveStatsDict[move].probability))
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// Sets a random node as destination. Node must be within dashDistanceRadius, 
    /// no wall inbetween path to node , and if awayfromplayer true angle between witch/player and witch/node must be >90 deg
    /// </summary>
    /// <param name="dashDistance"></param>
    /// <param name="awayFromPlayer"></param>
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
    /// <summary>
    /// select random node within distance
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
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
    /// <summary>
    /// selects random node
    /// </summary>
    /// <returns></returns>
    Vector3 SelectRandomNode()
    {
        nodeCollection = nodeCollection.OrderBy(x => UnityEngine.Random.value).ToList<Vector3>();
        return nodeCollection[0];
    }
    /// <summary>
    /// Given a list of node positions, randomly selects a node in proximity and sets it as destination. 
    /// Witch will ‘dash’ towards it with increased speed for a short time.
    /// </summary>
    /// <param name="dashDistance"></param>
    /// <param name="awayFromPlayer"></param>
    /// <param name="numberOfDashes"></param>
    /// <returns></returns>
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
    /// <summary>
    /// witch teleports to random node in proximity and leaves behind a green poof effect.
    /// </summary>
    void DoGreenPoof()
    {
        DoPoof(WitchMoves.GreenPoof);
    }
    /// <summary>
    /// witch casts a red poof effect and  teleports to random node in proximity and leaves behind a green poof effect.
    /// </summary>
    void DoRedPoof()
    {
        DoPoof(WitchMoves.RedPoof);
    }
    /// <summary>
    /// Casts several green spells (same as tiki), that 'spray' at different angles. 
    /// Number and angle coverage increases as the witch’s health becomes lower.
    /// </summary>
    void DoGreenSpray()
    {
        StartCoroutine(DoGreenSprayCoroutine());
    }
    IEnumerator DoGreenSprayCoroutine()
    {
        int spellCount;
        float anglePerSpell;
        float startAngle;
        switch (healthPhase)
        {
            case 1:
                spellCount = 2;
                anglePerSpell = 15f;
                startAngle = 0f;
                break;
            case 2:
                spellCount = 3;
                anglePerSpell = 15f;
                startAngle = 0f;
                break;
            case 3:
                spellCount = 3;
                anglePerSpell = 15f;
                startAngle = -15f;
                break;
            case 4:
                spellCount = 6;
                anglePerSpell = 15f;
                startAngle = -45f;
                break;
            case 5:
                spellCount = 8;
                anglePerSpell = 20f;
                startAngle = -60f;
                break;
            case 6:
                spellCount = 12;
                anglePerSpell = 30f;
                startAngle = -120f;
                break;
            default:
                spellCount = 1;
                anglePerSpell = 15f;
                startAngle = 0f;
                break;
        }
        float timeBetweenCasts = moveStatsDict[WitchMoves.GreenSpray].castTime / (float)spellCount;
        Vector3 playerdir = (playerTransform.position - transform.position).normalized * 3f;
        Vector3 dir = Quaternion.Euler(0f, 0f, startAngle) * playerdir;
        for (int i = 0; i < spellCount; i++)
        {
            Vector3 castpoint = transform.position + dir;
            CombatManager.InstantiateSpell(moveStatsDict[WitchMoves.GreenSpray].moveGameObject, gameObject, castpoint, Quaternion.Euler(0, 0, 0));
            dir = Quaternion.Euler(0f, 0f, anglePerSpell) * dir;
            yield return new WaitForSeconds(timeBetweenCasts);
        }
        if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
        {
            DoGreenSpray();
        }
    }
    /// <summary>
    /// Conducts cooldown of given ability. Updates its cando bool to true at the end.
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
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
        ActOnHealth();
        if (!isMovementBlocked)
        {
            ActOnStatus();
        }
        DoMovement();
    }
    void Update()
    {
        ManageBodyAnimators();
        if ((transform.position - playerTransform.position).magnitude > 30f)
        {
            DoGreenPoof();
        }
    }
    /// <summary>
    /// Called every frame. 3 Short (~1 unit) raycasts in movement direction. hit is saved as global variable.
    /// </summary>
    public void NewHitRayCast()
    {
        // check hit
        float rayLength = 2f;
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, 10f) * movement, rayLength, mask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, -10f) * movement, rayLength, mask);
        hit = hit0 ? hit0 : (hit1 ? hit1 : (hit2 ? hit2 : hit0));
    }
    /// <summary>
    /// Rolls dice every few seconds (~3-9). Some die are rolled every 3s, some every 6s, some every 9s. 
    /// An individual die used to decide on how the witch should behave or which ability she should use.
    /// </summary>
    void ThrowDice()
    {
        int intervalThrowDiceFramerate = (int)(1.0f / Time.deltaTime * intervalThrowdice);
        for (int i = 0; i < diceCount; i++)
        {
            if (Time.frameCount % intervalThrowDiceFramerate * (i + 1) == 0)
            {
                dice[i] = UnityEngine.Random.Range(0, 1);
                dice[i + diceCount] = UnityEngine.Random.Range(0, 1);
            }
        }
    }
    /// <summary>
    /// Given a list of abilities and taking their probability into account, randomly executes one of these abilities.
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    public WitchMoves Chose(List<WitchMoves> choices, bool executeChoice = true)
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
            if (executeChoice)
            {
                ExecuteMove(minKey);
            }
            return minKey;
        }
        return WitchMoves.None;
    }
    bool CanInput()
    {
        return !doingDash && !isCasting;
    }
    /// <summary>
    /// Update animator for body, wand and hair to reflect witch’s current state and movement direction.
    /// </summary>
    void ManageBodyAnimators()
    {
        bodyAnimator.SetFloat("Horizontal", movement.x);
        hairAnimator.SetFloat("Horizontal", movement.x);
        wandAnimator.SetFloat("Horizontal", movement.x);
        sparkleEffect1.SetFloat("Horizontal", movement.x);
        sparkleEffect2.SetFloat("Horizontal", movement.x);
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
        sparkleEffect1.SetBool("Hover", hovering || baldy);
        sparkleEffect2.SetBool("Hover", hovering || baldy);
    }
    /// <summary>
    /// Executes chosen movement.
    /// </summary>
    public override void DoMovement()
    {
        if (!isCasting)
        {
            movement = movement.normalized;
            Vector2 moving_by = movement * GetMoveSpeed() * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moving_by);
        }
    }
    public override void OnFreeze(bool freezeOn = true)
    {
        if (freezeOn)
        {
            movement = Vector2.zero;
            bodyAnimator.speed = 0;
            hairAnimator.speed = 0;
            wandAnimator.speed = 0;
        }
        else
        {
            bodyAnimator.speed = 1;
            hairAnimator.speed = 1;
            wandAnimator.speed = 1;
        }
    }
    public Vector2 CalcCircleDir()
    {
        return (Vector2)(Quaternion.Euler(0f, 0f, clockwiseFactor * 90f) * (playerTransform.position - transform.position));
    }
    public Vector2 PickStraightDir()
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
    /// Called every frame. Executes OnXXXStatus() method corresponding to the witch’s state.
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
    public WitchMoves ChoseSpellFromPriorityBatches(List<List<WitchMoves>> moveBatches, List<float> priorityFactors, bool executeChoice = true)
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
            if (canDoCount > 0 && priorityFactors[i] > 0)
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
            chosen = Chose(batchChoice, executeChoice: executeChoice);
        }
        else
        {
            chosen = WitchMoves.None;
        }
        return chosen;
    }
    /// <summary>
    /// Given an ability, executes it (via the dictionary)
    /// </summary>
    /// <param name="move"></param>
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
    /// <summary>
    /// Is in walkstatus if still has >~80%health. Witch randomly executes an ability from the following selection if 
    /// their cooldown isnt running, the dice allow it and depending on her distance from the player: 
    /// Greenpoof, RedPoof, GreenSpray, Dash, Boulder. Else she walks around randomly.
    /// </summary>
    public void OnWalkStatus()
    {
        if (currentRangeState == RangeState.Close && CanInput())
        {
            if (moveStatsDict[WitchMoves.SpecialMove].canDo && ReadDice(WitchMoves.SpecialMove) &&
                ((moveStatsDict[WitchMoves.Evasion].canDo && ReadDice(WitchMoves.Evasion)) ||
                (moveStatsDict[WitchMoves.Dash].canDo && ReadDice(WitchMoves.Dash))))
            {
                // pick witchMove
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.GreenPoof };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.Dash };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 0.5f, 1f };
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
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Boulder, WitchMoves.GreenSpray };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.GreenPoof };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 1.5f, 1f };
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
                moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense))
            {
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Boulder, WitchMoves.GreenSpray };
                Chose(choices0);
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
    /// <summary>
    /// Is in hoverstatus if still has ~80%>health >~30%. Witch randomly executes an ability from the following selection if 
    /// their cooldown isnt running, the dice allow it and depending on her distance from the player: 
    /// Greenpoof, RedPoof, GreenSpray, Dash, Boulder, Vortex, Medusa. Else she walks around randomly.
    /// </summary>
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
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder, WitchMoves.GreenSpray };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 2.5f, 1f };
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
                (moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense)))
            {
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder, WitchMoves.GreenSpray };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0 };
                List<float> priorities = new List<float> { 1f };
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
        if (clockSwitchCounter > 4 && ReadDice(WitchMoves.RamPlayer) && currentWitchState == WitchState.Hover)
        {
            clockSwitchCounter = 0;
            ExecuteMove(WitchMoves.RamPlayer);
        }
    }
    /// <summary>
    /// executes red or green poof depending on argument
    /// </summary>
    /// <param name="poof"></param>
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
    /// <summary>
    /// Takes position of player at the moment the move was started and dashes towards it.
    /// </summary>
    void DoRamPlayer()
    {
        StartCoroutine(DoRamPlayerRoutine());
    }
    IEnumerator DoRamPlayerRoutine()
    {
        moveStatsDict[WitchMoves.AnyMove].canDo = false;
        doingDash = true;
        movement = playerTransform.position - transform.position;
        float distance = movement.magnitude + 5f;
        //raycast to cut distance
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hitInWay = Physics2D.Raycast(transform.position, movement, movement.magnitude, mask);
        if (hitInWay)
        {
            distance = movement.magnitude;
        }
        float dashTime = dashSpeed / distance;
        Physics2D.IgnoreCollision(solidCollider, playerCollider, true);
        yield return new WaitForSeconds(dashTime);
        Physics2D.IgnoreCollision(solidCollider, playerCollider, false);
        doingDash = false;
        moveStatsDict[WitchMoves.AnyMove].canDo = true;
    }
    /// <summary>
    /// Casts vortex spells. Number increases as the witch’s health becomes lower.
    /// </summary>
    void DoVortex()
    {
        //(float)(healthPhase - 1) / 2 + 1
        int vortexCount;
        int d = 3;
        switch (healthPhase)
        {
            case 2:
                vortexCount = 1 + ((dice[d] < 0.2) ? 1 : 0);
                break;
            case 3:
                vortexCount = 1 + ((dice[d] < 0.5) ? 1 : 0);
                break;
            case 4:
                vortexCount = 1 + ((dice[d] < 0.7) ? 1 : 0);
                break;
            case 5:
                vortexCount = 1 + ((dice[d] < 0.5) ? 1 : 0) + ((dice[d + 1] < 0.3) ? 1 : 0);
                break;
            case 6:
                vortexCount = 1 + ((dice[d] < 0.5) ? 1 : 0) + ((dice[d + 1] < 0.3) ? 1 : 0);
                break;
            default:
                vortexCount = 1;
                break;
        }
        for (int i = 0; i < vortexCount; i++)
        {
            CombatManager.InstantiateSpell(moveStatsDict[WitchMoves.Vortex].moveGameObject, gameObject, transform.position + (Vector3)movement.normalized * 2f, Quaternion.Euler(0, 0, 0));
        }
    }
    /// <summary>
    /// Instantiates the medusa effect and shows animation in witch’s hair.
    /// </summary>
    /// <returns></returns>
    public IEnumerator DoMedusaRoutine()
    {
        hairAnimator.SetBool("Medusa", true);
        Instantiate(moveStatsDict[WitchMoves.Medusa].moveGameObject, playerTransform.position, Quaternion.Euler(0f, 0f, 0f));
        yield return new WaitForSeconds(medusaTime);
        hairAnimator.SetBool("Medusa", false);
    }
    /// <summary>
    ///  Instantiates the medusa effect and shows animation in witch’s hair.
    /// </summary>
    void DoMedusa()
    {
        StartCoroutine(DoMedusaRoutine());
    }
    /// <summary>
    /// Shows animation where the snake hair of the witch leave her and become 1 single independent snake. 
    /// Witch is left bald. Instantiates a snake enemy.
    /// </summary>
    void DoSnek()
    {
        StartCoroutine(DoSnekRoutine());

    }
    IEnumerator DoSnekRoutine()
    {
        hairAnimator.SetTrigger("Snek");
        yield return new WaitForSeconds(snekCastTime);
        Vector3 spawnPointSnek = transform.GetChild(3).GetChild(1).transform.position;
        if (movement.x < 0)
        {
            spawnPointSnek = transform.GetChild(3).GetChild(0).transform.position;
        }
        Instantiate(moveStatsDict[WitchMoves.Snek].moveGameObject, spawnPointSnek, Quaternion.Euler(0f, 0f, 0f));
    }
    /// <summary>
    ///  Casts a few boulder spells at random locations inside of the bossroom. 
    /// Number of boulders increases as the witch’s health becomes lower.
    /// </summary>
    void DoBoulder()
    {
        int boulderCount = (int)Mathf.Clamp(healthPhase * 2 + UnityEngine.Random.Range(-2, healthPhase + 1), 1, Mathf.Infinity);
        int randomNumber = UnityEngine.Random.Range(0, healthPhase);
        int d = 2;
        switch (healthPhase)
        {
            case 2:
                boulderCount = 1 + ((dice[d] < 0.5) ? 1 : 0) + randomNumber / 2;
                break;
            case 3:
                boulderCount = 2 + ((dice[d] < 0.5) ? 1 : 0) + randomNumber;
                break;
            case 4:
                boulderCount = 2 + ((dice[d] < 0.7) ? 1 : 0) + (int)(randomNumber * (1 + dice[d]));
                break;
            case 5:
                boulderCount = 3 + randomNumber / 2 + (int)(randomNumber * (1 + dice[d] * 2));
                break;
            case 6:
                boulderCount = 3 + randomNumber + (int)(randomNumber * (1 + dice[d] * 2));
                break;
            default:
                boulderCount = 1 + ((dice[d] < 0.5) ? 1 : 0);
                break;
        }
        for (int i = 0; i < boulderCount; i++)
        {
            Vector3 boulderPos = SelectRandomNode() + new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);
            Vector3 diffFromPlayer = playerTransform.position - boulderPos;
            if (diffFromPlayer.magnitude > 15f)
            {
                boulderPos = boulderPos + diffFromPlayer * UnityEngine.Random.Range(0.7f, 1.3f);
            }
            if (i == 0)
            {
                boulderPos = pathfinder.transform.position + new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3), 0);
            }
            CombatManager.InstantiateSpell(moveStatsDict[WitchMoves.Boulder].moveGameObject, gameObject, boulderPos, Quaternion.Euler(0, 0, 0));
        }
    }
    /// <summary>
    /// Witch takes out wand and freezes during cast time. If spell to cast is offensive, show red sparkle effects around wand
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
    IEnumerator HandleCast(WitchMoves spell)
    {
        movement = playerTransform.position - transform.position;
        wandAnimator.SetBool("WandOut", true);
        int sparkleChoice = (int)UnityEngine.Random.Range(-1, 2);
        TurnSparkleEffectOnOff(spell, sparkleChoice, true);
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
        TurnSparkleEffectOnOff(spell, sparkleChoice, false);
        wandAnimator.SetBool("WandOut", false);
    }
    /// <summary>
    /// turns sparkle effects around wand on or off
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="choice"></param>
    /// <param name="onOff"></param>
    void TurnSparkleEffectOnOff(WitchMoves spell, int choice, bool onOff)
    {
        if (moveStatsDict[spell].parentCategory == WitchMoves.Offense)
        {
            string triggerName = "Off";
            if (onOff)
            {
                triggerName = "On";
            }

            switch (choice)
            {
                case -1:
                    sparkleEffect1.SetTrigger(triggerName);
                    sparkleEffect2.SetTrigger(triggerName);
                    break;
                case 0:
                    sparkleEffect1.SetTrigger(triggerName);
                    break;
                case 1:
                    sparkleEffect2.SetTrigger(triggerName);
                    break;
            }
        }
    }
    /// <summary>
    /// updates witch state
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeWitchState(WitchState newState)
    {
        currentWitchState = newState;
    }
    /// <summary>
    ///  Is in baldstatus if  has <~30% health. Witch randomly executes an ability from the following selection 
    /// if their cooldown isnt running, the dice allow it and depending on her distance from the player: 
    /// Greenpoof, RedPoof, GreenSpray, Dash, Boulder, Vortex. Else she walks around randomly.
    /// </summary>
    public void OnBaldStatus()
    {
        if (currentRangeState == RangeState.Close && CanInput())
        {
            if (moveStatsDict[WitchMoves.SpecialMove].canDo && ReadDice(WitchMoves.SpecialMove) &&
                ((moveStatsDict[WitchMoves.Evasion].canDo && ReadDice(WitchMoves.Evasion)) ||
                (moveStatsDict[WitchMoves.Dash].canDo && ReadDice(WitchMoves.Dash))))
            {
                // pick witchMove
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.Dash };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 1.5f, 1f };
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
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder, WitchMoves.GreenSpray };
                List<WitchMoves> choices1 = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0, choices1 };
                List<float> priorities = new List<float> { 2.5f, 1f };
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
                moveStatsDict[WitchMoves.Offense].canDo && ReadDice(WitchMoves.Offense))
            {
                List<WitchMoves> choices0 = new List<WitchMoves> { WitchMoves.Vortex, WitchMoves.Boulder, WitchMoves.GreenSpray };
                List<List<WitchMoves>> choices = new List<List<WitchMoves>> { choices0 };
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
    /// <summary>
    /// Is idle if player out of range. no movement.
    /// </summary>
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
    /// Abstract method to define behavior based on the enemy's health.
    /// </summary>
    public override void ActOnHealth()
    {
        if (health.IsZero())
        {
            DeathEffect.SpawnDeathEffect(deathEffect, transform.position, deathEffectDelay);
            try
            {
                bossbar.SetActive(false);
            }
            catch (System.Exception)
            {
                Debug.LogError("no bossbar to deactivate");
                throw;
            }
            // destroy all
            LayerMask enemyMask = LayerMask.GetMask("Enemies");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 100f, enemyMask);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                Health enemyHealth = colliders[i].GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.SetHealth(0);
                }
            }
            Destroy(gameObject, 0);
        }
        float percentage = health.GetHealth() / health.GetMaxHealth();
        healthPhase = 1;
        for (int i = 5; i > 0; i--)
        {
            if (percentage < (float)i / 6)
            {
                healthPhase++;
            }
        }
        if (healthPhase < 2)
        {
            ChangeWitchState(WitchState.Walk);
        }
        else if (healthPhase < 5)
        {
            ChangeWitchState(WitchState.Hover);
        }
        else
        {
            if (currentWitchState != WitchState.Bald)
            {
                moveStatsDict[WitchMoves.Snek].doMethod();
            }
            ChangeWitchState(WitchState.Bald);
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
        flashEffectWand?.Flash(flashDuration);
        flashEffectHair?.Flash(flashDuration);
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
    public void SetMovement(Vector2 newMovement)
    { movement = newMovement; }
    public RaycastHit2D GetHit()
    {
        return hit;
    }
    public float GetWalkSpeed()
    {
        return moveSpeed;
    }
    public float GetHoverSpeed()
    {
        return moveSpeed * hoverSpeedFactor;
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