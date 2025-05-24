using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WitchTest
{
    private GameObject testEnemy;
    Witch witchScript;
    private LineOfSightPathfinder pathfinder;
    /*
    [SetUp]
    public void Setup()
    {
        testEnemy = new GameObject("TestEnemy");
        witchScript = testEnemy.AddComponent<Witch>();
        // Required components
        testEnemy.AddComponent<WitchHealth>();
        testEnemy.AddComponent<Mana>();
        testEnemy.AddComponent<Animator>();
        testEnemy.AddComponent<Rigidbody2D>();
        testEnemy.AddComponent<BoxCollider2D>(); // Add one collider
        testEnemy.AddComponent<BoxCollider2D>(); // Add another collider for collisionBox
        testEnemy.AddComponent<SpriteRenderer>();
        testEnemy.AddComponent<FlashEffect>();
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
        witchScript.InitializeGivenPlayerPathfinder(player.transform, pathfinder);
        witchScript.InitializeGeneral();
        witchScript.InitializeDice();
        for (int i = 0; i < witchScript.dice.Count; i++)
        {
            witchScript.dice[i] = 0f;
        }
        witchScript.InitilializeMoveStats();
    }
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testEnemy);
    }
*/
    [SetUp]
    public void Setup()
    {
        GameObject testEnemy = new GameObject("testEnemy");
        testEnemy.AddComponent<Witch>();
        witchScript = testEnemy.AddComponent<Witch>();
        witchScript.InitializeDice();
        for (int i = 0; i < witchScript.dice.Count; i++)
        {
            witchScript.dice[i] = UnityEngine.Random.Range(0, 1);
        }
        witchScript.InitilializeMoveStats();
    }

    [Test]
    public void TestChose()
    {
        List<WitchMoves> selection = new List<WitchMoves>() { WitchMoves.Dash, WitchMoves.Circle, WitchMoves.Vortex };
        WitchMoves pick = witchScript.Chose(selection, executeChoice: false);
        Assert.True(selection.Contains(pick), "Invalid choice");
    }
    [Test]
    public void TestNewHitRaycast()
    {
        witchScript.SetMovement(Vector2.left);
        witchScript.NewHitRayCast();
        Assert.IsFalse(witchScript.GetHit(), "hit even though no wall");
    }
    [Test]
    public void TestPickStraightDir()
    {
        Vector2 dir = witchScript.PickStraightDir();
        List<Vector2> straightLines = new List<Vector2>() { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
        Assert.IsTrue(straightLines.Contains(dir), "direction not horizontal or vertical");
    }
    [Test]
    public void TestGetMoveSpeed()
    {
        witchScript.ChangeWitchState(WitchState.Walk);
        Assert.AreEqual(witchScript.GetWalkSpeed(), witchScript.GetMoveSpeed(), "walkspeed incorrect");
        witchScript.ChangeWitchState(WitchState.Hover);
        Assert.AreEqual(witchScript.GetHoverSpeed(), witchScript.GetHoverSpeed(), "hoverspeed incorrect");
        witchScript.ChangeWitchState(WitchState.Bald);
        Assert.AreEqual(witchScript.GetHoverSpeed(), witchScript.GetHoverSpeed(), "baldspeed incorrect");
    }
    [Test]
    public void TestChoseSpellFromPriorityBatches()
    {
        for (int i = 0; i < 20; i++)
        {
            List<WitchMoves> choice1 = new List<WitchMoves>() { WitchMoves.Vortex };
            List<WitchMoves> choice2 = new List<WitchMoves>() { WitchMoves.Dash, WitchMoves.Circle };
            List<WitchMoves> choice3 = new List<WitchMoves>() { };
            List<float> priorities = new List<float>() { 1f, 0f, 10f };
            WitchMoves pick = witchScript.ChoseSpellFromPriorityBatches(new List<List<WitchMoves>>() { choice1, choice2, choice3 }, priorities, executeChoice: false);
            Assert.AreEqual(WitchMoves.Vortex, pick, "choice2 is impossible as priority is 0, choice 3 is empty");
        }
    }
}
