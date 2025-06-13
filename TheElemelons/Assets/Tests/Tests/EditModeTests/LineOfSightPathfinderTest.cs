using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LineOfSightPathfinderTests
{
    private GameObject player;
    private GameObject enemy;
    private LineOfSightPathfinder pathfinder;
    private GameObject floorTilemap;
    private GameObject wallTilemap;

    [SetUp]
    public void Setup()
    {
        // Create the player
        player = new GameObject("Player");
        player.transform.position = Vector3.zero;

        // Create the enemy
        enemy = new GameObject("Enemy");
        pathfinder = enemy.AddComponent<LineOfSightPathfinder>();
        pathfinder.player = player.transform;
        pathfinder.playerSightRange = 10f;
        pathfinder.floorPositions = new HashSet<Vector2>();

        // Create the floor tilemap
        floorTilemap = new GameObject("FloorTilemap");
        var floorTilemapRenderer = floorTilemap.AddComponent<TilemapRenderer>();
        var floorCollider = floorTilemap.AddComponent<TilemapCollider2D>();

        // Add floor positions
        pathfinder.floorPositions.Add(new Vector2(0, 0)); // Player position
        pathfinder.floorPositions.Add(new Vector2(1, 0)); // Nearby position
        pathfinder.floorPositions.Add(new Vector2(1, 1)); // Position around the wall
        pathfinder.floorPositions.Add(new Vector2(2, 0)); // Farther away

        // Create the wall tilemap
        wallTilemap = new GameObject("WallTilemap");
        wallTilemap.layer = LayerMask.NameToLayer("Walls");
        var wallTilemapRenderer = wallTilemap.AddComponent<TilemapRenderer>();
        var wallCollider = wallTilemap.AddComponent<TilemapCollider2D>();

        // Initialize pathfinder
        pathfinder.InitializeVisibilityGrid();
        pathfinder.obstacleLayer = LayerMask.GetMask("Walls");
        pathfinder.UpdateVisibilityGrid();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(floorTilemap);
        Object.DestroyImmediate(wallTilemap);
    }

    [Test]
    public void CheckPlayerInSight_PlayerInRangeAndInSight_ReturnsDirection()
    {
        // Enemy and player are in line of sight
        enemy.transform.position = new Vector3(1.2f, 0, 0);

        Vector2 result = pathfinder.CheckPlayerInSight(enemy.transform.position, pathfinder.playerSightRange);
        Assert.AreNotEqual(Vector2.zero, result, "Expected player to be in sight.");
    }

    [Test]
    public void CheckPlayerInSight_PlayerInRangeButNotInSight_ReturnsZero()
    {
        // Place a wall between enemy and player
        GameObject wall = new GameObject("Wall");
        wall.transform.position = new Vector3(0.5f, 0, 0);
        wall.AddComponent<BoxCollider2D>();
        wall.layer = LayerMask.NameToLayer("Walls");

        // Enemy and player are in range but blocked by wall
        enemy.transform.position = new Vector3(1, 0, 0);

        Vector2 result = pathfinder.CheckPlayerInSight(enemy.transform.position, pathfinder.playerSightRange);
        Assert.AreEqual(Vector2.zero, result, "Expected player to be out of sight due to the wall.");

        Object.DestroyImmediate(wall);
    }

    [Test]
    public void CheckPlayerInSight_PlayerOutOfRange_ReturnsZero()
    {
        // Enemy and player are out of range
        enemy.transform.position = new Vector3(11, 0, 0);

        Vector2 result = pathfinder.CheckPlayerInSight(enemy.transform.position, pathfinder.playerSightRange);
        Assert.AreEqual(Vector2.zero, result, "Expected player to be out of range.");
    }

    [Test]
    public void CheckPlayerReachable_PlayerDirectlyReachable_ReturnsDirection()
    {
        // Enemy and player are in line of sight and reachable
        enemy.transform.position = new Vector3(1.2f, 0, 0);

        Vector2 result = pathfinder.CheckPlayerReachable(enemy.transform.position, pathfinder.playerSightRange);
        Assert.AreNotEqual(Vector2.zero, result, "Expected player to be reachable directly.");
    }

    [Test]
    public void CheckPlayerReachable_PlayerReachableAroundWall_ReturnsDirection()
    {
        // Place a wall between enemy and player
        GameObject wall = new GameObject("Wall");
        wall.transform.position = new Vector3(0.5f, 0, 0);
        var collider = wall.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.5f, 0.5f); // Large enough to block the ray
        wall.layer = LayerMask.NameToLayer("Walls");

        // Enemy and player are not in direct line but reachable around wall
        enemy.transform.position = new Vector3(1.2f, 0, 0);

        Vector2 result = pathfinder.CheckPlayerReachable(enemy.transform.position, pathfinder.playerSightRange);
        Assert.AreNotEqual(Vector2.zero, result, "Expected player to be reachable around the wall.");

        Object.DestroyImmediate(wall);
    }

    [Test]
    public void CheckPlayerReachable_PlayerNotReachable_ReturnsZero()
    {
        // Enemy and player are completely out of range
        enemy.transform.position = new Vector3(20, 0, 0);

        Vector2 result = pathfinder.CheckPlayerReachable(enemy.transform.position, pathfinder.playerSightRange);
        Assert.AreEqual(Vector2.zero, result, "Expected player to be unreachable.");
    }
}

