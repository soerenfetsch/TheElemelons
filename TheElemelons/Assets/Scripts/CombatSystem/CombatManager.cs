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

/// <summary>
/// Manages combat interactions, including damage calculations, attack execution, 
/// and spell instantiation in the game.
/// </summary>
public class CombatHelper : MonoBehaviour
{
    private static CombatHelper _instance;
    private CombatHelper() { } // Prevent instantiation

    /// <summary>
    /// Gets the singleton instance of the CombatHelper.
    /// If the instance does not exist, it creates a new GameObject and attaches the CombatHelper component to it.
    /// </summary>
    public static CombatHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject helperObject = new GameObject("CombatHelper");
                _instance = helperObject.AddComponent<CombatHelper>();
                DontDestroyOnLoad(helperObject);
            }
            return _instance;
        }
    }

    /// <summary>
    /// Starts a coroutine to handle temporary invincibility.
    /// </summary>
    /// <param name="target">The target to make invincible.</param>
    /// <param name="duration">The duration of invincibility.</param>
    public void MakeInvincible(ICombatant target, float duration)
    {
        StartCoroutine(InvincibilityRoutine(target, duration));
    }

    /// <summary>
    /// Coroutine to handle temporary invincibility.
    /// Sets the target's invincibility status to true, waits for the specified duration, and then sets it to false.
    /// </summary>
    /// <param name="target">The target to make invincible.</param>
    /// <param name="duration">The duration of invincibility.</param>
    private IEnumerator InvincibilityRoutine(ICombatant target, float duration)
    {
        target.isInvincible = true;
        yield return new WaitForSeconds(duration);
        target.isInvincible = false;
    }
}

/// <summary>
/// Manages combat interactions, including damage calculations, attack execution, 
/// and spell instantiation in the game.
/// </summary>
public static class CombatManager
{
    public const float invincibilityDuration = 0.2f;

    /// <summary>
    /// Event triggered when the player dies.
    /// This happens when the player is knocked and their health reaches zero.
    /// </summary>
    public static Action OnPlayerDeath;

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

    /// <summary>
    /// Converts a ChargeType to its corresponding ElementType.
    /// </summary>
    /// <param name="charge">The ChargeType to convert.</param>
    /// <returns>The corresponding ElementType.</returns>
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

    /// <summary>
    /// Calculates damage based on attack and defense element types and modifiers.
    /// Damage multiplier is calculated by multiplying the damage multipliers of the attacking elements.
    /// Fire: 1.25x against Earth, Earth: 1.25x against Wind, Water: 1.25x against Fire, Wind: 1.25x against Water.
    /// None: 1x against all elements, rest are 1x against each other.
    /// Final damage is calculated as baseDamage * damageMultiplier * attackFactor / defenseFactor.
    /// </summary>
    /// <param name="attackTypes">Tuple of attacking elements.</param>
    /// <param name="defenseType">Defending element.</param>
    /// <param name="baseDamage">Base damage of the attack.</param>
    /// <param name="attackFactor">Attack factor multiplier.</param>
    /// <param name="defenseFactor">Defense factor multiplier.</param>
    /// <returns>The calculated damage value.</returns>
    public static float CalculateDamage(Tuple<ElementType, ElementType> attackTypes, ElementType defenseType, float baseDamage, float attackFactor, float defenseFactor)
    {
        if (defenseFactor <= 0)
        {
            throw new ArgumentException("Defense factor cannot be less than or equal to 0.");
        }

        ElementType firstElement = attackTypes.Item1;
        ElementType secondElement = attackTypes.Item2;

        float damageMultiplier = damageTable[firstElement][defenseType] * damageTable[secondElement][defenseType];
        return baseDamage * damageMultiplier * attackFactor / defenseFactor;
    }

