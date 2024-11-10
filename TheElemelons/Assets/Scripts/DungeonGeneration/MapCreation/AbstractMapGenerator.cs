using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGenerator : MonoBehaviour
{
    // Public method to create the entire map (floors, walls, decorations, spawn points)
    public MapData GenerateMap()
    {
        // Initialize lists to hold the positions for each part of the map
        List<Vector2Int> floorPositions = new List<Vector2Int>();
        List<Vector2Int> wallPositions = new List<Vector2Int>();
        List<Vector2Int> decorationPositions = new List<Vector2Int>();
        List<Vector2Int> monsterSpawnPositions = new List<Vector2Int>();

        // Generate rooms and corridors (they form the floors)
        floorPositions.AddRange(CreateFloor());

        // Generate walls around the floors
        wallPositions.AddRange(CreateWalls(floorPositions));

        // Generate decorations
        decorationPositions.AddRange(CreateDecorationPositions(floorPositions));

        // Generate monster spawn points
        monsterSpawnPositions.AddRange(CreateMonsterSpawnPoints(floorPositions));

        // Return the map data as an object containing all relevant positions
        return new MapData(floorPositions, wallPositions, decorationPositions, monsterSpawnPositions);
    }

    // Abstract method to create floor, implemented in subclasses
    protected abstract List<Vector2Int> CreateFloor();

    // Abstract method to create walls around the floor, implemented in subclasses
    protected abstract List<Vector2Int> CreateWalls(List<Vector2Int> floorPositions);

    // Abstract method to place decoration positions, implemented in subclasses
    protected abstract List<Vector2Int> CreateDecorationPositions(List<Vector2Int> floorPositions);

    // Abstract method to determine monster spawn points, implemented in subclasses
    protected abstract List<Vector2Int> CreateMonsterSpawnPoints(List<Vector2Int> floorPositions);
}
