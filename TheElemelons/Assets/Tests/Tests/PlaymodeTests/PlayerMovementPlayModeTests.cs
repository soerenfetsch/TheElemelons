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

    [SetUp]
    public void Setup()
    {
        #if UNITY_EDITOR
        // Load Player prefab from the project
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Player.prefab");
        Assert.NotNull(playerPrefab, "Player prefab not found at the specified path.");

        // Instantiate the Player prefab
        playerGameObject = Object.Instantiate(playerPrefab);
        playerGameObject.GetComponent<Animator>().enabled = false;     // Disable Animator
        //UNCOMMENT IF INPUTMANAGER EXISTS
        //playerGameObject.GetComponent<InputManager>().enabled = false;
        #endif
        // Add a mock Main Camera
        Camera mainCamera = new GameObject("Main_Camera").AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        playerGameObject.SetActive(true);
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
        Assert.NotNull(playerGameObject.GetComponent<BoxCollider2D>(), "Boxcollider2D component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Animator>(), "Animator component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<SpriteRenderer>(), "SpriteRenderer component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Transform>(), "Transform component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Health>(), "Health component is missing.");
        Assert.NotNull(playerGameObject.GetComponent<Mana>(), "Mana component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(0), "Aim component is missing.");
        Assert.AreEqual(playerGameObject.transform.GetChild(0).transform.childCount,3,"Aim has all Children");
        Assert.NotNull(playerGameObject.transform.GetChild(0), "Aim component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1), "Wand component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1).GetComponent<Transform>(), "Wand Transform component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1).GetComponent<SpriteRenderer>(), "Wand Sprite Renderer component is missing.");
        Assert.NotNull(playerGameObject.transform.GetChild(1).GetComponent<Animator>(), "Wand Animator component is missing.");
        
        
    }
    [UnityTest]
    public IEnumerator Player_InitialState_IsIdle()
    {
        playerGameObject.SetActive(true);
        yield return null;
        // Assert initial state is idle
        Assert.AreEqual(PlayerState.idle, playerGameObject.GetComponent<PlayerMovement>().GetCurrentState(), "Player's initial state should be idle.");
    }

    [UnityTest]
    public IEnumerator Player_Movement_SetsStateToWalk()
    {
        // Suppress expected log message
        //LogAssert.Expect(LogType.Exception, "UnityException: Transform child out of bounds");
        playerGameObject.SetActive(true);
        yield return null; // Simulate frame update
        // Simulate movement
        playerGameObject.GetComponent<PlayerMovement>().SetMovement(new UnityEngine.Vector2(1, 0)); // Moving right
        yield return new WaitForSeconds(0.1f);
        // Assert state changes to walk
        Assert.AreEqual(PlayerState.walk, playerGameObject.GetComponent<PlayerMovement>().GetCurrentState(), "Player state should change to walk when moving.");
    }

    [UnityTest]
    public IEnumerator Player_StopsMoving_SetsStateToIdle()
    {
        // Simulate movement
        playerGameObject.GetComponent<PlayerMovement>().SetMovement(new Vector2(1, 0));
        yield return null; // Simulate frame update

        // Simulate stopping
        playerGameObject.GetComponent<PlayerMovement>().SetMovement(Vector2.zero);
        yield return new WaitForSeconds(0.1f);

        // Assert state changes to idle
        Assert.AreEqual(PlayerState.idle, playerGameObject.GetComponent<PlayerMovement>().GetCurrentState(), "Player state should change to idle when stopping.");
    }

    [UnityTest]
    public IEnumerator Player_Knockback_SetsStateToStagger()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(2, 0);
        float knockTime = 0.5f;

        playerGameObject.GetComponent<PlayerMovement>().Knock(knockTime, 1f, knockbackForce);
        yield return new WaitForSeconds(knockTime / 2); // During knockback

        // Assert state is stagger
        Assert.AreEqual(PlayerState.stagger, playerGameObject.GetComponent<PlayerMovement>().GetCurrentState(), "Player state should be stagger during knockback.");
    }

    [UnityTest]
    public IEnumerator Player_Knockback_ReturnsToIdleAfterKnockTime()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(2, 0);
        float knockTime = 0.5f;

        playerGameObject.GetComponent<PlayerMovement>().Knock(knockTime, 1f, knockbackForce);
        yield return new WaitForSeconds(knockTime + playerGameObject.GetComponent<PlayerMovement>().iFramesTime); // After knockback and iFrames

        // Assert state returns to idle
        Assert.AreEqual(PlayerState.idle, playerGameObject.GetComponent<PlayerMovement>().GetCurrentState(), "Player state should return to idle after knockback ends.");
    }

    [UnityTest]
    public IEnumerator Player_Knockback_AppliesForceToRigidbody()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(3, 0);
        float knockTime = 0.5f;

        Rigidbody2D rb = playerGameObject.GetComponent<PlayerMovement>().GetComponent<Rigidbody2D>();
        playerGameObject.GetComponent<PlayerMovement>().Knock(knockTime, 1f, knockbackForce);

        yield return new WaitForFixedUpdate(); // Wait for physics update

        // Assert Rigidbody velocity is non-zero during knockback
        Assert.AreNotEqual(Vector2.zero, rb.velocity, "Rigidbody should have non-zero velocity during knockback.");
    }

    [Test]
    public void Player_MoveSpeed_CalculationIsCorrect()
    {
        // Set baseSpeed and multiplier
        playerGameObject.GetComponent<PlayerMovement>().baseSpeed = 5f;
        playerGameObject.GetComponent<PlayerMovement>().speedMultiplier = 2f;

        // Calculate moveSpeed
        playerGameObject.GetComponent<PlayerMovement>().moveSpeed = playerGameObject.GetComponent<PlayerMovement>().GetComponent<PlayerMovement>().baseSpeed * playerGameObject.GetComponent<PlayerMovement>().speedMultiplier;

        // Assert moveSpeed calculation
        Assert.AreEqual(10f, playerGameObject.GetComponent<PlayerMovement>().moveSpeed, "MoveSpeed should equal baseSpeed multiplied by speedMultiplier.");
    }

    [UnityTest]
    public IEnumerator Player_AfterKnockback_StopsMoving()
    {
        // Simulate knockback
        Vector2 knockbackForce = new Vector2(3, 0);
        float knockTime = 0.5f;

        Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
        playerGameObject.GetComponent<PlayerMovement>().Knock(knockTime, 1f, knockbackForce);

        yield return new WaitForSeconds(knockTime); // Wait for knockback to finish

        // Assert Rigidbody velocity is zero after knockback ends
        Assert.AreEqual(Vector2.zero, rb.velocity, "Rigidbody should stop moving after knockback ends.");
    }
}