using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.Animations;
using System.Net.WebSockets;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CastingTest
{
    
/*
   private GameObject castingGameObject;
   private Casting casting; 
   private Mana manaScript;
   private UIManager uIManager;
   private GameObject spell;
   private PlayerMovement playerMovement;
   private Transform castpoint;
   private Animator wandAnimator;
   private AnimatorController controller;
   */
    private GameObject playerGameObject;
   private Casting casting; 
   private Mana manaScript;
   [SetUp]
    public void Setup()
    {
        #if UNITY_EDITOR
        var canvasPrefab = AssetDatabase.LoadAssetAtPath<Canvas>("Assets/UI_Canvas.prefab");
        Assert.NotNull(canvasPrefab, "UI_Canvas prefab not found at specified path.");

        // Load Player prefab from the project
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Player.prefab");
        Assert.NotNull(playerPrefab, "Player prefab not found at the specified path.");

        // Instantiate the Player prefab
        playerGameObject = Object.Instantiate(playerPrefab);
        playerGameObject.GetComponent<HealthBar>().enabled = false;    
        playerGameObject.GetComponent<ManaBar>().enabled = false; 
        manaScript = playerGameObject.GetComponent<Mana>();
        casting = playerGameObject.GetComponent<Casting>();
        casting.uIManager = canvasPrefab.GetComponent<UIManager>();
        
        #endif
        // Add a mock Main Camera
        Camera mainCamera = new GameObject("Main_Camera").AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        playerGameObject.SetActive(true);
        casting = playerGameObject.GetComponent<Casting>();
        manaScript = playerGameObject.GetComponent<Mana>();
    }
   /*
    [SetUp]
    public void Setup()
    {
        // Create HealthBar GameObject
        castingGameObject = new GameObject("Caster");
        spell = new GameObject("mockSpell");
        casting = castingGameObject.AddComponent<Casting>();
        manaScript = castingGameObject.AddComponent<Mana>();
        playerMovement = castingGameObject.AddComponent<PlayerMovement>();
        castingGameObject.AddComponent<BoxCollider2D>();
        spell.AddComponent<SpellEffect>();
        casting.noTypeSpell = spell;
        casting.castpoint = castpoint;
        // Creates the controller
        controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/testcontroller.controller");
        controller.AddParameter("Charge", AnimatorControllerParameterType.Bool);
        controller.AddParameter("RaiseWand", AnimatorControllerParameterType.Bool);
        controller.AddParameter("CanCast", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        controller.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        
        wandAnimator = castingGameObject.AddComponent<Animator>();
        wandAnimator.runtimeAnimatorController = controller;
        
        

        Object.Instantiate(castingGameObject);
        castingGameObject.SetActive(true);
    }
    */
    [TearDown]
    public void Teardown()
    {
        // Destroy the GameObject after each test
        Object.DestroyImmediate(playerGameObject);
    }
    [Test]
    public void OnSelectCharge_AddsCharge_WhenSlotIsEmpty()
    {
        // Act
        casting.OnSelectCharge(ChargeTypes.fire);
        int currentChargesCount = casting.GetCurrentCharges().Count();
        // Assert
        Assert.AreEqual(1, currentChargesCount, "Charge was not added to an empty slot.");
    }
    [Test]
    public void OnSelectCharge_DoesNotAddCharge_WhenSlotsAreFull()
    {
        // Fill both charge slots
        casting.OnSelectCharge(ChargeTypes.fire);
        casting.OnSelectCharge(ChargeTypes.water);

        // Act
        casting.OnSelectCharge(ChargeTypes.earth);

        int currentChargesCount = casting.GetCurrentCharges().Count();
        // Assert
        Assert.AreEqual(2, currentChargesCount, "A charge was added even when slots were full.");
    }
    /*
    [Test]
    public void ClearCharges_RemovesAllCharges()
    {
        // Add some charges
        casting.OnSelectCharge(ChargeTypes.fire);
        casting.OnSelectCharge(ChargeTypes.water);

        // Act
        casting.ClearCharges();
        int currentChargesCount = casting.GetCurrentCharges().Count();
        // Assert
        Assert.AreEqual(0, currentChargesCount, "Charges were not cleared.");
        Assert.IsNull(casting.charge1UI.sprite, "Charge1 UI sprite was not cleared.");
        Assert.IsNull(casting.charge2UI.sprite, "Charge2 UI sprite was not cleared.");
    }
    */
    [UnityTest]
    public IEnumerator DetermineSpell_ReturnsCorrectSpell_ForGivenChargeTuple()
    {
        manaScript.mana = 10f; // Ensure sufficient mana
        manaScript.maxMana = 10f;
        // Arrange
        casting.OnSelectCharge(ChargeTypes.notype);
        casting.OnSelectCharge(ChargeTypes.notype);

        // Act
        GameObject determinedSpell = casting.determineSpell();
        yield return null;
        // Assert
        Assert.AreEqual(casting.noTypeSpell, determinedSpell, "DetermineSpell did not return the correct spell for the given charge tuple.");
    }

    [UnityTest]
    public IEnumerator CastSpell_SuccessfullyCastsSpell_WhenRequirementsAreMet()
    {
        // Arrange
        manaScript.mana = 10f; // Ensure sufficient mana
        manaScript.maxMana = 10f;
        casting.OnSelectCharge(ChargeTypes.notype);
        casting.OnSelectCharge(ChargeTypes.notype);
        float expectedManaCost = casting.noTypeSpell.GetComponent<SpellEffect>().manaCost;
        // Mock castpoint
        var castpoint = new GameObject("CastPoint");
        casting.castpoint = castpoint.transform;

        // Act
        casting.CastSpell();
        yield return null;

        // Assert
        Assert.AreEqual(0, casting.GetCurrentChargesCount(), "Charges were not cleared after casting.");
        Assert.AreEqual(manaScript.mana, manaScript.maxMana-expectedManaCost, "Mana was not reduced correctly after casting."); // Assuming manaCost is 1
    }

    [UnityTest]
    public IEnumerator CastSpell_DoesNotCast_WhenRequirementsAreNotMet()
    {
        // Arrange
        manaScript.mana = 0.5f; // Insufficient mana
        manaScript.maxMana = 10f;
        casting.OnSelectCharge(ChargeTypes.fire);
        casting.OnSelectCharge(ChargeTypes.fire);

        // Mock castpoint
        var castpoint = new GameObject("CastPoint");
        casting.castpoint = castpoint.transform;

        // Act
        casting.CastSpell();
        yield return null;

        // Assert
        Assert.AreEqual(2, casting.GetCurrentChargesCount(), "Charges were cleared even though the spell was not cast.");
        Assert.AreEqual(manaScript.mana, 0.5f, "Mana was incorrectly reduced.");
    }
}
