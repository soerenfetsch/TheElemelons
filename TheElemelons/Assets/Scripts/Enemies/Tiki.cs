using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TikiState
{
    HealFriends,
    Panic,
    Circle
}
public class Tiki : Enemy
{
    public TikiState currentTikiState;
    public float moveSpeed = 1f;
    public bool flowerSkirt = false;
    Animator bodyAnimator;
    Animator skirtAnimator;
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
        skirtAnimator = transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
        mana = GetComponent<Mana>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        ActOnStatus();
    }

    /// <summary>
    /// Abstract method to enforce state transitions in derived classes.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public override void ChangeState(EnemyStatus newState)
    {
        currentState = newState;
    }
    public void ChangeTikiState(TikiState newState)
    {
        currentTikiState = newState;
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
        else if (currentState == EnemyStatus.Approaching)
        {

        }
        ActOnHealth();
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
}
