using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Enemy
{
    [Header("Components")]
    private Transform transformComponent;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;
    private BoxCollider2D hitbox;
    private BoxCollider2D collisionBox;
    private Animator animator;

    [Header("Player Info")]
    public Transform playerTransform; // To store the player's transform for pathfinding.

    [Header("Behavior")]
    private Vector2 homePosition;
    private bool isStaggering = false;
    public float moveSpeed = 1f;
    public float staggerTime = 0.5f;
    public float chaseRadius = 5f;
    public float attackRadius = 0.2f;

    // TODO: Add more attack behavior variables for combat

    [Header("Effects")]
    public GameObject deathEffect;
    public float deathEffectDelay = 1f;


    public void Initialize(Transform player)
    {
        playerTransform = player;
    }

    // Start is called before the first frame update
    void Start()
    {
        transformComponent = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        collisionBox = GetComponents<BoxCollider2D>()[1]; // Assuming second collider is for collision.
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();

        currentState = EnemyStatus.Idle;
        homePosition = transformComponent.position;
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
        currentState = newState;
    }

    public override void ActOnStatus()
    {
        switch (currentState)
        {
            case EnemyStatus.Idle:
                // Wait until player is within chase or attack radius
                if (CheckPlayerWithinRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Attacking);
                    animator.SetBool("wakeUp", true);
                }
                else if (CheckPlayerWithinRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Approaching);
                    animator.SetBool("wakeUp", true);
                }
                break;
            case EnemyStatus.Approaching:
                MoveTowardPlayer();
                if (CheckPlayerWithinRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Attacking);
                }
                else if (CheckPlayerOutsideRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Retreating);
                }
                break;
            case EnemyStatus.Attacking:
                // TODO: Add attack behavior with combat system
                if (CheckPlayerOutsideRadius(attackRadius))
                {
                    ChangeState(EnemyStatus.Approaching);
                }
                break;
            case EnemyStatus.Retreating:
                MoveTowardHome();
                if (CheckPlayerWithinRadius(chaseRadius))
                {
                    ChangeState(EnemyStatus.Approaching);
                }
                else if (CheckAtHome())
                {
                    ChangeState(EnemyStatus.Idle);
                    animator.SetBool("wakeUp", false);
                }
                break;
            case EnemyStatus.Staggered:
                if (!isStaggering) 
                {
                    isStaggering = true;
                    StartCoroutine(HandleStagger());
                }
                break;
        }
    }

    public override void ActOnHealth()
    {
        if (health.IsZero())
        {
            // TODO: start death animation and destroy object
        }
    }

    public override void ActOnMana()
    {
        // Do nothing
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
        else if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if(direction.y > 0)
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

    private bool CheckPlayerWithinRadius(float radius)
    {
        return Vector2.Distance(transformComponent.position, playerTransform.position) <= radius;
    }

    private bool CheckPlayerOutsideRadius(float radius)
    {
        return Vector2.Distance(transformComponent.position, playerTransform.position) > radius;
    }

    private void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transformComponent.position).normalized;
        rb2d.MovePosition((Vector2)transformComponent.position + direction * moveSpeed * Time.deltaTime);
        ChangeWalkAnimation(direction);
    }

    private void MoveTowardHome()
    {
        Vector2 direction = (homePosition - (Vector2)transformComponent.position).normalized;
        rb2d.MovePosition((Vector2)transformComponent.position + direction * moveSpeed * Time.deltaTime);
        ChangeWalkAnimation(direction);
    }

    private bool CheckAtHome()
    {
        return Vector2.Distance(transformComponent.position, homePosition) < 0.01f;
    }

    private IEnumerator HandleStagger()
    {
        animator.SetTrigger("stagger");
        yield return new WaitForSeconds(staggerTime);
        isStaggering = false;
        ChangeState(EnemyStatus.Idle);
    }
}
