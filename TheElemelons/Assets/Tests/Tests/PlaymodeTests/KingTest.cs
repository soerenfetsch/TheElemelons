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
        mockTarget.AddComponent<Rigidbody2D>();
        Animator animator = mockTarget.AddComponent<Animator>();
#if UNITY_EDITOR
        var ingamePanelPrefab = AssetDatabase.LoadAssetAtPath<Transform>("Assets/Prefabs/NPCPrefabs/King.prefab");
        Assert.NotNull(ingamePanelPrefab, "Ingame Panel prefab not found at specified path.");
#endif
        Animator prefabKingAnimator = ingamePanelPrefab.GetComponent<Animator>();
        animator.runtimeAnimatorController = prefabKingAnimator.runtimeAnimatorController;
        kingScript.SetCurrentState(NPCStatus.Idle);
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(mockTarget);
    }
    [UnityTest]
    public IEnumerator ActOnStatusIdle()
    {
        kingScript.SetCurrentState(NPCStatus.Idle);
        Vector3 position = mockTarget.transform.position;
        yield return null;
        Assert.AreEqual(position, mockTarget.transform.position, "Idle king must not move");
    }
    [UnityTest]
    public IEnumerator ActOnStatusWalkHline()
    {
        kingScript.SetCurrentState(NPCStatus.Walking);
        kingScript.SetWalkPattern(WalkPattern.HLine);
        kingScript.SetWalkSpeed(10f);
        kingScript.SetWalkDist(1f);
        kingScript.TestCallFixedUpdate();
        yield return null;
        Vector2 movement = kingScript.GetMovement();
        Assert.AreEqual(movement.x, 1, "king must move to the right");
        Assert.AreEqual(movement.y, 0, "king musnt move in y");
    }
}
