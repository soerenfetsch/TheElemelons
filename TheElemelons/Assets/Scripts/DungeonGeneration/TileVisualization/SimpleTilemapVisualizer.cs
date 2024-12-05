using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimpleTilemapVisualizer : AbstractTilemapVisualizer
{
    [SerializeField] protected Tilemap floorTilemap, wallTilemap;         // Tilemaps for floors and walls
    [SerializeField] protected TileBase floorTile, wallTile, doorTile;     // Tiles for floors and walls

    // Public method to paint a list of tiles on the dungeon map.
    public override void PaintTiles(MapData mapData)
    {
        IEnumerable<Vector2Int> floorPositions = mapData.floorPositions;
        IEnumerable<Vector2Int> wallPositions = mapData.wallPositions;
        IEnumerable<Vector2Int> decorationPositions = mapData.destructableDecorationPositions;

        PaintFloor(floorPositions);             // Paint the floor tiles
        PaintWalls(wallPositions);              // Paint the wall tiles
        DecorateDungeon(decorationPositions);   // Optionally add decorations
        PaintDoors(mapData.doorPositions);      // Paint the door tiles
    }

    // Clear the existing dungeon tiles (reset Tilemap).
    public override void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    // Paint the floor tiles at the given positions
    protected void PaintFloor(IEnumerable<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            // Convert the 2D position to the 3D cell position for the tilemap
            var tilePos = floorTilemap.WorldToCell((Vector3Int)position);
            floorTilemap.SetTile(tilePos, floorTile);
        }
    }

    // Paint the wall tiles at the given positions
    protected void PaintWalls(IEnumerable<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            // Convert the 2D position to the 3D cell position for the tilemap
            var tilePos = wallTilemap.WorldToCell((Vector3Int)position);
            wallTilemap.SetTile(tilePos, wallTile);
        }
    }

    // Leave the dungeon undecorated
    protected void DecorateDungeon(IEnumerable<Vector2Int> floorPositions)
    {
        // No decorations are being added in this simple version
    }

    // Paint the door tiles at the given positions
    protected void PaintDoors(HashSet<Vector2Int> doorPositions)
    {
        foreach (var position in doorPositions)
        {
            // Convert the 2D position to the 3D cell position for the tilemap
            var tilePos = wallTilemap.WorldToCell((Vector3Int)position);
            wallTilemap.SetTile(tilePos, doorTile);
        }
    }
}

