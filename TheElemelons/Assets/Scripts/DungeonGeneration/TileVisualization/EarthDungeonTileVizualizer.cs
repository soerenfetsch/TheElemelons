 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EarthDungeonTileVizualizer : AbstractTilemapVisualizer
{
    [SerializeField] protected Tilemap floorTilemap, lowerWallTilemap, upperWallTilemap; // Tilemaps for floors and walls
    [SerializeField] protected TileBase wallRuleTile;                                    // Rule Tile for walls

    // Public method to paint a list of tiles on the dungeon map.
    public override void PaintTiles(MapData mapData)
    {
        IEnumerable<Vector2Int> floorPositions = mapData.FloorPositions;
        IEnumerable<Vector2Int> wallPositions = mapData.WallPositions;
        IEnumerable<Vector2Int> decorationPositions = mapData.DecorationPositions;

        PaintFloor(floorPositions);                                    // Paint the floor tiles
        PaintWalls(new HashSet<Vector2Int>(floorPositions),
                   new HashSet<Vector2Int>(wallPositions));               // Paint the wall tiles
        DecorateDungeon(decorationPositions);                          // Optionally add decorations
    }

    // Clear the existing dungeon tiles (reset Tilemap).
    public override void Clear()
    {
        floorTilemap.ClearAllTiles();
        lowerWallTilemap.ClearAllTiles();
        upperWallTilemap.ClearAllTiles();
    }

    // Paint the floor tiles at the given positions
    protected void PaintFloor(IEnumerable<Vector2Int> positions)
    {
        // Calculate the possible area of the source tilemap from where to draw
        int xMin = positions.Min(p => p.x);
        int xMax = positions.Max(p => p.x);
        int yMin = positions.Min(p => p.y);
        int yMax = positions.Max(p => p.y);

        int xLen = xMax - xMin + 1;
        int yLen = yMax - yMin + 1;
        BoundsInt sourceMapBounds = floorTilemap.cellBounds;

        // Use a random seed to select a starting point
        int xRand = Random.Range(sourceMapBounds.xMin, sourceMapBounds.xMax - xLen);
        int yRand = Random.Range(sourceMapBounds.yMin, sourceMapBounds.yMax - xLen);

        // Map between the floor positions and the positions on the atlas tilemap
        foreach (var position in positions)
        {
            Vector2Int atlasPosition = new Vector2Int(position.x - xMin + xRand,
                                                      position.y - yMin + yRand);
            // Convert the 2D position to the 3D cell position for the tilemap
            Vector3Int tilePos = floorTilemap.WorldToCell((Vector3Int)atlasPosition);
            TileBase atlasTile = floorTilemap.GetTile(tilePos);
            Vector3Int floorPos = floorTilemap.WorldToCell((Vector3Int)position);
            floorTilemap.SetTile(floorPos, atlasTile);
        }
    }

    /// <summary>
    /// Paints the upper walls two tiles high with the upper being passable,
    /// the side is one wide and the bottom wall being one block high 
    /// </summary>
    protected void PaintWalls(HashSet<Vector2Int> floorPositions,
                              HashSet<Vector2Int> wallPositions)
    {
        foreach (var position in wallPositions)
        {
            // Handle the upper wall
            if (floorPositions.Contains(position + Vector2Int.down)
                || (!floorPositions.Contains(position + Vector2Int.left)
                    && !floorPositions.Contains(position + Vector2Int.up)
                    && wallPositions.Contains(position + Vector2Int.right)
                    && wallPositions.Contains(position + Vector2Int.down))
                || (!floorPositions.Contains(position + Vector2Int.right)
                    && !floorPositions.Contains(position + Vector2Int.up)
                    && wallPositions.Contains(position + Vector2Int.left)
                    && wallPositions.Contains(position + Vector2Int.down)))
            {
                // Convert the 2D position to the 3D cell position for the tilemap
                var lowerTilePos = lowerWallTilemap.WorldToCell((Vector3Int)position);
                var upperTilePos = upperWallTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                lowerWallTilemap.SetTile(lowerTilePos, wallRuleTile);
                upperWallTilemap.SetTile(upperTilePos, wallRuleTile);
            }
            else
            {
                var lowerTilePos = lowerWallTilemap.WorldToCell((Vector3Int)position);
                lowerWallTilemap.SetTile(lowerTilePos, wallRuleTile);
            }
        }
    }

    // Leave the dungeon undecorated for now
    protected void DecorateDungeon(IEnumerable<Vector2Int> floorPositions)
    {
        // TODO Add decorations
    }
}
