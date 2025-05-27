using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// it is the body that can receive damage
/// </summary>
public class TikiBody : Enemy
{
    Tiki tikiScript;
    void Start()
    {
        tikiScript = transform.parent.GetComponent<Tiki>();
        health = GetComponent<Health>();
        mana = tikiScript.GetComponent<Mana>();
        elementType = tikiScript.elementType;
    }
    void FixedUpdate()
    {
        currentState = tikiScript.GetCurrentState();
        attackFactor = tikiScript.attackFactor;
        defenseFactor = tikiScript.defenseFactor;
    }
    public override void ChangeState(EnemyStatus newState)
    {
        tikiScript.ChangeState(newState);
    }

    /// <summary>
    /// Abstract method to define behavior based on the current state.
    /// </summary>
    public override void ActOnStatus()
    { }

    /// <summary>
    /// Abstract method to define behavior based on the enemy's health.
    /// </summary>
    public override void ActOnHealth()
    { }

    /// <summary>
    /// Abstract method to define behavior based on the enemy's mana.
    /// </summary>
    public override void ActOnMana()
    { }
    public override void GetKnocked(float knockTime)
    {
        tikiScript.GetKnocked(knockTime);
    }

    public override void ModifySpeed(float factor)
    {
        tikiScript.ModifySpeed(factor);
    }
    public override void ModifyDamage(float factor)
    {
        tikiScript.ModifyDamage(factor);
    }
    public override void ModifyDefense(float factor)
    {
        tikiScript.ModifyDefense(factor);
    }
    public override void ModifyManaRegen(float factor)
    {
        tikiScript.ModifyManaRegen(factor);
    }
    public override void OnFreeze(bool freezeOn = true)
    {
        tikiScript.OnFreeze(freezeOn);
        tikiScript.SetIsMovementBlocked(freezeOn);
    }
    public override void DoMovement()
    {
    }
    /// <summary>
    /// Delegate to tiki
    /// </summary>
    /// <returns></returns>
    public override bool ObstructedSightCantSeePlayer()
    {
        return tikiScript.ObstructedSightCantSeePlayer();
    }
    public override void DoObstructSight(bool onOff)
    {
        ObstructSight(onOff);
        tikiScript.ObstructSight(onOff);
    }
}
