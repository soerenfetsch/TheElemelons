using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoleTest
{
    private GameObject moleObject;
    private Mole mole;
    private LineOfSightPathfinder pathfinder;
    private Health health;
    private Mana mana;

    /// <summary>
    /// Sets up the test environment by creating a TreeStump instance, player mock, and pathfinder.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject to hold the TreeStump script
        moleObject = new GameObject("TreeStump");
        mole = moleObject.AddComponent<Mole>();
        mole.transform.position = Vector3.zero;

        // Add required components
        health = moleObject.AddComponent<Health>();
        mana = moleObject.AddComponent<Mana>();
        moleObject.AddComponent<Rigidbody2D>();
        moleObject.AddComponent<SpriteRenderer>();
        moleObject.AddComponent<Animator>();
        moleObject.AddComponent<FlashEffect>();
        moleObject.AddComponent<BoxCollider2D>(); // Add one collider
        moleObject.AddComponent<BoxCollider2D>(); // Add another collider for collisionBox
        mole.arm = new GameObject("Arm");
        mole.arm.AddComponent<FlashEffect>();
        mole.arm.SetActive(false);

        // Mocking the player's transform
        var playerObject = new GameObject("Player");
        playerObject.AddComponent<Player>();
        var wandObject = new GameObject("Wand_Animation");
        wandObject.transform.SetParent(playerObject.transform);
        wandObject.AddComponent<Animator>();
        playerObject.transform.position = Vector3.zero + Vector3.up * 2f; // Set player position above the mole

        var pathfinderObject = new GameObject("Pathfinder");
        pathfinder = pathfinderObject.AddComponent<LineOfSightPathfinder>();
        mole.Initialize(playerObject.transform, pathfinder);

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

        mole.rippleEffectPrefab = new GameObject("RippleEffect");
        mole.rippleEffectPrefab.AddComponent<RippleEffect>();

        mole.deathEffect = new GameObject("DeathEffect");

        // Explicitly call InitializeComponents for testing
        mole.InitializeComponents();
    }

    /// <summary>
    /// Cleans up the test environment by destroying created objects.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Clean up objects after each test
        Object.DestroyImmediate(moleObject);
    }

    [Test]
    public void MoleStartsIdleAndDugIn()
    {
        Assert.AreEqual(EnemyStatus.Idle, mole.GetCurrentState());
        Assert.IsTrue(mole.isInvincible);
    }

    [UnityTest]
    public IEnumerator MoleTransitionsToApproachingWhenPlayerInSight()
    {
        mole.ChangeState(EnemyStatus.Idle);
        mole.ActOnStatus();
        yield return null;
        Assert.AreEqual(EnemyStatus.Approaching, mole.GetCurrentState());
        Assert.IsTrue(mole.isInvincible);
    }

    [Test]
    public void MoleDigUpInApproachingSetsStateAttacking()
    {
        mole.ChangeState(EnemyStatus.Approaching);
        mole.OnDigUp();

        Assert.AreEqual(EnemyStatus.Attacking, mole.GetCurrentState());
        Assert.IsFalse(mole.isInvincible);
    }

    [Test]
    public void MoleDigDownSetsIdleState()
    {
        mole.OnDigDown();

        Assert.AreEqual(EnemyStatus.Idle, mole.GetCurrentState());
        Assert.IsTrue(mole.isInvincible);
    }

    [UnityTest]
    public IEnumerator MoleStaggersWhenHitAndRecovers()
    {
        mole.ChangeState(EnemyStatus.Approaching);
        mole.OnDigUp();
        float knockTime = 1f;

        mole.GetKnocked(knockTime);

        yield return null;
        Assert.AreEqual(EnemyStatus.Staggered, mole.GetCurrentState());

        yield return new WaitForSeconds(knockTime + 0.1f); // Wait for the knock time to pass
        Assert.AreNotEqual(EnemyStatus.Staggered, mole.GetCurrentState());
    }

    [UnityTest]
    public IEnumerator MoleDiesWhenHealthIsZero()
    {
        health.SetHealth(0);
        mole.ActOnHealth();
        yield return null;

        Assert.IsTrue(moleObject == null);
    }
}
