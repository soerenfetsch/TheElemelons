using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] protected Vector2Int startPosition;  // Starting position for dungeon generation
    [SerializeField] protected AbstractTilemapVisualizer tilemapVisualizer; // The Tilemap visualizer that handles the visual representation of the dungeon
    [SerializeField] protected AbstractMapGenerator mapGenerator; // The map generator that creates the lists of positions for a map
    [SerializeField] protected MonsterManager monsterManager;
    [SerializeField] public MapData mapData;
    [SerializeField] public TilePositionData tilePositionData;
    [SerializeField] public LineOfSightPathfinder pathfinder;
    /// <summary>
    /// Public method to generate the dungeon.
    /// This method clears the previous dungeon visualization and then calls the procedural generation algorithm
    /// to create a new dungeon. It can be triggered from the Unity editor using the context menu.
    /// </summary>
    [ContextMenu("Generate Dungeon")]
    /// <summary>
    /// Public method to generate the dungeon.
    /// This method clears the previous dungeon visualization and then calls the procedural generation algorithm
    /// to create a new dungeon. It can be triggered from the Unity editor using the context menu.
    /// </summary>
    void Start()
    {
        if (mapData != null)
        {
            monsterManager.ClearAllEnemies();
        }
        GenerateDungeon();
        pathfinder.InitializeFloorPositions();
        pathfinder.InitializeVisibilityGrid();
    }
    public void GenerateDungeon()
    {
        Debug.Log("Called the Dungeon Generator");
        // Clears the previous dungeon visualization
        tilemapVisualizer.Clear();
        // Calls the procedural generation algorithm to create a dungeon
        RunProceduralGeneration();
    }

    /// <summary>
    /// Procedural generation of the map and then painting the tiles
    /// </summary>
    private void RunProceduralGeneration()
    {
        mapData = mapGenerator.GenerateMap();
        Debug.Log(mapData == null);
        tilemapVisualizer.PaintTiles(mapData);
        tilePositionData.tilePositions = ConvertTilePositionsToCenter(mapData.floorPositions.ToList());
    }

    private static List<Vector2> ConvertTilePositionsToCenter(List<Vector2Int> tilePositions)
    {
        List<Vector2> centerPositions = new List<Vector2>();
        foreach (Vector2Int position in tilePositions)
        {
            centerPositions.Add(new Vector2(position.x + 0.5f, position.y + 1f));
        }
        return centerPositions;
    }

    void OnApplicationQuit()
    {
        tilePositionData.tilePositions.Clear();
    }
}
