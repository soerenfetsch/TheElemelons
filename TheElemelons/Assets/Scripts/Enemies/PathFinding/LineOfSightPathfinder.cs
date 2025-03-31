using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A line-of-sight pathfinding system that dynamically calculates visibility 
/// and helps enemies navigate toward the player while avoiding obstacles.
/// </summary>
public class LineOfSightPathfinder : MonoBehaviour
{
    public Transform player; // Reference to the player.
    public LayerMask obstacleLayer; // Layer mask for obstacles.
    public TilePositionData tilePositionData; // Optional data to get floor positions.
    public HashSet<Vector2Int> floorPositions; // The possible map floor positions.
    public float playerSightRange = 10f; // Maximum range for reachable path for player.
    public bool drawDebugGizmos = false; // Toggle for drawing debug gizmos.
    private Dictionary<Vector2Int, bool> visibilityGrid; // Grid to track reachable positions to the player.
    private bool isGridInitialized = false; // Tracks if the visibility grid has been initialized.

    /// <summary>
    /// Initializes the floor positions based on the TilePositionData object.
    /// Gives a warning if the object is not set.
    /// </summary>
    [ContextMenu("Initialize Floor Positions")]
    public void InitializeFloorPositions()
    {
        if (tilePositionData == null)
        {
            Debug.LogWarning("TilePositionData object is not set.");
            return;
        }
        floorPositions = new HashSet<Vector2Int>(tilePositionData.tilePositions);
    }

    /// <summary>
    /// Initializes the visibility grid based on the MapData object.
    /// </summary>
    [ContextMenu("Initialize Visibility Grid")]
    public void InitializeVisibilityGrid()
    {
        if (floorPositions == null)
        {
            Debug.LogWarning("Floor position HashSet object is not set.");
            return;
        }
        visibilityGrid = new Dictionary<Vector2Int, bool>();
        foreach (Vector2Int position in floorPositions)
        {
            visibilityGrid[position] = false; // Initialize all positions as not visible.
        }
        isGridInitialized = true;
    }

    /// <summary>
    /// Updates the visibility grid based on the player's current position and obstacles.
    /// </summary>
    [ContextMenu("Update Visibility Grid")]
    public void UpdateVisibilityGrid()
    {
        if (!isGridInitialized)
        {
            Debug.LogWarning("Visibility grid has not been initialized. Call InitializeVisibilityGrid first.");
            return;
        }

        Vector2 playerPos = player.position;

        foreach (var position in visibilityGrid.Keys.ToList())
        {
            // Check if the position is within range first to avoid unnecessary raycasting.
            if (Vector2.Distance(playerPos, position) <= playerSightRange)
            {
                visibilityGrid[position] = IsPositionInSight(playerPos, position, playerSightRange);
            }
            else
            {
                visibilityGrid[position] = false;
            }
        }
    }

    /// <summary>
    /// Checks if the position is directly in line-of-sight from a given position.
    /// </summary>
    /// <param name="startPosition">The starting position to check from.</param>
    /// <param name="goalPosition">The goal position to check.</param>
    /// <param name="sightRange">The maximum range for line-of-sight.</param>
    /// <returns>True if the player is visible; otherwise, false.</returns>
    private bool IsPositionInSight(Vector2 startPosition, Vector2 goalPosition, float sightRange)
    {
        if (Vector2.Distance(startPosition, goalPosition) > sightRange)
        {
            return false; // The goal is out of range.
        }
        Vector2 direction = (goalPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, goalPosition);
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, distance, obstacleLayer);

        // Draw a debug ray to visualize the line-of-sight.
        if (drawDebugGizmos)
            Debug.DrawRay(startPosition, direction * distance, hit.collider ? Color.red : Color.green);

        // The player is visible if there's no hit or if the hit is the player itself.
        return hit.collider == null || hit.collider.transform == player;
    }

    /// <summary>
    /// Checks if the player is reachable from a start position by finding a visible tile in player reach.
    /// Returns the normalized vector toward the closest reachable tile.
    /// If no tile is reachable, returns Vector2.zero.
    /// </summary>
    /// <param name="startPosition">The starting position to check from.</param>
    /// <returns>A normalized vector toward the reachable tile, or Vector2.zero if no tile is reachable.</returns>
    public Vector2 CheckPlayerReachable(Vector2 startPosition, float sightRange)
    {
        if (!isGridInitialized)
        {
            Debug.LogWarning("Visibility grid has not been initialized. Call InitializeVisibilityGrid first.");
            return Vector2.zero;
        }

        // Get visible tile positions sorted by distance to the startPosition.
        var visibleTiles = visibilityGrid
            .Where(pair => pair.Value) // Only consider visible tiles.
            .Select(pair => pair.Key)
            .OrderBy(pos => Vector2.Distance(startPosition, pos));

        foreach (Vector2Int tile in visibleTiles)
        {
            // Check if the path to the tile is clear using raycasting.
            if (IsPositionInSight(startPosition, tile, sightRange))
            {
                return ((Vector2)tile - startPosition).normalized; // Return the direction to the tile.
            }
        }

        return Vector2.zero; // No reachable tile found.
    }

    /// <summary>
    /// Checks if the player is in line-of-sight from a start position.
    /// </summary>
    /// <param name="startPosition">The starting position to check from.</param>
    /// <param name="sightRange">The maximum range for line-of-sight.</param>
    /// <returns>A normalized vector toward the player, or Vector2.zero if no tile is reachable.</returns>
    public Vector2 CheckPlayerInSight(Vector2 startPosition, float sightRange)
    {
        if (!isGridInitialized)
        {
            Debug.LogWarning("Visibility grid has not been initialized. Call InitializeVisibilityGrid first.");
            return Vector2.zero;
        }

        Vector2 playerPos = player.position;

        // Check if the player is within range first to avoid unnecessary raycasting.
        if (IsPositionInSight(startPosition, playerPos, sightRange))
        {
            return (playerPos - startPosition).normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }

    /// <summary>
    /// Visualizes the visibility grid for debugging purposes.
    /// White circles indicate non-visible positions, red circles indicate visible positions.
    /// </summary>
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (visibilityGrid == null || !isGridInitialized || !drawDebugGizmos) return;

        foreach (var pair in visibilityGrid)
        {
            Vector3 position = new Vector3(pair.Key.x, pair.Key.y, 0); // Convert Vector2Int to Vector3 for drawing.
            Gizmos.color = pair.Value ? Color.red : Color.white; // Red if visible, white if not.
            Gizmos.DrawWireSphere(position, 0.1f); // Draw a circle with a small radius.
        }
#endif
    }

    private void Update()
    {
        if (!isGridInitialized) return;

        // Update the visibility grid every frame.
        UpdateVisibilityGrid();
    }

    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask("Obstacles", "Walls");
        InitializeFloorPositions();
        InitializeVisibilityGrid();
    }
}

