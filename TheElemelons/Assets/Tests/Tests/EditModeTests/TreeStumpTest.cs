using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TreeStumpTests
{
    private GameObject treeStumpObject;
    private TreeLog treeStump;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject to hold the TreeStump script
        treeStumpObject = new GameObject("TreeStump");
        treeStump = treeStumpObject.AddComponent<TreeLog>();

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
        treeStump.Initialize(playerObject.transform);

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
        treeStump.playerTransform.position = treeStumpObject.transform.position + new Vector3(treeStump.chaseRadius + 1f, 0);

        // Call ActOnStatus
        treeStump.ActOnStatus();

        // Assert state transitioned to Idle
        Assert.AreEqual(EnemyStatus.Retreating, treeStump.GetCurrentState());
    }
}
