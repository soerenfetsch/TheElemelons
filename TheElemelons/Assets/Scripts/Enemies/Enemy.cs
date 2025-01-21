using System.Collections;
using System.Collections.Generic;
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
public abstract class Enemy : MonoBehaviour
{
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
}
