using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.Animations;

public class CollectableTest
{
    private GameObject player;
    private Health playerHealth;
    private PlayerInteraction playerInteraction;
    private CollectableHeart collectableScript;
    private GameObject collectable;
    private LayerMask interactableLayer;
    float previouPlayerHealth;
    private string animatorControllerPath = "Assets/TestAnimatorController.controller";

    [SetUp]
    public void Setup()
    {
        // Create player object with PlayerInteraction script
        player = new GameObject("Player");
        playerHealth = player.AddComponent<Health>();
        player.AddComponent<Rigidbody2D>().isKinematic = true;
        player.AddComponent<BoxCollider2D>().isTrigger = true;
        player.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
        player.tag = "Player";

        // Create interactable objects, assign layer, and add colliders
        collectable = new GameObject("Collectable");
        collectableScript = collectable.AddComponent<CollectableHeart>();
        collectable.AddComponent<Rigidbody2D>().isKinematic = false;
        collectable.GetComponent<Rigidbody2D>().gravityScale = 0;
        collectable.AddComponent<BoxCollider2D>().isTrigger = true;
        collectable.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
        collectable.AddComponent<SpriteRenderer>();
        collectable.AddComponent<Animator>();
        // Set up animator and wand animator variables
        var animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
        animatorController.AddParameter("heal", AnimatorControllerParameterType.Trigger);
        collectable.GetComponent<Animator>().runtimeAnimatorController = animatorController;

        collectableScript.shadowObject = new GameObject("Shadow");
        player.transform.position = collectable.transform.position + Vector3.left * 5;
    }
    [UnityTest]
    public IEnumerator TestOnCollect()
    {
        playerHealth.SubtractHealth(2f);
        previouPlayerHealth = playerHealth.GetHealth();
        player.transform.position = collectable.transform.position;
        yield return new WaitForSeconds(1f);
        Assert.IsTrue(collectable == null, "Collectable wasn't destroyed");
        Assert.AreEqual(previouPlayerHealth + 1, playerHealth.GetHealth(),
        "player should have been healed by one, but was healed by " + (playerHealth.GetHealth() - previouPlayerHealth).ToString());
    }
}
