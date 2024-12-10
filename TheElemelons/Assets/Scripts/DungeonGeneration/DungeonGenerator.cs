using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] protected Vector2Int startPosition;  // Starting position for dungeon generation
    [SerializeField] protected AbstractTilemapVisualizer tilemapVisualizer; // The Tilemap visualizer that handles the visual representation of the dungeon
    [SerializeField] protected AbstractMapGenerator mapGenerator; // The map generator that creates the lists of positions for a map

    // Public method to generate the dungeon
    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        Debug.Log("Called the Dungeon Generator");
        // Clears the previous dungeon visualization
        tilemapVisualizer.Clear();

        // Calls the procedural generation algorithm to create a dungeon
        RunProceduralGeneration();
    }

    // Procedural generation of the map and then painting the tiles
    private void RunProceduralGeneration()
    {
        MapData mapData = mapGenerator.GenerateMap();
        tilemapVisualizer.PaintTiles(mapData);
    }
}
