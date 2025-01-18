using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Fire,
    Water,
    Earth,
    Wind,
    None
}

public static class CombatManager
{
    private static Dictionary<ElementType, Dictionary<ElementType, float>> damageTable = new Dictionary<ElementType, Dictionary<ElementType, float>>()
    {
        {ElementType.Fire, new Dictionary<ElementType, float>(){
            {ElementType.Fire, 1f},
            {ElementType.Water, 1f},
            {ElementType.Earth, 1.25f},
            {ElementType.Wind, 1f},
            {ElementType.None, 1f}
        }},
        {ElementType.Water, new Dictionary<ElementType, float>(){
            {ElementType.Fire, 1.25f},
            {ElementType.Water, 1f},
            {ElementType.Earth, 1f},
            {ElementType.Wind, 1f},
            {ElementType.None, 1f}
        }},
        {ElementType.Earth, new Dictionary<ElementType, float>(){
            {ElementType.Fire, 1f},
            {ElementType.Water, 1f},
            {ElementType.Earth, 1f},
            {ElementType.Wind, 1.25f},
            {ElementType.None, 1f}
        }},
        {ElementType.Wind, new Dictionary<ElementType, float>(){
            {ElementType.Fire, 1f},
            {ElementType.Water, 1.25f},
            {ElementType.Earth, 1f},
            {ElementType.Wind, 1f},
            {ElementType.None, 1f}
        }},
        {ElementType.None, new Dictionary<ElementType, float>(){
            {ElementType.Fire, 1f},
            {ElementType.Water, 1f},
            {ElementType.Earth, 1f},
            {ElementType.Wind, 1f},
            {ElementType.None, 1f}
        }}
    };

    public static ElementType ConvertChargeTypeToElement(ChargeTypes charge)
    {
        switch (charge)
        {
            case ChargeTypes.fire:
                return ElementType.Fire;
            case ChargeTypes.water:
                return ElementType.Water;
            case ChargeTypes.earth:
                return ElementType.Earth;
            case ChargeTypes.wind:
                return ElementType.Wind;
            default:
                return ElementType.None;
        }
    }

    public static float CalculateDamage(Tuple<ElementType, ElementType> attackTypes, ElementType defenseType, float baseDamage, float attackFactor, float defenseFactor)
    {
        ElementType firstElement = attackTypes.Item1;
        ElementType secondElement = attackTypes.Item2;

        float damageMultiplier = damageTable[firstElement][defenseType] * damageTable[secondElement][defenseType];
        return baseDamage * damageMultiplier * attackFactor * defenseFactor;
    }

    public static float CalculateDamage(ChargeTuple attackTypes, ElementType defenseType, float baseDamage, float attackFactor, float defenseFactor)
    {
        ElementType firstElement = ConvertChargeTypeToElement(attackTypes.first());
        ElementType secondElement = ConvertChargeTypeToElement(attackTypes.second());

        float damageMultiplier = damageTable[firstElement][defenseType] * damageTable[secondElement][defenseType];
        return baseDamage * damageMultiplier * attackFactor * defenseFactor;
    }

    public static float CalculateDamage(ElementType attackType, ElementType defenseType, float baseDamage, float attackFactor, float defenseFactor)
    {
        float damageMultiplier = damageTable[attackType][defenseType];
        return baseDamage * damageMultiplier * attackFactor * defenseFactor;
    }

    public static void InstantiateSpell(GameObject spellObject, GameObject caster, Vector3 position, Quaternion rotation)
    {
        GameObject spell = GameObject.Instantiate(spellObject, position, rotation);
        spellObject.GetComponent<SpellEffect>().caster = caster;
    }

    // TODO: Add public method to apply a buff/debuff to a player using the BuffDebuff Manager

    private static void KnockBack(Rigidbody2D hit, Vector2 difference, float thrust, float knockTime)
    {
        difference = difference.normalized * thrust;
        hit.AddForce(difference, ForceMode2D.Impulse);

        hit.GetComponent<ICombatant>()?.GetKnocked(knockTime);
    }

    public static void ExecuteAttack(GameObject attacker, Rigidbody2D target, ChargeTuple attackTypes,
                                     Vector2 attackDirection, float baseDamage, float thrust, float knockTime)
    {
        var attackerScript = attacker.GetComponent<ICombatant>();
        var targetScript = target.GetComponent<ICombatant>();

        if (attackerScript != null && targetScript != null)
        {
            // Check if the target is invincible.
            if (targetScript.isInvincible)
            {
                return;
            }

            float attackFactor = attackerScript.attackFactor;
            float defenseFactor = targetScript.defenseFactor;

            float damage = CalculateDamage(attackTypes, targetScript.elementType, baseDamage, attackFactor, defenseFactor);
            Debug.Log($"{attacker.name} attacked {target.name} for {damage} damage!");

            // Apply knockback
            KnockBack(target, attackDirection, thrust, knockTime);

            // Apply damage
            var health = target.GetComponent<Health>();
            health.SubtractHealth(damage);
            // TODO: If target is dead, game manager might need to act
        }
        else
        {
            Debug.LogWarning("Either the caster or the target does not implement ICombatant.");
        }
    }

    public static void ExecuteAttack(GameObject attacker, Rigidbody2D target, ElementType attackType,
                                     Vector2 attackDirection, float baseDamage, float thrust, float knockTime)
    {
        var attackerScript = attacker.GetComponent<ICombatant>();
        var targetScript = target.GetComponent<ICombatant>();

        if (attackerScript != null && targetScript != null)
        {
            // Check if the target is invincible.
            if (targetScript.isInvincible)
            {
                return;
            }

            float attackFactor = attackerScript.attackFactor;
            float defenseFactor = targetScript.defenseFactor;

            float damage = CalculateDamage(attackType, targetScript.elementType, baseDamage, attackFactor, defenseFactor);
            Debug.Log($"{attacker.name} attacked {target.name} for {damage} damage!");

            // Apply knockback
            KnockBack(target, attackDirection, thrust, knockTime);

            // Apply damage
            var health = target.GetComponent<Health>();
            health.SubtractHealth(damage);
            // TODO: If target is dead, game manager might need to act
        }
        else
        {
            Debug.LogWarning("Either the caster or the target does not implement ICombatant.");
        }
    }

    public static void ExecuteAttack(GameObject attacker, Rigidbody2D target, Tuple<ElementType, ElementType> attackTypes,
                                     Vector2 attackDirection, float baseDamage, float thrust, float knockTime)
    {
        var attackerScript = attacker.GetComponent<ICombatant>();
        var targetScript = target.GetComponent<ICombatant>();

        if (attackerScript != null && targetScript != null)
        {
            // Check if the target is invincible.
            if (targetScript.isInvincible)
            {
                return;
            }

            float attackFactor = attackerScript.attackFactor;
            float defenseFactor = targetScript.defenseFactor;

            float damage = CalculateDamage(attackTypes, targetScript.elementType, baseDamage, attackFactor, defenseFactor);
            Debug.Log($"{attacker.name} attacked {target.name} for {damage} damage!");

            // Apply knockback
            KnockBack(target, attackDirection, thrust, knockTime);

            // Apply damage
            var health = target.GetComponent<Health>();
            health.SubtractHealth(damage);
            // TODO: If target is dead, game manager might need to act
        }
        else
        {
            Debug.LogWarning("Either the caster or the target does not implement ICombatant.");
        }
    }
}
