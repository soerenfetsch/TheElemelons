using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Controls the procedural generation and visualization of the dungeon.
/// Handles clearing previous dungeons, generating new maps, and initializing pathfinding and monster management.
=======
/// Main controller for procedural dungeon generation and visualization.
/// Handles the generation, clearing, and initialization of dungeon data, tilemaps, monsters, and pathfinding.
>>>>>>> Stashed changes
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    /// <summary>
    /// Starting position for dungeon generation.
    /// </summary>
    [SerializeField] protected Vector2Int startPosition;

    /// <summary>
    /// The Tilemap visualizer that handles the visual representation of the dungeon.
    /// </summary>
    [SerializeField] protected AbstractTilemapVisualizer tilemapVisualizer;

    /// <summary>
    /// The map generator that creates the lists of positions for a map.
    /// </summary>
    [SerializeField] protected AbstractMapGenerator mapGenerator;

    /// <summary>
    /// The monster manager responsible for spawning and clearing monsters.
    /// </summary>
    [SerializeField] protected MonsterManager monsterManager;

    /// <summary>
    /// The current map data generated for the dungeon.
    /// </summary>
    [SerializeField] public MapData mapData;

    /// <summary>
    /// Stores the center positions of all floor tiles for use in pathfinding and other systems.
    /// </summary>
    [SerializeField] public TilePositionData tilePositionData;

    /// <summary>
    /// The pathfinder for line-of-sight and navigation.
    /// </summary>
    [SerializeField] public LineOfSightPathfinder pathfinder;

    /// <summary>
    /// Public method to generate the dungeon.
    /// This method clears the previous dungeon visualization and then calls the procedural generation algorithm
    /// to create a new dungeon. It can be triggered from the Unity editor using the context menu.
    /// </summary>
    [ContextMenu("Generate Dungeon")]
    void Start()
    {
        // Clear previous monsters if a map exists
        if (mapData != null)
        {
            monsterManager.ClearAllEnemies();
        }
        GenerateDungeon();
        pathfinder.InitializeFloorPositions();
        pathfinder.InitializeVisibilityGrid();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Public method to generate the dungeon.
    /// This method clears the previous dungeon visualization and then calls the procedural generation algorithm
    /// to create a new dungeon. It can be triggered from the Unity editor using the context menu.
    /// </summary>
    [ContextMenu("Generate Dungeon")]
=======
    /// Generates a new dungeon by clearing the previous visualization and running procedural generation.
    /// </summary>
>>>>>>> Stashed changes
    public void GenerateDungeon()
    {
        Debug.Log("Called the Dungeon Generator");
        // Clears the previous dungeon visualization
        tilemapVisualizer.Clear();
        // Calls the procedural generation algorithm to create a dungeon
        RunProceduralGeneration();
    }

    /// <summary>
    /// Procedural generation of the map and then painting the tiles.
<<<<<<< Updated upstream
=======
    /// Updates the map data, paints the tiles, and updates tile position data for pathfinding.
>>>>>>> Stashed changes
    /// </summary>
    private void RunProceduralGeneration()
    {
        mapData = mapGenerator.GenerateMap();
        Debug.Log(mapData == null);
        tilemapVisualizer.PaintTiles(mapData);
        tilePositionData.tilePositions = ConvertTilePositionsToCenter(mapData.floorPositions.ToList());
    }

    /// <summary>
    /// Converts a list of tile positions (Vector2Int) to their center positions (Vector2).
    /// Used for pathfinding and feature placement.
    /// </summary>
    /// <param name="tilePositions">List of tile positions as Vector2Int.</param>
    /// <returns>List of tile center positions as Vector2.</returns>
    private static List<Vector2> ConvertTilePositionsToCenter(List<Vector2Int> tilePositions)
    {
        List<Vector2> centerPositions = new List<Vector2>();
        foreach (Vector2Int position in tilePositions)
        {
            // Center of tile: (x + 0.5, y + 1)
            centerPositions.Add(new Vector2(position.x + 0.5f, position.y + 1f));
        }
        return centerPositions;
    }

<<<<<<< Updated upstream

    /// <summary>
    /// Clears tile positions on application quit.
=======
    /// <summary>
    /// Clears the tile position data when the application quits.
>>>>>>> Stashed changes
    /// </summary>
    void OnApplicationQuit()
    {
        tilePositionData.tilePositions.Clear();
    }
}
