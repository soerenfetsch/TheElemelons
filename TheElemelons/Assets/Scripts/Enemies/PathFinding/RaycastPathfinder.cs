using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RaycastPathfinder
{
    [Header("Pathfinding Settings")]
    public static float rayRange = 5f; // Maximum range of the rays.
    public static float rayAngleIncrement = 10f; // Angle increment between rays.
    public static LayerMask obstacleLayer = LayerMask.GetMask("Obstacles", "Walls"); // Layer mask for obstacles.

    [Header("Weights and Penalties")]
    public static float maxGoalWeight = 1f; // Weight for the ray perfectly aligned with the target.
    public static float maxPenalty = 1f; // Maximum penalty for a ray hitting a close obstacle.

    [Header("Debug")]
    public static bool drawDebugRays = false; // Toggle for drawing debug rays.

    /// <summary>
    /// Finds the best direction for an object to move toward a goal while avoiding obstacles.
    /// The function casts rays in a circle around the object and calculates a score for each ray.
    /// The score is based on the alignment with the goal and the distance to the closest obstacle.
    /// </summary>
    /// <param name="ownPosition">Position of the object.</param>
    /// <param name="goalPosition">Position of the goal (e.g., player).</param>
    /// <returns>Best direction as a normalized Vector2.</returns>
    public static Vector2 FindBestDirection(Vector2 ownPosition, Vector2 goalPosition)
    {
        Vector2 bestDirection = Vector2.zero;
        float bestScore = float.NegativeInfinity;

        Vector2 goalDirection = (goalPosition - ownPosition).normalized;

        for (float angle = 0; angle < 360; angle += rayAngleIncrement)
        {
            Vector2 rayDirection = AngleToVector2(angle);

            // Cast the ray
            RaycastHit2D hit = Physics2D.Raycast(ownPosition, rayDirection, rayRange, obstacleLayer);

            // Calculate goal weight
            float alignment = Vector2.Dot(rayDirection, goalDirection);
            float goalWeight = Mathf.Lerp(0, maxGoalWeight, (alignment + 1) / 2); // Normalize alignment [-1, 1] to [0, 1].

            // Calculate penalty
            float penalty = 0;
            if (hit.collider != null)
            {
                float distance = hit.distance;
                penalty = Mathf.Lerp(maxPenalty, 0, distance / rayRange); // Closer obstacles get higher penalties.
            }

            // Final score
            float score = goalWeight - penalty;

            // Keep track of the best direction
            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = rayDirection;
            }

            // Debug visualization
            if (drawDebugRays)
                Debug.DrawRay(ownPosition, rayDirection * score, hit.collider ? Color.red : Color.green);
        }

        return bestDirection;
    }

    /// <summary>
    /// Converts an angle in degrees to a normalized Vector2.
    /// </summary>
    /// <param name="angle">Angle in degrees.</param>
    /// <returns>Normalized direction vector.</returns>
    public static Vector2 AngleToVector2(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