    /// <summary>
    /// Overload for CalculateDamage for ChargeTuple attack types.
    /// </summary>
    /// <param name="attackTypes">ChargeTuple of attack elements.</param>
    /// <param name="defenseType">Defending element.</param>
    /// <param name="baseDamage">Base damage of the attack.</param>
    /// <param name="attackFactor">Attack factor multiplier.</param>
    /// <param name="defenseFactor">Defense factor multiplier.</param>
    /// <returns>The calculated damage value.</returns>
    public static float CalculateDamage(ChargeTuple attackTypes, ElementType defenseType, float baseDamage, float attackFactor, float defenseFactor)
    {
        if (defenseFactor <= 0)
        {
            throw new ArgumentException("Defense factor cannot be less than or equal to 0.");
        }
        ElementType firstElement = ConvertChargeTypeToElement(attackTypes.first());
        ElementType secondElement = ConvertChargeTypeToElement(attackTypes.second());

        float damageMultiplier = damageTable[firstElement][defenseType] * damageTable[secondElement][defenseType];
        return baseDamage * damageMultiplier * attackFactor / defenseFactor;
    }

    /// <summary>
    /// Overload for CalculateDamage for single element attacks.
    /// </summary>
    /// <param name="attackType">Attacking element.</param>
    /// <param name="defenseType">Defending element.</param>
    /// <param name="baseDamage">Base damage of the attack.</param>
    /// <param name="attackFactor">Attack factor multiplier.</param>
    /// <param name="defenseFactor">Defense factor multiplier.</param>
    /// <returns>The calculated damage value.</returns>
    public static float CalculateDamage(ElementType attackType, ElementType defenseType, float baseDamage, float attackFactor, float defenseFactor)
    {
        if (defenseFactor <= 0)
        {
            throw new ArgumentException("Defense factor cannot be less than or equal to 0.");
        }
        float damageMultiplier = damageTable[attackType][defenseType];
        return baseDamage * damageMultiplier * attackFactor / defenseFactor;
    }

    /// <summary>
    /// Instantiates a spell object in the game world.
    /// Sets the caster on the instantiated object's SpellEffect component.
    /// </summary>
    /// <param name="spellObject">The prefab of the spell to instantiate.</param>
    /// <param name="caster">The GameObject casting the spell.</param>
    /// <param name="position">The position to instantiate the spell at.</param>
    /// <param name="rotation">The rotation of the instantiated spell.</param>
    public static void InstantiateSpell(GameObject spellObject, GameObject caster, Vector3 position, Quaternion rotation)
    {
        // Instantiate the spell object
        GameObject spell = GameObject.Instantiate(spellObject, position, rotation);
        // Set the caster on the instantiated object's SpellEffect component.
        SpellEffect spellEffect = spell.GetComponent<SpellEffect>();
        if (spellEffect != null)
        {
            spellEffect.caster = caster;
        }
    }

    // TODO: Add public method to apply a buff/debuff to a player using the BuffDebuff Manager

    /// <summary>
    /// Applies a knockback effect to a Rigidbody2D and if the object implements ICombatant, 
    /// calls the GetKnocked method of the object.
    /// </summary>
    /// <param name="hit">The Rigidbody2D to knock back.</param>
    /// <param name="difference">Direction of the knockback.</param>
    /// <param name="thrust">Strength of the knockback.</param>
    /// <param name="knockTime">Duration of the knockback effect.</param>
    private static void KnockBack(Rigidbody2D hit, Vector2 difference, float thrust, float knockTime)
    {
        difference = difference.normalized * thrust;
        hit.AddForce(difference, ForceMode2D.Impulse);

        hit.GetComponent<ICombatant>()?.GetKnocked(knockTime);
    }


    /// <summary>
    /// Executes an attack with elemental attributes (ChargeTuple attack types).
    /// Only executes attack if the target is not invincible.
    /// Calculates the attack damage, applies knockback, and subtracts health from the target.
    /// Target is marked as invincible for a short duration.
    /// </summary>
    /// <param name="attacker">The attacking GameObject.</param>
    /// <param name="target">The target's Rigidbody2D.</param>
    /// <param name="attackTypes">ChargeTuple of attack elements.</param>
    /// <param name="attackDirection">Direction of the attack.</param>
    /// <param name="baseDamage">Base damage of the attack.</param>
    /// <param name="thrust">Knockback strength.</param>
    /// <param name="knockTime">Knockback duration.</param>
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

