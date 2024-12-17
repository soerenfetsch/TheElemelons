using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStatus
{
    Idle,
    Approaching,
    Attacking,
    Retreating,
    Staggered
}

public abstract class Enemy : MonoBehaviour
{

    protected Health health;
    protected Mana mana;
    protected EnemyStatus currentState;


    // Abstract methods to enforce specific behaviors in derived classes
    public abstract void ChangeState(EnemyStatus newState);
    public abstract void ActOnStatus();
    public abstract void ActOnHealth();
    public abstract void ActOnMana();

    // Methods to interact with health and mana
    public void SubtractHealth(float amount)
    {
        health.SubtractHealth(amount);
    }

    public void SubtractMana(float amount)
    {
        mana.SubtractMana(amount);
    }

    public Health GetHealth()
    {
        return health;
    }

    public Mana GetMana()
    {
        return mana;
    }

    public EnemyStatus GetCurrentState()
    {
        return currentState;
    }
}
