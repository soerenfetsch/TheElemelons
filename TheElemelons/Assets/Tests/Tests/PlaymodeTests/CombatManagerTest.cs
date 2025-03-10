using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;


public class CombatManagerTests
{
    private GameObject attacker;
    private GameObject target;
    private ICombatant attackerScript;
    private ICombatant targetScript;
    private Rigidbody2D targetRb;

    [SetUp]
    public void Setup()
    {
        // Setup attacker
        attacker = new GameObject("Attacker");
        attackerScript = attacker.AddComponent<TestCombatant>();
        attackerScript.attackFactor = 1.5f;

        // Setup target
        target = new GameObject("Target");
        targetScript = target.AddComponent<TestCombatant>();
        targetScript.defenseFactor = 1.25f;
        targetScript.elementType = ElementType.Fire;

        // Add Rigidbody2D for knockback
        targetRb = target.AddComponent<Rigidbody2D>();

        // Add Health component to the target
        target.AddComponent<Health>().SetHealth(100f);
        target.GetComponent<Health>().SetMaxHealth(100f);

        // Reset invincibility
        targetScript.isInvincible = false;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(attacker);
        Object.DestroyImmediate(target);
    }

    [Test]
    public void CalculateDamage_SingleElementType_ReturnsExpectedDamage()
    {
        float baseDamage = 50f;
        ElementType attackType = ElementType.Water;

        // Water is 1.25x effective against Fire
        float expectedDamage = baseDamage * 1.25f * attackerScript.attackFactor / targetScript.defenseFactor;

        float actualDamage = CombatManager.CalculateDamage(attackType, ElementType.Fire, baseDamage, attackerScript.attackFactor, targetScript.defenseFactor);

        Assert.AreEqual(expectedDamage, actualDamage, 0.01f, "Damage calculation failed for single ElementType.");
    }

    [Test]
    public void ExecuteAttack_MakesTargetInvincible()
    {
        // Simulate an attack
        Vector2 attackDirection = Vector2.up;
        float baseDamage = 50f;
        ChargeTuple attackTypes = new ChargeTuple(ChargeTypes.water, ChargeTypes.water);

        CombatManager.ExecuteAttack(attacker, targetRb, attackTypes, attackDirection, baseDamage, 5f, 0.5f);

        Assert.IsTrue(targetScript.isInvincible, "Target was not set to invincible after the attack.");
    }

    [UnityTest]
    public IEnumerator ExecuteAttack_TargetBecomesVulnerableAfterInvincibilityDuration()
    {
        // Simulate an attack
        Vector2 attackDirection = Vector2.up;
        float baseDamage = 50f;
        ChargeTuple attackTypes = new ChargeTuple(ChargeTypes.water, ChargeTypes.water);

        CombatManager.ExecuteAttack(attacker, targetRb, attackTypes, attackDirection, baseDamage, 5f, 0.5f);

        // Wait for invincibility duration + a small buffer
        yield return new WaitForSeconds(CombatManager.invincibilityDuration + 0.1f);

        Assert.IsFalse(targetScript.isInvincible, "Target did not become vulnerable after invincibility duration.");
    }

    [Test]
    public void ExecuteAttack_DoesNotApplyDamageToInvincibleTarget()
    {
        // Make the target invincible
        targetScript.isInvincible = true;

        // Simulate an attack
        Vector2 attackDirection = Vector2.up;
        float baseDamage = 50f;
        ChargeTuple attackTypes = new ChargeTuple(ChargeTypes.water, ChargeTypes.water);

        float initialHealth = target.GetComponent<Health>().GetHealth();

        CombatManager.ExecuteAttack(attacker, targetRb, attackTypes, attackDirection, baseDamage, 5f, 0.5f);

        float finalHealth = target.GetComponent<Health>().GetHealth();

        Assert.AreEqual(initialHealth, finalHealth, "Damage was applied to an invincible target.");
    }

    [Test]
    public void ExecuteAttack_AppliesCorrectDamage()
    {
        // Simulate an attack
        Vector2 attackDirection = Vector2.up;
        float baseDamage = 50f;
        ChargeTuple attackTypes = new ChargeTuple(ChargeTypes.water, ChargeTypes.earth);

        float initialHealth = target.GetComponent<Health>().GetHealth();

        CombatManager.ExecuteAttack(attacker, targetRb, attackTypes, attackDirection, baseDamage, 5f, 0.5f);

        float expectedDamage = CombatManager.CalculateDamage(
            attackTypes,
            ElementType.Fire,
            baseDamage,
            attackerScript.attackFactor,
            targetScript.defenseFactor
        );
        float finalHealth = target.GetComponent<Health>().GetHealth();

        Assert.AreEqual(initialHealth - expectedDamage, finalHealth, 0.01f, "Incorrect damage applied.");
    }

    [Test]
    public void ExecuteAttack_AppliesKnockback()
    {
        // Simulate an attack
        Vector2 attackDirection = Vector2.right;
        float baseDamage = 50f;
        ChargeTuple attackTypes = new ChargeTuple(ChargeTypes.water, ChargeTypes.earth);
        float thrust = 10f;

        CombatManager.ExecuteAttack(attacker, targetRb, attackTypes, attackDirection, baseDamage, thrust, 0.5f);

        // Check if the target has been moved by knockback
        Assert.AreNotEqual(Vector2.zero, targetRb.velocity, "Target did not experience knockback.");
    }
}

