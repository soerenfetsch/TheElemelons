using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to hold the map data (positions for floors, walls, decorations, and monsters)
public class MapData
{
    public List<Vector2Int> FloorPositions { get; private set; }
    public List<Vector2Int> WallPositions { get; private set; }
    public List<Vector2Int> DecorationPositions { get; private set; }
    public List<Vector2Int> MonsterSpawnPositions { get; private set; }

    public MapData(List<Vector2Int> floorPositions, List<Vector2Int> wallPositions, 
                   List<Vector2Int> decorationPositions, List<Vector2Int> monsterSpawnPositions)
    {
        FloorPositions = floorPositions;
        WallPositions = wallPositions;
        DecorationPositions = decorationPositions;
        MonsterSpawnPositions = monsterSpawnPositions;
    }
}
