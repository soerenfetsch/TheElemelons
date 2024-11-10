using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimpleTilemapVisualizer : AbstractTilemapVisualizer
{
    [SerializeField] protected Tilemap floorTilemap, wallTilemap; // Tilemaps for floors and walls
    [SerializeField] protected TileBase floorTile, wallTile;      // Tiles for floors and walls

    // Public method to paint a list of tiles on the dungeon map.
    public override void PaintTiles(MapData mapData)
    {
        IEnumerable<Vector2Int> floorPositions = mapData.FloorPositions;
        IEnumerable<Vector2Int> wallPositions = mapData.WallPositions;
        IEnumerable<Vector2Int> decorationPositions = mapData.DecorationPositions;

        PaintFloor(floorPositions);             // Paint the floor tiles
        PaintWalls(wallPositions);              // Paint the wall tiles
        DecorateDungeon(decorationPositions);   // Optionally add decorations
    }

    // Clear the existing dungeon tiles (reset Tilemap).
    public override void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    // Paint the floor tiles at the given positions
    protected override void PaintFloor(IEnumerable<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            // Convert the 2D position to the 3D cell position for the tilemap
            var tilePos = floorTilemap.WorldToCell((Vector3Int)position);
            floorTilemap.SetTile(tilePos, floorTile);
        }
    }

    // Paint the wall tiles at the given positions
    protected override void PaintWalls(IEnumerable<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            // Convert the 2D position to the 3D cell position for the tilemap
            var tilePos = wallTilemap.WorldToCell((Vector3Int)position);
            wallTilemap.SetTile(tilePos, wallTile);
        }
    }

    // Leave the dungeon undecorated
    protected override void DecorateDungeon(IEnumerable<Vector2Int> floorPositions)
    {
        // No decorations are being added in this simple version
    }
}

