using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.Animations;
using System;
using System.IO;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerMovementTests
{
    private GameObject playerGameObject;
    private Player player;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Animator animator;
    private Animator wandAnimator;
    private String animatorControllerPath = "Assets/TestAnimatorController.controller";


    [SetUp]
    public void Setup()
    {
        playerGameObject = new GameObject();
        player = playerGameObject.AddComponent<Player>();
        playerMovement = playerGameObject.AddComponent<PlayerMovement>();
        rb = playerGameObject.AddComponent<Rigidbody2D>();
        animator = playerGameObject.AddComponent<Animator>();

        GameObject childObject = new GameObject("Wand_Animation");
        wandAnimator = childObject.AddComponent<Animator>();
        childObject.transform.parent = playerGameObject.transform;

        playerMovement.Initialize();
        player.Initialize();

        // Set up animator and wand animator variables
        var animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
        animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);
        animator.runtimeAnimatorController = animatorController;
        wandAnimator.runtimeAnimatorController = animatorController;
    }

    [TearDown]
    public void Teardown()
    {
        // Destroy the GameObject after each test
        UnityEngine.Object.DestroyImmediate(playerGameObject);
        // Delete the temporary AnimatorController asset
        if (File.Exists(animatorControllerPath))
        {
            File.Delete(animatorControllerPath);
            Debug.Log($"Deleted temporary AnimatorController: {animatorControllerPath}");
        }
    }

    [Test]
    public void Player_Movement_SetsStateToWalk()
    {
        // Simulate movement
        playerMovement.SetMovement(new Vector2(1, 0)); // Move right
        player.SetCurrentState(PlayerState.Idle); // Set state to idle
        // Assert state changes to walk
        playerMovement.OnInputMove();
        Assert.AreEqual(PlayerState.Walk, player.GetCurrentState(), "Player state should change to walk when moving.");
    }

    [Test]
    public void Player_StopsMoving_SetsStateToIdle()
    {
        // Simulate stopping
        playerMovement.SetMovement(Vector2.zero); // Stop moving
        player.SetCurrentState(PlayerState.Walk); // Set state to walk
        // Assert state changes to idle
        playerMovement.OnInputMove();
        Assert.AreEqual(PlayerState.Idle, player.GetCurrentState(), "Player state should change to idle when stopping.");
    }

    [Test]
    public void Player_MoveSpeed_CalculationIsCorrect()
    {
        // Set baseSpeed and multiplier
        playerMovement.baseSpeed = 5f;
        playerMovement.speedMultiplier = 2f;

        // Assert moveSpeed calculation
        Assert.AreEqual(10f, playerMovement.GetMoveSpeed(), "MoveSpeed should equal baseSpeed multiplied by speedMultiplier.");
    }
}