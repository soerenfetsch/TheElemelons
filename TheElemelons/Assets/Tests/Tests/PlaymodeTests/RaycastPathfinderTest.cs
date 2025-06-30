using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class RaycastPathfinderPlayModeTests
{
    private LayerMask testLayer;

    [SetUp]
    public void SetUp()
    {
        // Create a test layer for obstacles.
        testLayer = LayerMask.GetMask("Default");

        // Set up static RaycastPathfinder parameters.
        RaycastPathfinder.rayRange = 5f;
        RaycastPathfinder.rayAngleIncrement = 45f;
        RaycastPathfinder.obstacleLayer = testLayer;
        RaycastPathfinder.maxGoalWeight = 1f;
        RaycastPathfinder.maxPenalty = 1f;
    }

    [UnityTest]
    public IEnumerator FindBestDirection_NoObstacles_ReturnsGoalDirection()
    {
        // Arrange
        Vector2 ownPosition = new Vector2(100f, 100f);
        Vector2 goalPosition = new Vector2(101f, 101f);

        // Act
        Vector2 bestDirection = RaycastPathfinder.FindBestDirection(ownPosition, goalPosition);

        // Assert
        Vector2 expectedDirection = (goalPosition - ownPosition).normalized;
        Assert.IsTrue(AreVectorsApproximatelyEqual(expectedDirection, bestDirection),
            $"Expected {expectedDirection}, but got {bestDirection}");

        yield return null;
    }

    [UnityTest]
    public IEnumerator FindBestDirection_WithObstacle_AvoidsObstacle()
    {
        // Arrange
        Vector2 ownPosition = new Vector2(100, 100);
        Vector2 goalPosition = new Vector2(103, 103);

        // Create a test obstacle.
        GameObject obstacle = new GameObject("Obstacle");
        BoxCollider2D collider = obstacle.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 1f); // Large enough to block the ray
        obstacle.transform.position = new Vector2(101.5f, 101.5f);
        obstacle.layer = LayerMask.NameToLayer("Default");

        Physics2D.SyncTransforms(); // Ensure the physics system is updated before testing.
        yield return new WaitForFixedUpdate(); // Wait for physics updates.
        yield return new WaitForFixedUpdate();
        
        // Act
        Vector2 bestDirection = RaycastPathfinder.FindBestDirection(ownPosition, goalPosition);

        yield return new WaitForFixedUpdate(); // Wait for physics updates.

        // Assert
        Vector2 normallyExpectedDirection = (goalPosition - ownPosition).normalized;
        Assert.IsFalse(AreVectorsApproximatelyEqual(normallyExpectedDirection, bestDirection),
            "Pathfinder should avoid obstacles.");
        Assert.IsFalse(AreVectorsApproximatelyEqual(Vector2.zero, bestDirection),
            "Pathfinder should return a non-zero direction.");

        Object.Destroy(obstacle); // Clean up the test object.
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up any remaining objects after each test.
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith("Obstacle"))
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    private static bool AreVectorsApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Vector2.Distance(a, b) < tolerance;
    }
}
