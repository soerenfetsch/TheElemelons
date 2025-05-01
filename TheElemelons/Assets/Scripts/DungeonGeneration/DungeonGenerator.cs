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
        tilemapVisualizer.PaintTiles(mapData);
        tilePositionData.tilePositions = mapData.floorPositions.ToList();
    }
    void OnApplicationQuit()
    {
        tilePositionData.tilePositions.Clear();
    }
}
