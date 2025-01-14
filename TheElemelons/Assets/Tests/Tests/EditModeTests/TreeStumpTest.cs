using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TreeStumpTests
{
    private GameObject treeStumpObject;
    private TreeLog treeStump;

    private LineOfSightPathfinder pathfinder;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject to hold the TreeStump script
        treeStumpObject = new GameObject("TreeStump");
        treeStump = treeStumpObject.AddComponent<TreeLog>();
        treeStump.transform.position = Vector3.zero;

        // Add required components
        treeStumpObject.AddComponent<Health>();
        treeStumpObject.AddComponent<Mana>();
        treeStumpObject.AddComponent<Rigidbody2D>();
        treeStumpObject.AddComponent<SpriteRenderer>();
        treeStumpObject.AddComponent<Animator>();
        treeStumpObject.AddComponent<BoxCollider2D>(); // Add one collider
        treeStumpObject.AddComponent<BoxCollider2D>(); // Add another collider for collisionBox

        // Mocking the player's transform
        var playerObject = new GameObject("Player");
        playerObject.transform.position = Vector3.zero;

        var pathfinderObject = new GameObject("Pathfinder");
        pathfinder = pathfinderObject.AddComponent<LineOfSightPathfinder>();
        treeStump.Initialize(playerObject.transform, pathfinder);

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
        pathfinder.player = playerObject.transform;
        pathfinder.InitializeVisibilityGrid();
        pathfinder.obstacleLayer = LayerMask.GetMask("Walls");
        pathfinder.UpdateVisibilityGrid();

        // Explicitly call InitializeComponents for testing
        treeStump.InitializeComponents();
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up objects after each test
        Object.DestroyImmediate(treeStumpObject);
    }

    [Test]
    public void TestTreeLogStart_InitializesStateToIdle()
    {
        // Assert initial state is Idle
        Assert.AreEqual(EnemyStatus.Idle, treeStump.GetCurrentState());
    }

    [Test]
    public void TestActOnStatus_TransitionsToApproaching_WhenPlayerWithinChaseRadius()
    {
        // Position the player within the chase radius
        treeStump.playerTransform.position = treeStumpObject.transform.position + new Vector3(treeStump.chaseRadius - 0.1f, 0);
        pathfinder.UpdateVisibilityGrid();

        // Call ActOnStatus
        treeStump.ActOnStatus();

        // Assert state transitioned to Approaching
        Assert.AreEqual(EnemyStatus.Approaching, treeStump.GetCurrentState());
    }

    [Test]
    public void TestActOnStatus_TransitionsToAttacking_WhenPlayerWithinAttackRadius()
    {
        // Position the player within the attack radius
        treeStump.playerTransform.position = treeStumpObject.transform.position + new Vector3(treeStump.attackRadius - 0.1f, 0);
        pathfinder.UpdateVisibilityGrid();

        // Call ActOnStatus
        treeStump.ActOnStatus();

        // Assert state transitioned to Attacking
        Assert.AreEqual(EnemyStatus.Attacking, treeStump.GetCurrentState());
    }

    [Test]
    public void TestActOnStatus_TransitionsToIdle_WhenPlayerOutOfChaseRadius()
    {
        // Set initial state to Approaching
        treeStump.ChangeState(EnemyStatus.Approaching);

        // Position the player outside the chase radius
        treeStump.playerTransform.position = treeStumpObject.transform.position + new Vector3(treeStump.chaseRadius + 10f, 0);
        pathfinder.UpdateVisibilityGrid();

        // Call ActOnStatus
        treeStump.ActOnStatus();

        // Assert state transitioned to Idle
        Assert.AreEqual(EnemyStatus.Retreating, treeStump.GetCurrentState());
    }
}
