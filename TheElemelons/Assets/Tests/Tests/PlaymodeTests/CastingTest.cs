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
    private Player playerMock;
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
        playerMock = Substitute.For<Player>();
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

}