using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.Animations;
using System.Net.WebSockets;
using Moq;
using NSubstitute;  // For mocking dependencies

[TestFixture]
public class CastingTests
{
    private Casting casting;
    private Mana manaMock;
    private PlayerMovement playerMovementMock;
    private UIManager uiManagerMock;
    private Animator wandAnimatorMock;

    private GameObject gameObject;
    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject to attach the Casting component
        gameObject = new GameObject("GameObject");

        // Create mock objects using NSubstitute
        manaMock = Substitute.For<Mana>();
        playerMovementMock = Substitute.For<PlayerMovement>();
        uiManagerMock = Substitute.For<UIManager>();
        wandAnimatorMock = Substitute.For<Animator>();

        // Create the Casting instance and inject dependencies
        casting = gameObject.AddComponent<Casting>();
        casting.manaScript = manaMock;
        casting.playerMovement = playerMovementMock;
        casting.uIManager = uiManagerMock;
        casting.wandAnimator = wandAnimatorMock;
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the GameObject after each test
        Object.DestroyImmediate(casting.gameObject);
    }
    
    // Example test for 'determineSpell' method
    [Test]
    public void TestDetermineSpell_WhenValidSpellExists_ReturnsSpellObject()
    {
        // Arrange
        ChargeTuple chargeTuple = new ChargeTuple(ChargeTypes.fire, ChargeTypes.water);
        SpellEffect spellEffectMock = Substitute.For<SpellEffect>();
        GameObject spellObjectMock = new GameObject();
        spellEffectMock.spellObject = spellObjectMock;

        casting.chargeToSpell.Add(chargeTuple, spellEffectMock);
        casting.currentCharges = chargeTuple;

        // Act
        GameObject result = casting.determineSpell();

        // Assert
        Assert.AreEqual(spellObjectMock, result);
    }
    
    [Test]
    public void TestDetermineSpell_WhenNoValidSpellExists_ReturnsNull()
    {
        // Arrange
        ChargeTuple chargeTuple = new ChargeTuple(ChargeTypes.fire, ChargeTypes.water);
        casting.currentCharges = chargeTuple;

        // Act
        GameObject result = casting.determineSpell();

        // Assert
        Assert.IsNull(result);
    }

    // Example test for 'CanCast' method
    [Test]
    public void TestCanCast_WhenManaIsSufficient_ReturnsTrue()
    {
        // Arrange
        casting.currentCharges = new ChargeTuple(ChargeTypes.fire, ChargeTypes.water);
        
        // Instead of mocking SpellEffect, add a real instance of SpellEffect
        SpellEffect spellEffect = gameObject.AddComponent<SpellEffect>();
        spellEffect.manaCost = 1f;  // Set mana cost to 1 for the spell
        
        // Add the spell effect to the chargeToSpell dictionary
        casting.AddSpellToDict(gameObject, casting.currentCharges);
        
        // Set mana to a sufficient amount
        manaMock.mana = 2f;  // Enough mana to cast the spell
        
        // Act
        bool result = casting.CanCast();
        
        // Assert
        Assert.IsTrue(result);
    }
    [Test]
    public void TestCanCast_WhenNotEnoughMana_ReturnsFalse()
    {
        // Arrange
        casting.currentCharges = new ChargeTuple(ChargeTypes.fire, ChargeTypes.water);
        SpellEffect spellEffectMock = Substitute.For<SpellEffect>();
        spellEffectMock.manaCost = 5f;
        casting.chargeToSpell.Add(casting.currentCharges, spellEffectMock);
        manaMock.mana = 2f;  // Not enough mana

        playerMovementMock.GetCurrentState().Returns(PlayerState.idle);

        // Act
        bool result = casting.CanCast();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void TestCanCast_WhenPlayerIsStaggered_ReturnsFalse()
    {
        // Arrange
        casting.currentCharges = new ChargeTuple(ChargeTypes.fire, ChargeTypes.water);
        SpellEffect spellEffectMock = Substitute.For<SpellEffect>();
        spellEffectMock.manaCost = 1f;
        casting.chargeToSpell.Add(casting.currentCharges, spellEffectMock);
        manaMock.mana = 2f;

        playerMovementMock.GetCurrentState().Returns(PlayerState.stagger);  // Player is staggered

        // Act
        bool result = casting.CanCast();

        // Assert
        Assert.IsFalse(result);
    }

    // Example test for 'OnSelectCharge' method
    [Test]
    public void TestOnSelectCharge_WhenChargeCanBeAdded_AddsCharge()
    {
        // Arrange
        casting.currentCharges = new ChargeTuple();
        ChargeTypes selectedCharge = ChargeTypes.fire;

        // Act
        casting.OnSelectCharge(selectedCharge);

        // Assert
        Assert.AreEqual(1, casting.GetCurrentChargesCount());
    }

    [Test]
    public void TestOnSelectCharge_WhenMaxChargesReached_DoesNotAddCharge()
    {
        // Arrange
        casting.currentCharges = new ChargeTuple(ChargeTypes.fire, ChargeTypes.water);
        ChargeTypes selectedCharge = ChargeTypes.earth;

        // Act
        casting.OnSelectCharge(selectedCharge);

        // Assert
        Assert.AreEqual(2, casting.GetCurrentChargesCount());
    }

    // Example test for 'ClearCharges' method
    [Test]
    public void TestClearCharges_ClearsCharges()
    {
        // Arrange
        casting.currentCharges.Add(ChargeTypes.fire);
        casting.currentCharges.Add(ChargeTypes.water);
        // Act
        casting.ClearCharges();

        // Assert
        Assert.AreEqual(0, casting.GetCurrentChargesCount());
    }
}