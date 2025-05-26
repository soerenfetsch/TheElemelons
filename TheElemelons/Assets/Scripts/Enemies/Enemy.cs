using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

/// <summary>
/// Enum representing possible enemy states.
/// </summary>
public enum EnemyStatus
{
    Idle,
    Approaching,
    Attacking,
    Retreating,
    Staggered
}

/// <summary>
/// Abstract base class for enemy behavior. 
/// Defines common properties and enforces implementation of specific behaviors in derived classes.
/// </summary>
public abstract class Enemy : MonoBehaviour, ICombatant, IBuffable
{
    public Transform playerTransform;
    public Player player;
    protected Vector2 movement;
    protected bool isMovementBlocked = false;
    protected bool isSightObstructed = false;
    protected int confusedCounter = 0;
    protected float obstructedSightRange = 3f;
    /// <summary>
    /// Reference to the Health script for managing enemy health.
    /// </summary>
    protected Health health;

    /// <summary>
    /// Reference to the Mana script for managing enemy mana.
    /// </summary>
    protected Mana mana;

    /// <summary>
    /// Current state of the enemy (e.g., Idle, Attacking).
    /// </summary>
    protected EnemyStatus currentState;
    public float attackFactor { get; set; }
    public float defenseFactor { get; set; }
    public bool isInvincible { get; set; }
    public ElementType elementType { get; set; }

    public void DoMovementWithChecks()
    {
        if (isMovementBlocked)
        {
            OnFreeze();
        }
        else
        {
            if (ObstructedSightCantSeePlayer())
            {
                RandomWalk();
            }
            DoMovement();
        }
    }
    public virtual bool ObstructedSightCantSeePlayer()
    {
        return player.GetIsHidden() || isSightObstructed && (playerTransform.position - transform.position).magnitude > obstructedSightRange;
    }
    protected void RandomWalk()
    {
        float rayLength = 1f;
        LayerMask mask = LayerMask.GetMask("Walls");
        RaycastHit2D hitRandomWalk0 = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        RaycastHit2D hitRandomWalk1 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, 10f) * movement, rayLength, mask);
        RaycastHit2D hitRandomWalk2 = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, -10f) * movement, rayLength, mask);
        RaycastHit2D hitRandomWalk = hitRandomWalk0 ? hitRandomWalk0 : (hitRandomWalk1 ? hitRandomWalk1 : (hitRandomWalk2 ? hitRandomWalk2 : hitRandomWalk0));
        if (hitRandomWalk)
        {
            movement *= -1;
        }
        else if (Time.frameCount % 50 == 0)
        {
            movement = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-90f, 90f)) * movement;
            movement = movement.normalized;
        }
    }
    public void ObstructSight(bool onOff)
    {
        if (onOff && movement == Vector2.zero)
        {
            movement = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        }
        isSightObstructed = onOff;
    }
    public abstract void DoObstructSight(bool onOff);
    protected bool CanAct()
    {
        return !isMovementBlocked && !ObstructedSightCantSeePlayer();
    }
    public abstract void DoMovement();
    public abstract void OnFreeze(bool freezeOn = true);
    /// <summary>
    /// Abstract method to enforce state transitions in derived classes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public abstract void ChangeState(EnemyStatus newState);

    /// <summary>
    /// Abstract method to define behavior based on the current state.
    /// </summary>
    public abstract void ActOnStatus();

    /// <summary>
    /// Abstract method to define behavior based on the enemy's health.
    /// </summary>
    public abstract void ActOnHealth();

    /// <summary>
    /// Abstract method to define behavior based on the enemy's mana.
    /// </summary>
    public abstract void ActOnMana();
    public abstract void GetKnocked(float knockTime);

    /// <summary>
    /// Reduces the enemy's health by a specified amount.
    /// </summary>
    /// <param name="amount">The amount of health to subtract.</param>
    public void SubtractHealth(float amount)
    {
        health.SubtractHealth(amount);
    }

    /// <summary>
    /// Reduces the enemy's mana by a specified amount.
    /// </summary>
    /// <param name="amount">The amount of mana to subtract.</param>
    public void SubtractMana(float amount)
    {
        mana.SubtractMana(amount);
    }

    /// <summary>
    /// Retrieves the enemy's health component.
    /// </summary>
    /// <returns>The Health component of the enemy.</returns>
    public Health GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Retrieves the enemy's mana component.
    /// </summary>
    /// <returns>The Mana component of the enemy.</returns>
    public Mana GetMana()
    {
        return mana;
    }

    /// <summary>
    /// Retrieves the current state of the enemy.
    /// </summary>
    /// <returns>The current EnemyStatus of the enemy.</returns>
    public EnemyStatus GetCurrentState()
    {
        return currentState;
    }

    public abstract void ModifySpeed(float factor);
    public abstract void ModifyDamage(float factor);
    public abstract void ModifyDefense(float factor);
    public abstract void ModifyManaRegen(float factor);
    public bool GetIsMovementBlocked()
    { return isMovementBlocked; }
    public void SetIsMovementBlocked(bool newIsMovementBlocked)
    {
        isMovementBlocked = newIsMovementBlocked;
    }
    public void DoFreezeUnFreeze(bool freezeOn)
    {
        isMovementBlocked = freezeOn;
        OnFreeze(freezeOn: freezeOn);
    }
    /// <summary>
    /// does nothing, monsters cant hide
    /// </summary>
    /// <param name="factor"></param>
    public void ModifyIsHidden(float factor)
    { }
    public void ModifyIsConfused(float factor)
    {
        if (factor == 1)
        {
            confusedCounter++;
        }
        else
        {
            confusedCounter = (int)Mathf.Clamp(confusedCounter - 1, 0f, Mathf.Infinity);
        }
        DoObstructSight(confusedCounter > 0);
        Debug.Log(isSightObstructed);
    }
    [ContextMenu("Freeze")]
    public void DoFreezeUnFreeze()
    {
        DoFreezeUnFreeze(!isMovementBlocked);
    }
    [ContextMenu("ObstructSight")]
    public void DoObstructSightOnOff()
    {
        DoObstructSight(!isSightObstructed);
    }
}
