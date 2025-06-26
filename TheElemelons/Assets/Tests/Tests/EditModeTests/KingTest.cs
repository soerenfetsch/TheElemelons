using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class KingTest
{
    private GameObject mockTarget;
    private King kingScript;


    [SetUp]
    public void Setup()
    {
        mockTarget = new GameObject();
        kingScript = mockTarget.AddComponent<King>();
        Rigidbody2D rigidBody = mockTarget.AddComponent<Rigidbody2D>();
        Animator animator = mockTarget.AddComponent<Animator>();
#if UNITY_EDITOR
        var ingamePanelPrefab = AssetDatabase.LoadAssetAtPath<Transform>("Assets/Prefabs/NPCPrefabs/King.prefab");
        Assert.NotNull(ingamePanelPrefab, "Ingame Panel prefab not found at specified path.");
#endif
        Animator prefabKingAnimator = ingamePanelPrefab.GetComponent<Animator>();
        animator.runtimeAnimatorController = prefabKingAnimator.runtimeAnimatorController;

        // Add the dialog data
        List<DialogueData> introDialogue = new();
        List<DialogueData> thanksDialogue = new();
        List<DialogueData> standardDialogue = new();
        List<DialogueData> endDialogue = new();
        kingScript.intro = introDialogue;
        kingScript.thankYou = thanksDialogue;
        kingScript.standardDialogue = standardDialogue;
        kingScript.endDialogue = endDialogue;
        kingScript.InitializeComponents(rigidBody, animator);

        // Add a game manager instance
        new GameObject("GameManager").AddComponent<GameManager>();

        kingScript.SetCurrentState(NPCStatus.Idle);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(mockTarget);
    }

    [Test]
    public void ActOnStatusIdle()
    {
        kingScript.SetCurrentState(NPCStatus.Idle);
        Vector3 position = mockTarget.transform.position;
        
        Assert.True(AreVectorsApproximatelyEqual(position, mockTarget.transform.position), "Idle king must not move");
    }

    [Test]
    public void ActOnStatusWalkHline()
    {
        kingScript.SetCurrentState(NPCStatus.Walking);
        kingScript.SetWalkPattern(WalkPattern.HLine);
        kingScript.SetWalkSpeed(10f);
        kingScript.SetWalkDist(1f);
        kingScript.TestCallFixedUpdate();
        
        Vector2 movement = kingScript.GetMovement();
        Assert.AreEqual(movement.x, 1, "king must move to the right");
        Assert.AreEqual(movement.y, 0, "king musnt move in y");
    }

    private static bool AreVectorsApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Vector2.Distance(a, b) < tolerance;
    }
}
