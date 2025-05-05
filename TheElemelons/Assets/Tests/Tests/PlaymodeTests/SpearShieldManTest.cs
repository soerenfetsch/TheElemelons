using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;

public class SpearShieldManTests
{
    private GameObject testEnemy;
    private GameObject testSpear;
    private GameObject testShield;
    private SpearShieldMan spearShieldMan;
    private LineOfSightPathfinder pathfinder;
    private string animatorControllerPath = "Assets/TestAnimatorController.controller";

    [SetUp]
    public void Setup()
    {
        testEnemy = new GameObject("TestEnemy");
        spearShieldMan = testEnemy.AddComponent<SpearShieldMan>();

        // Required components
        testEnemy.AddComponent<Health>();
        testEnemy.AddComponent<Mana>();
        testEnemy.AddComponent<Animator>();
        testEnemy.AddComponent<Rigidbody2D>();
        testEnemy.AddComponent<BoxCollider2D>(); // Add one collider
        testEnemy.AddComponent<BoxCollider2D>(); // Add another collider for collisionBox
        testEnemy.AddComponent<SpriteRenderer>();
        testEnemy.AddComponent<FlashEffect>();

        // Create mock prefabs
        testSpear = new GameObject("SpearPrefab");
        testSpear.AddComponent<Spear>();
        testSpear.AddComponent<Animator>();
        testSpear.AddComponent<SpriteRenderer>();
        testSpear.AddComponent<Rigidbody2D>();
        testSpear.AddComponent<BoxCollider2D>();
        GameObject spearCollider = new GameObject("SpearCollider");
        spearCollider.AddComponent<ShieldSpearCollider>();
        for (int i = 0; i < 4; i++)
        {
            spearCollider.AddComponent<PolygonCollider2D>();
        }
        spearCollider.transform.parent = testSpear.transform;
        spearShieldMan.spearPrefab = testSpear;

        testShield = new GameObject("ShieldPrefab");
        testShield.AddComponent<Animator>();
        testShield.AddComponent<SpriteRenderer>();
        GameObject shieldCollider = new GameObject("ShieldCollider");
        shieldCollider.AddComponent<ShieldSpearCollider>();
        for (int i = 0; i < 4; i++)
        {
            shieldCollider.AddComponent<PolygonCollider2D>();
        }
        shieldCollider.transform.parent = testShield.transform;
        spearShieldMan.shieldPrefab = testShield;

        // Set up animator and wand animator variables
        var animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
        animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        testEnemy.GetComponent<Animator>().runtimeAnimatorController = animatorController;

        // Mock Pathfinder and Player
        GameObject player = new GameObject("Player");
        GameObject pathfinderObject = new GameObject("Pathfinder");
        pathfinder = pathfinderObject.AddComponent<LineOfSightPathfinder>();

        // Initialize visibility grid for pathfinder
        pathfinder.playerSightRange = 3f;
        pathfinder.floorPositions = new HashSet<Vector2Int>();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                pathfinder.floorPositions.Add(new Vector2Int(x, y));
            }
        }
        pathfinder.player = player.transform;
        pathfinder.InitializeVisibilityGrid();
        pathfinder.obstacleLayer = LayerMask.GetMask("Walls");
        pathfinder.UpdateVisibilityGrid();

        // Initialize the SpearShieldMan
        spearShieldMan.Initialize(player.transform, pathfinder);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testEnemy);
        Object.DestroyImmediate(GameObject.Find("SpearPrefab"));
        Object.DestroyImmediate(GameObject.Find("ShieldPrefab"));
    }

    [Test]
    public void RandomizeSpearShield_InstantiatesCorrectPrefab()
    {
        spearShieldMan.RandomizeSpearShield();

        int count = testEnemy.transform.childCount;
        Assert.IsTrue(count >= 1 && count <= 2, "Should have spawned spear, shield, or both.");
        Assert.IsTrue(spearShieldMan.spearInstance != null || spearShieldMan.shieldInstance != null,
                      "Spear or shield instance should not be null.");
    }

    [Test]
    public void ChangeState_TransitionsCorrectly()
    {
        spearShieldMan.ChangeState(EnemyStatus.Approaching);
        Assert.AreEqual(EnemyStatus.Approaching, spearShieldMan.GetCurrentState());
    }

    [Test]
    public void ActOnHealth_EnablesSpearThrowWhenLowHealth()
    {
        spearShieldMan.InitializeComponents();
        spearShieldMan.spearInstance = Object.Instantiate(testSpear, testEnemy.transform);
        spearShieldMan.spearInstance.GetComponent<Spear>().enabled = false;

        var health = spearShieldMan.GetComponent<Health>();
        health.SetMaxHealth(10);
        health.SetHealth(2); // Below 25%

        spearShieldMan.ActOnHealth();

        Assert.IsTrue(spearShieldMan.spearInstance.GetComponent<Spear>().enabled);
    }

    [UnityTest]
    public IEnumerator AttackWithSpear_EnablesAndDisablesCollider()
    {
        spearShieldMan.InitializeComponents();
        spearShieldMan.spearInstance = Object.Instantiate(testSpear, testEnemy.transform);
        spearShieldMan.spearInstance.GetComponent<Spear>().enabled = false;

        var spearCollider = spearShieldMan.spearInstance.GetComponentInChildren<ShieldSpearCollider>();
        Assert.IsNotNull(spearCollider, "Spear collider should not be null.");

        spearShieldMan.StartCoroutine(spearShieldMan.AttackWithSpear());
        Assert.IsTrue(spearCollider.IsEnabled(), "Spear collider should be enabled after attack.");

        yield return new WaitForSeconds(2f); // Wait for the attack animation to finish

        Assert.IsFalse(spearCollider.IsEnabled(), "Spear collider should be disabled after attack.");
    }

    [UnityTest]
    public IEnumerator AttackWithShield_EnablesAndDisablesCollider()
    {
        spearShieldMan.InitializeComponents();
        spearShieldMan.shieldInstance = Object.Instantiate(testShield, testEnemy.transform);
        spearShieldMan.shieldInstance.GetComponent<Animator>().enabled = false; // Disable animator for testing

        var shieldCollider = spearShieldMan.shieldInstance.GetComponentInChildren<ShieldSpearCollider>();
        Assert.IsNotNull(shieldCollider, "Shield collider should not be null.");

        spearShieldMan.StartCoroutine(spearShieldMan.AttackWithShield());
        Assert.IsTrue(shieldCollider.IsEnabled(), "Shield collider should be enabled after attack.");

        yield return new WaitForSeconds(2f); // Wait for the attack animation to finish

        Assert.IsFalse(shieldCollider.IsEnabled(), "Shield collider should be disabled after attack.");
    }

    /// <summary>
    /// Tests if the SpearShieldMan initializes with the Idle state.
    /// </summary>
    [Test]
    public void TestStart_InitializesStateToIdle()
    {
        spearShieldMan.InitializeComponents();
        // Assert initial state is Idle
        Assert.AreEqual(EnemyStatus.Idle, spearShieldMan.GetCurrentState());
    }

    /// <summary>
    /// Tests that the SpearShieldMan transitions to Approaching when the player is within the chase radius.
    /// </summary>
    [Test]
    public void TestActOnStatus_TransitionsToApproaching_WhenPlayerWithinChaseRadius()
    {
        spearShieldMan.InitializeComponents();
        // Position the player within the chase radius
        spearShieldMan.playerTransform.position = spearShieldMan.transform.position + new Vector3(spearShieldMan.chaseRadius - 0.1f, 0);
        pathfinder.UpdateVisibilityGrid();

        // Call ActOnStatus
        spearShieldMan.ActOnStatus();

        // Assert state transitioned to Approaching
        Assert.AreEqual(EnemyStatus.Approaching, spearShieldMan.GetCurrentState());
    }

    /// <summary>
    /// Tests that the SpearShieldMan transitions to Attacking when the player is within the attack radius
    /// and the SpearShieldMan is facing the player
    /// </summary>
    [Test]
    public void TestActOnStatus_TransitionsToAttacking_WhenPlayerWithinAttackRadiusAndFacing()
    {
        spearShieldMan.InitializeComponents();
        // Position the player within the attack radius
        spearShieldMan.playerTransform.position = testEnemy.transform.position + new Vector3(spearShieldMan.attackRadius - 0.1f, 0);
        pathfinder.UpdateVisibilityGrid();

        spearShieldMan.ChangeState(EnemyStatus.Approaching); // Set initial state to Approaching
        // Set animator direction to the right
        spearShieldMan.GetComponent<Animator>().SetFloat("Horizontal", 1f);
        spearShieldMan.GetComponent<Animator>().SetFloat("Vertical", 0f);
        Debug.Log("Animator Horizontal: " + spearShieldMan.GetComponent<Animator>().GetFloat("Horizontal"));
        Debug.Log("Animator Vertical: " + spearShieldMan.GetComponent<Animator>().GetFloat("Vertical"));

        // Call ActOnStatus
        spearShieldMan.ActOnStatus();

        // Assert state transitioned to Attacking
        Assert.AreEqual(EnemyStatus.Attacking, spearShieldMan.GetCurrentState());
    }

    /// <summary>
    /// Tests that the SpearShieldMan does not transition to Attacking when the player is within the attack radius
    /// but the SpearShieldMan is not facing the player
    /// </summary>
    [Test]
    public void TestActOnStatus_NotTransitionsToAttacking_WhenPlayerWithinAttackRadiusButNotFacing()
    {
        spearShieldMan.InitializeComponents();
        // Position the player within the attack radius
        spearShieldMan.playerTransform.position = testEnemy.transform.position + new Vector3(spearShieldMan.attackRadius - 0.1f, 0);
        pathfinder.UpdateVisibilityGrid();

        spearShieldMan.ChangeState(EnemyStatus.Approaching); // Set initial state to Approaching
        // Set animator direction to the left
        spearShieldMan.GetComponent<Animator>().SetFloat("Horizontal", -1f);
        spearShieldMan.GetComponent<Animator>().SetFloat("Vertical", 0f);
        Debug.Log("Animator Horizontal: " + spearShieldMan.GetComponent<Animator>().GetFloat("Horizontal"));
        Debug.Log("Animator Vertical: " + spearShieldMan.GetComponent<Animator>().GetFloat("Vertical"));

        // Call ActOnStatus
        spearShieldMan.ActOnStatus();

        // Assert state transitioned to Attacking
        Assert.AreEqual(EnemyStatus.Approaching, spearShieldMan.GetCurrentState());
    }

    /// <summary>
    /// Tests that the TreeStump transitions to Retreating when the player leaves the chase radius.
    /// </summary>
    [Test]
    public void TestActOnStatus_TransitionsToIdle_WhenPlayerOutOfChaseRadius()
    {
        spearShieldMan.InitializeComponents();
        // Set initial state to Approaching
        spearShieldMan.ChangeState(EnemyStatus.Approaching);

        // Position the player outside the chase radius
        spearShieldMan.playerTransform.position = testEnemy.transform.position + new Vector3(spearShieldMan.chaseRadius + 10f, 0);
        pathfinder.UpdateVisibilityGrid();

        // Call ActOnStatus
        spearShieldMan.ActOnStatus();

        // Assert state transitioned to Idle
        Assert.AreEqual(EnemyStatus.Idle, spearShieldMan.GetCurrentState());
    }
}