            try
            {
                float damage = CalculateDamage(attackTypes, targetScript.elementType, baseDamage, attackFactor, defenseFactor);
                Debug.Log($"{attacker.name} attacked {target.name} for {damage} damage!");

                // Apply knockback
                KnockBack(target, attackDirection, thrust, knockTime);

                // Apply damage
                var health = target.GetComponent<Health>();
                health.SubtractHealth(damage);

                // Mark target as invincible for a short duration
                CombatHelper.Instance.MakeInvincible(targetScript, invincibilityDuration);

                // If player is dead, invoke the OnPlayerDeath event
                if (target.gameObject.CompareTag("Player") && health.IsZero())
                {
                    OnPlayerDeath?.Invoke();
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Either the caster or the target does not implement ICombatant.");
        }
    }

    /// <summary>
    /// Overload for single ElementType attack type. 
    /// Executes an attack with elemental attributes.
    /// Only executes attack if the target is not invincible.
    /// Calculates the attack damage, applies knockback, and subtracts health from the target.
    /// Target is marked as invincible for a short duration.
    /// </summary>
    /// <param name="attacker">The attacking GameObject.</param>
    /// <param name="target">The target's Rigidbody2D.</param>
    /// <param name="attackType">Attacking element.</param>
    /// <param name="attackDirection">Direction of the attack.</param>
    /// <param name="baseDamage">Base damage of the attack.</param>
    /// <param name="thrust">Knockback strength.</param>
    /// <param name="knockTime">Knockback duration.</param>
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

            try
            {
                float damage = CalculateDamage(attackType, targetScript.elementType, baseDamage, attackFactor, defenseFactor);
                Debug.Log($"{attacker.name} attacked {target.name} for {damage} damage!");

                // Apply knockback
                KnockBack(target, attackDirection, thrust, knockTime);

                // Apply damage
                var health = target.GetComponent<Health>();
                health.SubtractHealth(damage);

                // Mark target as invincible for a short duration
                CombatHelper.Instance.MakeInvincible(targetScript, invincibilityDuration);

                // If player is dead, invoke the OnPlayerDeath event
                if (target.gameObject.CompareTag("Player") && health.IsZero())
                {
                    OnPlayerDeath?.Invoke();
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Either the caster or the target does not implement ICombatant.");
        }
    }

    /// <summary>
    /// Overload for ElementType Tuple attack types.
    /// Executes an attack with elemental attributes.
    /// Only executes attack if the target is not invincible.
    /// Calculates the attack damage, applies knockback, and subtracts health from the target.
    /// Target is marked as invincible for a short duration.
    /// </summary>
    /// <param name="attacker">The attacking GameObject.</param>
    /// <param name="target">The target's Rigidbody2D.</param>
    /// <param name="attackTypes">Tuple of attack elements.</param>
    /// <param name="attackDirection">Direction of the attack.</param>
    /// <param name="baseDamage">Base damage of the attack.</param>
    /// <param name="thrust">Knockback strength.</param>
    /// <param name="knockTime">Knockback duration.</param>
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

            try
            {
                float damage = CalculateDamage(attackTypes, targetScript.elementType, baseDamage, attackFactor, defenseFactor);
                Debug.Log($"{attacker.name} attacked {target.name} for {damage} damage!");

                // Apply knockback
                KnockBack(target, attackDirection, thrust, knockTime);

                // Apply damage
                var health = target.GetComponent<Health>();
                health.SubtractHealth(damage);

                // Mark target as invincible for a short duration
                CombatHelper.Instance.MakeInvincible(targetScript, invincibilityDuration);

                // If player is dead, invoke the OnPlayerDeath event
                if (target.gameObject.CompareTag("Player") && health.IsZero())
                {
                    OnPlayerDeath?.Invoke();
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Either the caster or the target does not implement ICombatant.");
        }
    }
}
