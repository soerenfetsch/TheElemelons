using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SnekStatus
{
    Crawl,
    Dash,
    Attack
}
public class Snek : Enemy
{
    [Header("Bound Components")]
    public GameObject deathEffect;
    float deathEffectDelay = 0.5f;
    public FlashEffect flashEffect;
    public float flashDuration = 0.2f;
    Transform playerTransform;
    LineOfSightPathfinder pathfinder;
    Rigidbody2D rb;
    Animator animator;
    GameObject witch;
    [Header("Ranges")]
    float aggroRange = 50f;
    float dashRange = 10f;
    float tongueRange = 5f;
    [Header("Flags")]
    bool canTongue = true;
    float tongueCooldown = 6f;
    float attackTime = 3f;
    public SnekStatus currentSnekStatus = SnekStatus.Crawl;
    bool spawned = true;
    [Header("Damage")]
    float baseDamage = 1f;
    float knockTime = 0.3f;
    float thrust = 5f;
    [Header("Movement")]
    RaycastHit2D hit;
    Vector2 movement;
    float moveFactor = 1f;
    float dashSpeed = 7f;
    float attackSpeed = 10f;
    float crawlSpeed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        elementType = ElementType.Earth;
        attackFactor = 1;
        defenseFactor = 1;
        playerTransform = GameObject.Find("Player").transform;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<LineOfSightPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        movement = Vector2.zero;
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        witch = GameObject.Find("Witch");
        if (witch != null)
        {
            spawned = false;
            StartCoroutine(Spawn());
        }
    }
    IEnumerator Spawn()
    {
        float orientation = witch.GetComponent<Animator>().GetFloat("Horizontal");
        if (orientation < 0)
        {
            transform.position = witch.transform.GetChild(3).GetChild(0).transform.position;
            movement = Vector2.left;
        }
        else
        {
            transform.position = witch.transform.GetChild(3).GetChild(1).transform.position;
            movement = Vector2.right;
        }
        float scale = 2;
        transform.localScale = new Vector3(scale, scale, 0);
        float fadeTimer = 1f;
        float fadeDuration = fadeTimer;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            scale = alpha + 1;
            transform.localScale = new Vector3(scale, scale, 0);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        spawned = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawned)
        {
            NewRayCast();
            ActOnHealth();
            ActOnRange();
            ActOnStatus();
            DoMovement();
        }
    }
    void NewRayCast()
    {
        // check hit
        float rayLength = 2f;
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, 10f) * movement, rayLength, mask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, -10f) * movement, rayLength, mask);
        hit = hit0 ? hit0 : (hit1 ? hit1 : (hit2 ? hit2 : hit0));
    }
    void Update()
    {
        UpdateAnimator();
    }
    void UpdateAnimator()
    {
        if (currentSnekStatus == SnekStatus.Crawl)
        {
            animator.SetBool("Speed", false);
        }
        else
        {
            animator.SetBool("Speed", true);
        }
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }
    public void DoMovement()
    {
        int turn = 15;
        for (int i = 0; i < 360f; i = i + turn)
        {
            if (hit)
            {
                movement = Quaternion.Euler(0f, 0f, turn) * movement;
                NewRayCast();
            }
        }
        movement.Normalize();
        Vector2 moving_by = movement * GetMoveSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moving_by);
    }
    public float GetMoveSpeed()
    {
        if (currentSnekStatus == SnekStatus.Dash)
        {
            return dashSpeed * moveFactor; ;
        }
        else if (currentSnekStatus == SnekStatus.Attack)
        {
            return attackSpeed * moveFactor; ;
        }
        return crawlSpeed * moveFactor;
    }
    /// <summary>
    /// Abstract method to enforce state transitions in derived classes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        currentState = newState;
    }

    void ActOnRange()
    {
        if (Mathf.Abs((transform.position - playerTransform.position).magnitude) < aggroRange)
        {
            ChangeState(EnemyStatus.Approaching);
        }
        else
        {
            ChangeState(EnemyStatus.Idle);
        }
        if (currentSnekStatus != SnekStatus.Attack && currentState != EnemyStatus.Idle)
        {
            float dist = Mathf.Abs((transform.position - playerTransform.position).magnitude);
            bool isDashing = dist < dashRange;
            Debug.Log(currentState.ToString() + "  " + currentSnekStatus.ToString());
            if (!isDashing)
            {
                currentSnekStatus = SnekStatus.Crawl;
            }
            else
            {
                currentSnekStatus = SnekStatus.Dash;
            }
            if (canTongue && dist < tongueRange)
            {
                StartCoroutine(DoTongue());
            }
        }
    }
    /// <summary>
    /// Abstract method to define behavior based on the current state.
    /// </summary>
    public override void ActOnStatus()
    {
        if (currentState == EnemyStatus.Idle)
        {
            OnIdleStatus();
        }
        else if (currentSnekStatus == SnekStatus.Crawl)
        {
            if (Time.frameCount % 30 == 0)
            {
                OnCrawlStatus();
            }
        }
        else if (currentSnekStatus == SnekStatus.Dash)
        {
            if (Time.frameCount % 30 == 0)
            {
                OnDashStatus();
            }
        }
        else if (currentSnekStatus == SnekStatus.Attack)
        {
            OnAttackStatus();
        }
    }
    void OnAttackStatus()
    {
    }
    void OnDashStatus()
    {
        Vector2 dir;
        if (canTongue)
        {
            dir = pathfinder.CheckPlayerInSight(transform.position, aggroRange);
            if (dir == Vector2.zero)
            {
                dir = pathfinder.CheckPlayerReachable(transform.position, aggroRange);
            }
        }
        else
        {
            dir = transform.position - playerTransform.position;
        }

        Vector2 dirAbs = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
        if (dirAbs.x > dirAbs.y)
        {
            movement = new Vector2(dir.x, 0);
        }
        else
        {
            movement = new Vector2(0, dir.y);
        }
    }
    IEnumerator DoTongue()
    {
        movement = playerTransform.position - transform.position;
        currentSnekStatus = SnekStatus.Attack;
        animator.SetTrigger("Tongue");
        yield return new WaitForSeconds(attackTime);
        animator.SetTrigger("Tongue");
        currentSnekStatus = SnekStatus.Dash;
        canTongue = false;
        yield return new WaitForSeconds(tongueCooldown);
        canTongue = true;
    }
    public void OnIdleStatus()
    {
        movement = Vector2.zero;
    }
    public void OnCrawlStatus()
    {
        Vector2 dir = pathfinder.CheckPlayerInSight(transform.position, aggroRange);
        if (dir == Vector2.zero)
        {
            dir = pathfinder.CheckPlayerReachable(transform.position, aggroRange);
        }
        if (!canTongue)
        {
            dir *= -1;
        }
        movement = new Vector2(Mathf.Sign(dir.x), Mathf.Sign(dir.y));
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
        EnemyStatus previousState = currentState;
        ChangeState(EnemyStatus.Staggered);
        flashEffect?.Flash(flashDuration);
        yield return new WaitForSeconds(staggerTime);
        ChangeState(previousState);
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
        // Do nothing
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(gameObject, other.GetComponent<Rigidbody2D>(), elementType, attackDirection, baseDamage, thrust, knockTime);
        }
    }
}
