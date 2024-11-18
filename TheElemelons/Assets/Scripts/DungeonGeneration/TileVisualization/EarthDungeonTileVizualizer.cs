using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EarthDungeonTileVizualizer : AbstractTilemapVisualizer
{
    [SerializeField] protected Tilemap floorTilemap, wallTilemap;        // Tilemaps for floors and walls
    [SerializeField] protected RuleTile wallRuleTile;                                           // Rule Tile for walls
    [SerializeField] protected TilemapData floorAtlasTilemapData;                               // Atlas tilemap for floor patterns
    [SerializeField] protected TileBase rimTopLeftTile, rimTopTile, rimTopRightTile,    
                                        rimLeftTile,                rimRightTile,               // Rim Tiles for room rims on the floor
                                        rimBottomLeftTile, rimBottomTile, rimBottomRightTile;


    private Tilemap atlasTilemap;

    // Method to initialize the atlas tilemap from TilemapData
    private void InitializeAtlasTilemap()
    {
        if (atlasTilemap == null)
        {
            GameObject atlasTilemapObject = new GameObject("AtlasTilemap");
            atlasTilemapObject.transform.SetParent(this.transform);
            atlasTilemap = atlasTilemapObject.AddComponent<Tilemap>();

            for (int i = 0; i < floorAtlasTilemapData.tilePositions.Count; i++)
            {
                Vector3Int tilePosition = (Vector3Int)floorAtlasTilemapData.tilePositions[i];
                TileBase tile = floorAtlasTilemapData.tiles[i];
                atlasTilemap.SetTile(tilePosition, tile);
            }
        }
    }

    // Public method to paint a list of tiles on the dungeon map.
    public override void PaintTiles(MapData mapData)
    {
        IEnumerable<Vector2Int> floorPositions = mapData.FloorPositions;
        IEnumerable<Vector2Int> wallPositions = mapData.WallPositions;
        IEnumerable<Vector2Int> decorationPositions = mapData.DecorationPositions;

        PaintFloor(floorPositions);                                    // Paint the floor tiles
        PaintWalls(new HashSet<Vector2Int>(floorPositions),
                   new HashSet<Vector2Int>(wallPositions));            // Paint the wall tiles
        DecorateDungeon(decorationPositions);                          // Optionally add decorations
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
        // Ensure the atlas tilemap is initialized
        InitializeAtlasTilemap();

        // Calculate the possible area of the source tilemap from where to draw
        int xMin = positions.Min(p => p.x);
        int xMax = positions.Max(p => p.x);
        int yMin = positions.Min(p => p.y);
        int yMax = positions.Max(p => p.y);

        int xLen = xMax - xMin + 1;
        int yLen = yMax - yMin + 1;
        BoundsInt sourceMapBounds = atlasTilemap.cellBounds;

        Debug.Log("Atlas source bounds: " + sourceMapBounds);
        Debug.Log("Atlas min and max x: " + sourceMapBounds.xMin + ", " + sourceMapBounds.xMax);
        Debug.Log("Atlas min and max y: " + sourceMapBounds.yMin + ", " + sourceMapBounds.yMax);

        // Use a random seed to select a starting point
        int xRand = Random.Range(sourceMapBounds.xMin, sourceMapBounds.xMax - xLen);
        int yRand = Random.Range(sourceMapBounds.yMin, sourceMapBounds.yMax - xLen);

        Debug.Log("Random starting point: " + xRand + ", " + yRand);

        Vector3Int startingPos = new Vector3Int(xRand, yRand, 0);
        Debug.Log("The tile at starting position is: " + atlasTilemap.GetTile(startingPos));

        
        foreach (var position in positions)
        {
            Vector3Int floorPos = floorTilemap.WorldToCell((Vector3Int)position);
            // If floor position is at the rim of the floor, use the border tiles
            if (!positions.Contains(position + Vector2Int.left)
             && !positions.Contains(position + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, rimTopLeftTile);
            }
            else if (!positions.Contains(position + Vector2Int.up)
                  && !positions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, rimTopRightTile);
            }
            else if (!positions.Contains(position + Vector2Int.left)
                  && !positions.Contains(position + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, rimBottomLeftTile);
            }
            else if (!positions.Contains(position + Vector2Int.down)
                  && !positions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, rimBottomRightTile);
            }
            else if (!positions.Contains(position + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, rimTopTile);
            }
            else if (!positions.Contains(position + Vector2Int.left))
            {
                floorTilemap.SetTile(floorPos, rimLeftTile);
            }
            else if (!positions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, rimRightTile);
            }
            else if (!positions.Contains(position + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, rimBottomTile);
            }

            // Else map between the floor positions and the positions on the atlas tilemap
            else
            {
                Vector3Int atlasPosition = new Vector3Int(position.x - xMin + xRand,
                                                        position.y - yMin + yRand,
                                                        0);
                TileBase atlasTile = atlasTilemap.GetTile(atlasPosition);
                if (atlasTile == null)
                    Debug.LogWarning("Got null tile at position " + atlasPosition);
                floorTilemap.SetTile(floorPos, atlasTile);
            }
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
                var lowerTilePos = wallTilemap.WorldToCell((Vector3Int)position);
                var upperTilePos = wallTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                // Set wall tile in one tilemap, this is sufficient with the current setup
                // since the character cannot hide behind walls
                wallTilemap.SetTile(lowerTilePos, wallRuleTile);
                wallTilemap.SetTile(upperTilePos, wallRuleTile);
            }
            else
            {
                var lowerTilePos = wallTilemap.WorldToCell((Vector3Int)position);
                wallTilemap.SetTile(lowerTilePos, wallRuleTile);
            }
        }
        
    }

    // Leave the dungeon undecorated for now
    protected void DecorateDungeon(IEnumerable<Vector2Int> floorPositions)
    {
        // TODO Add decorations
    }
}
