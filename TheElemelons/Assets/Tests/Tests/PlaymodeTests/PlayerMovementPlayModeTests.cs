using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerMovementTests
{
    private GameObject playerGameObject;
    private PlayerMovement playerMovement;

    [SetUp]
    public void Setup()
    {
        #if UNITY_EDITOR
        // Load Player prefab from the project
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Player.prefab");
        Assert.NotNull(playerPrefab, "Player prefab not found at the specified path.");

        // Instantiate the Player prefab
        playerGameObject = Object.Instantiate(playerPrefab);
        playerGameObject.GetComponent<InputManager>().enabled = false;
        #endif
        // Add a mock Main Camera
        Camera mainCamera = new GameObject("Main_Camera").AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
    }
    
    [TearDown]
    public void Teardown()
    {
        // Destroy the GameObject after each test
        Object.DestroyImmediate(playerGameObject);
    }
    [Test]
    public void All_Components_Present()
    {
        Assert.NotNull(playerGameObject.GetComponent<PlayerMovement>(), "PlayerMovement component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Rigidbody2D>(), "Rigidbody2D component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<InputManager>(), "InputManager component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<BoxCollider2D>(), "Boxcollider2D component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Animator>(), "Animator component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<SpriteRenderer>(), "SpriteRenderer component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Transform>(), "Transform component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Health>(), "Health component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Mana>(), "Mana component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<InputManager>(), "InputManager component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(0), "Aim component is missing.");
        Assert.AreEqual(playerGameObject.transform.GetChild(0).transform.childCount,3,"Aim has all Children");
        Assert.NotNull(playerGameObject.transform.GetChild(0), "Aim component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1), "Wand component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1).GetComponent<Transform>(), "Wand Transform component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1).GetComponent<SpriteRenderer>(), "Wand Sprite Renderer component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1).GetComponent<Animator>(), "Wand Animator component is missing.");
        
        
    }
    [Test]
    public void Player_InitialState_IsIdle()
    {
        // Assert initial state is idle
        Assert.AreEqual(PlayerState.idle, playerMovement.currentState, "Player's initial state should be idle.");
    }

    [UnityTest]
    public IEnumerator Player_Movement_SetsStateToWalk()
    {
        // Suppress expected log message
        LogAssert.Expect(LogType.Exception, "UnityException: Transform child out of bounds");

        // Simulate movement
        playerMovement.movement = new Vector2(1, 0); // Moving right
        yield return null; // Simulate frame update
        // Assert state changes to walk
        Assert.AreEqual(PlayerState.walk, playerMovement.currentState, "Player state should change to walk when moving.");
    }

    [UnityTest]
    public IEnumerator Player_StopsMoving_SetsStateToIdle()
    {
        // Simulate movement
        playerMovement.movement = new Vector2(1, 0);
        yield return null; // Simulate frame update

        // Simulate stopping
        playerMovement.movement = Vector2.zero;
        yield return null; // Simulate another frame update

        // Assert state changes to idle
        Assert.AreEqual(PlayerState.idle, playerMovement.currentState, "Player state should change to idle when stopping.");
    }

    [UnityTest]
    public IEnumerator Player_Knockback_SetsStateToStagger()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(2, 0);
        float knockTime = 0.5f;

        playerMovement.Knock(knockTime, 1f, knockbackForce);
        yield return new WaitForSeconds(knockTime / 2); // During knockback

        // Assert state is stagger
        Assert.AreEqual(PlayerState.stagger, playerMovement.currentState, "Player state should be stagger during knockback.");
    }

    [UnityTest]
    public IEnumerator Player_Knockback_ReturnsToIdleAfterKnockTime()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(2, 0);
        float knockTime = 0.5f;

        playerMovement.Knock(knockTime, 1f, knockbackForce);
        yield return new WaitForSeconds(knockTime + playerMovement.iFramesTime); // After knockback and iFrames

        // Assert state returns to idle
        Assert.AreEqual(PlayerState.idle, playerMovement.currentState, "Player state should return to idle after knockback ends.");
    }

    [UnityTest]
    public IEnumerator Player_Knockback_AppliesForceToRigidbody()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(3, 0);
        float knockTime = 0.5f;

        Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
        playerMovement.Knock(knockTime, 1f, knockbackForce);

        yield return new WaitForFixedUpdate(); // Wait for physics update

        // Assert Rigidbody velocity is non-zero during knockback
        Assert.AreNotEqual(Vector2.zero, rb.velocity, "Rigidbody should have non-zero velocity during knockback.");
    }

    [Test]
    public void Player_MoveSpeed_CalculationIsCorrect()
    {
        // Set baseSpeed and multiplier
        playerMovement.baseSpeed = 5f;
        playerMovement.speedMultiplier = 2f;

        // Calculate moveSpeed
        playerMovement.moveSpeed = playerMovement.baseSpeed * playerMovement.speedMultiplier;

        // Assert moveSpeed calculation
        Assert.AreEqual(10f, playerMovement.moveSpeed, "MoveSpeed should equal baseSpeed multiplied by speedMultiplier.");
    }

    [UnityTest]
    public IEnumerator Player_AfterKnockback_StopsMoving()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(3, 0);
        float knockTime = 0.5f;

        Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
        playerMovement.Knock(knockTime, 1f, knockbackForce);

        yield return new WaitForSeconds(knockTime); // Wait for knockback to finish

        // Assert Rigidbody velocity is zero after knockback ends
        Assert.AreEqual(Vector2.zero, rb.velocity, "Rigidbody should stop moving after knockback ends.");
    }
}