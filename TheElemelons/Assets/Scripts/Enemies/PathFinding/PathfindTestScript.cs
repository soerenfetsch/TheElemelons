using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindTestScript : MonoBehaviour
{
    public Tilemap floorTilemap; // Reference to the tilemap for floor tiles.
    public Tilemap wallTilemap; // Reference to the tilemap for wall tiles.
    public TileBase floorTile; // Tile to represent the floor.
    public TileBase wallTile; // Tile to represent the wall.
    public LineOfSightPathfinder pathfinder; // Reference to the pathfinding system.

    public HashSet<Vector2Int> floorPositions; // Stores the positions of all floor tiles.
    public HashSet<Vector2Int> wallPositions; // Stores the positions of all wall tiles.

    /// <summary>
    /// Initializes and populates the tilemaps with sample data for testing.
    /// </summary>
    [ContextMenu("Populate Tilemaps")]
    public void PopulateTilemaps()
    {
        // Initialize position sets.
        floorPositions = new HashSet<Vector2Int>();
        wallPositions = new HashSet<Vector2Int>();

        // Define wall tile positions.
        List<Vector2Int> wallTiles = new List<Vector2Int>();
        int index = 10;
        for (int y = 4; y <= 11; y++)
        {
            wallTiles.Add(new Vector2Int(index, y));
        }
        // Define floor tile positions.
        List<Vector2Int> floorTiles = new List<Vector2Int>();
        for (int x = 0; x <= 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (wallTiles.Contains(position))
                {
                    continue;
                }
                floorTiles.Add(position);
            }
        }


        // Populate floor tiles.
        foreach (Vector2Int pos in floorTiles)
        {
            floorTilemap.SetTile((Vector3Int)pos, floorTile);
            floorPositions.Add(pos);
        }

        // Populate wall tiles.
        foreach (Vector2Int pos in wallTiles)
        {
            wallTilemap.SetTile((Vector3Int)pos, wallTile);
            wallPositions.Add(pos);
        }

        pathfinder.floorPositions = floorPositions;
        Debug.Log("Tilemaps populated with sample data.");

    }
}
