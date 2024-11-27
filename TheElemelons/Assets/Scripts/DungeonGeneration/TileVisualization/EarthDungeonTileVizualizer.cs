using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EarthDungeonTileVizualizer : AbstractTilemapVisualizer
{
    [SerializeField] protected Tilemap floorTilemap, collisionTilemap, decorationTilemap; // Tilemaps for floors and walls
    [SerializeField] protected TileBase leftWallBasic, leftCornerBasic, rightWallBasic, rightCornerBasic, // sandy walls
                                        topBottomWallBasic, topUpperWallBasic, bottomWallBasic;
    [SerializeField] protected TileBase leftWallAdvanced, leftCornerAdvanced, rightWallAdvanced, rightCornerAdvanced, // grassy walls
                                        topBottomWallAdvanced, topUpperWallAdvanced, bottomWallAdvanced;

    [SerializeField] protected TilemapData floorAtlasTilemapData;                         // Atlas tilemap for floor patterns
    // Rim tiles for the floor
    // Basic rim tiles for when rim is on the upper half of the tile and Other for lower half of the tile
    [SerializeField] protected TileBase rimTopLeftBasic, rimTopRightBasic, rimBottomLeftBasic, rimBottomRightBasic,
                                        rimTopBasic, rimLeftBasic, rimRightBasic, rimBottomBasic,
                                        rimTopLeftBasicOther, rimTopRightBasicOther, rimBottomLeftBasicOther, rimBottomRightBasicOther;
            
    // Advanced rim tiles for when rim is on the upper half of the tile and Other for lower half of the tile
    [SerializeField] protected TileBase rimTopLeftAdvanced, rimTopRightAdvanced, rimBottomLeftAdvanced, rimBottomRightAdvanced,
                                        rimTopAdvanced, rimLeftAdvanced, rimRightAdvanced, rimBottomAdvanced,
                                        rimTopLeftAdvancedOther, rimTopRightAdvancedOther, rimBottomLeftAdvancedOther, rimBottomRightAdvancedOther;
    // For the top and bottom doors the tiles are named after their respective position
    [SerializeField] protected TileBase topDoorBottomLeft, topDoorBottomMiddle, topDoorBottomRight,
                                        topDoorUpperLeft, topDoorUpperMiddle, topDoorUpperRight;
    [SerializeField] protected TileBase bottomDoorBottomLeft, bottomDoorBottomMiddle, bottomDoorBottomRight,
                                        bottomDoorUpperLeft, bottomDoorUpperMiddle, bottomDoorUpperRight;
    // For the left and right door the tiles are numbered from top to bottom
    [SerializeField] protected TileBase leftDoorOne, leftDoorTwo, leftDoorThree,
                                        leftDoorFour, leftDoorFive;
    [SerializeField] protected TileBase rightDoorOne, rightDoorTwo, rightDoorThree,
                                        rightDoorFour, rightDoorFive;

    protected enum RimPosition { UpperTopLeft, UpperTopRight, UpperBottomLeft, UpperBottomRight,
                               UpperTop, UpperLeft, UpperRight, UpperBottom,
                               LowerTopLeft, LowerTopRight, LowerBottomLeft, LowerBottomRight};
    protected Tilemap atlasTilemap;

    // Method to initialize the atlas tilemap from TilemapData
    private void InitializeAtlasTilemap()
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

        // We need to compress the bounds of the tilemap to get accurate BoundsInt
        atlasTilemap.CompressBounds();
    }

    private TileBase SelectTile(Room.RoomType roomType, RimPosition rimPosition)
    {
        if (roomType == Room.RoomType.START || roomType == Room.RoomType.NORMAL)
        {
            switch (rimPosition)
            {
                case RimPosition.UpperTopLeft:
                    return rimTopLeftBasic;
                case RimPosition.UpperTopRight:
                    return rimTopRightBasic;
                case RimPosition.UpperBottomLeft:
                    return rimBottomLeftBasic;
                case RimPosition.UpperBottomRight:
                    return rimBottomRightBasic;
                case RimPosition.UpperTop:
                    return rimTopBasic;
                case RimPosition.UpperLeft:
                    return rimLeftBasic;
                case RimPosition.UpperRight:
                    return rimRightBasic;
                case RimPosition.UpperBottom:
                    return rimBottomBasic;
                case RimPosition.LowerTopLeft:
                    return rimTopLeftBasicOther;
                case RimPosition.LowerTopRight:
                    return rimTopRightBasicOther;
                case RimPosition.LowerBottomLeft:
                    return rimBottomLeftBasicOther;
                case RimPosition.LowerBottomRight:
                    return rimBottomRightBasicOther;
            }
        }
        else if (roomType == Room.RoomType.ADVANCED || roomType == Room.RoomType.END)
        {
            switch (rimPosition)
            {
                case RimPosition.UpperTopLeft:
                    return rimTopLeftAdvanced;
                case RimPosition.UpperTopRight:
                    return rimTopRightAdvanced;
                case RimPosition.UpperBottomLeft:
                    return rimBottomLeftAdvanced;
                case RimPosition.UpperBottomRight:
                    return rimBottomRightAdvanced;
                case RimPosition.UpperTop:
                    return rimTopAdvanced;
                case RimPosition.UpperLeft:
                    return rimLeftAdvanced;
                case RimPosition.UpperRight:
                    return rimRightAdvanced;
                case RimPosition.UpperBottom:
                    return rimBottomAdvanced;
                case RimPosition.LowerTopLeft:
                    return rimTopLeftAdvancedOther;
                case RimPosition.LowerTopRight:
                    return rimTopRightAdvancedOther;
                case RimPosition.LowerBottomLeft:
                    return rimBottomLeftAdvancedOther;
                case RimPosition.LowerBottomRight:
                    return rimBottomRightAdvancedOther;
            }
        }
        return null;
    }

    // Public method to paint a list of tiles on the dungeon map.
    public override void PaintTiles(MapData mapData)
    {
        PaintFloor(mapData);                               // Paint the floor tiles
        PaintWalls(mapData);                               // Paint the wall tiles
        PaintDoors(mapData);                               // Paint the door tiles
        DecorateDungeon(mapData);                          // Optionally add decorations
    }

    // Clear the existing dungeon tiles (reset Tilemap).
    public override void Clear()
    {
        floorTilemap.ClearAllTiles();
        collisionTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();
    }

    // Paint the floor tiles at the given positions
    protected void PaintFloor(MapData mapData)
    {
        // Ensure the atlas tilemap is initialized
        InitializeAtlasTilemap();

        // Calculate the center of each room
        Dictionary<Vector2Int, Room.RoomType> roomCenters = new Dictionary<Vector2Int, Room.RoomType>();
        foreach (var room in mapData.rooms)
        {
            Vector2Int center = CalculateRoomCenter(room.floorPositions);
            roomCenters[center] = room.roomType;
        }

        // Calculate the possible area of the source tilemap from where to draw
        int xMin = mapData.floorPositions.Min(p => p.x);
        int xMax = mapData.floorPositions.Max(p => p.x);
        int yMin = mapData.floorPositions.Min(p => p.y);
        int yMax = mapData.floorPositions.Max(p => p.y);

        int xLen = xMax - xMin + 1;
        int yLen = yMax - yMin + 1;
        BoundsInt sourceMapBounds = atlasTilemap.cellBounds;

        Debug.Log("Atlas source bounds: " + sourceMapBounds);
        Debug.Log("Atlas min and max x: " + sourceMapBounds.xMin + ", " + sourceMapBounds.xMax);
        Debug.Log("Atlas min and max y: " + sourceMapBounds.yMin + ", " + sourceMapBounds.yMax);

        if (sourceMapBounds.xMax - xLen < sourceMapBounds.xMin)
        {
            Debug.LogWarning("Dungeon is larger in x direction than the given atlas map");
        }
        if (sourceMapBounds.yMax - yLen < sourceMapBounds.yMin)
        {
            Debug.LogWarning("Dungeon is larger in y direction than the given atlas map");
        }

        // Use a random seed to select a starting point
        int xRand = UnityEngine.Random.Range(sourceMapBounds.xMin, sourceMapBounds.xMax - xLen);
        int yRand = UnityEngine.Random.Range(sourceMapBounds.yMin, sourceMapBounds.yMax - yLen);

        Debug.Log("Random starting point: " + xRand + ", " + yRand);

        Vector3Int startingPos = new Vector3Int(xRand, yRand, 0);
        Debug.Log("The tile at starting position is: " + atlasTilemap.GetTile(startingPos));

        
        foreach (var position in mapData.floorPositions)
        {
            // Get the type of the closest room for each floor position
            Vector2Int closestRoomCenter = FindClosestRoomCenter(position, roomCenters.Keys);
            Room.RoomType closestRoomType = roomCenters[closestRoomCenter];

            Vector3Int floorPos = floorTilemap.WorldToCell((Vector3Int)position);

            // If floor position is at the rim of the floor, use the border tiles
            if (!mapData.floorPositions.Contains(position + Vector2Int.left)
             && !mapData.floorPositions.Contains(position + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperTopLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.up)
                  && !mapData.floorPositions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperTopRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left)
                  && !mapData.floorPositions.Contains(position + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperBottomLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.down)
                  && !mapData.floorPositions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperBottomRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperTop));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.UpperBottom));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.LowerTopRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.LowerTopLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.LowerBottomRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectTile(closestRoomType, RimPosition.LowerBottomLeft));
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
    protected void PaintWalls(MapData mapData)
    {
        // Calculate the center of each room
        Dictionary<Vector2Int, Room.RoomType> roomCenters = new Dictionary<Vector2Int, Room.RoomType>();
        foreach (var room in mapData.rooms)
        {
            Vector2Int center = CalculateRoomCenter(room.floorPositions);
            roomCenters[center] = room.roomType;
        }
        foreach (var position in mapData.wallPositions)
        {
            // Get the type of the closest room to decide on the rule tile
            Vector2Int closestRoomCenter = FindClosestRoomCenter(position, roomCenters.Keys);
            Room.RoomType closestRoomType = roomCenters[closestRoomCenter];
            // Set the wall rule tile based on the closest room type
            String wallType;
            if (closestRoomType == Room.RoomType.NORMAL || closestRoomType == Room.RoomType.START)
            {
                wallType = "Basic";
            }
            else
            {
                wallType = "Advanced";
            } 
            
            // Handle the different walls
            if (mapData.floorPositions.Contains(position + Vector2Int.down))
            {
                // Upper wall (two tiles high)
                var upperTilePos = collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                var lowerTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                if (wallType == "Basic")
                {
                    collisionTilemap.SetTile(upperTilePos, topUpperWallBasic);
                    collisionTilemap.SetTile(lowerTilePos, topBottomWallBasic);
                }
                else
                {
                    collisionTilemap.SetTile(upperTilePos, topUpperWallAdvanced);
                    collisionTilemap.SetTile(lowerTilePos, topBottomWallAdvanced);
                }
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.down)
                  && !mapData.wallPositions.Contains(position + Vector2Int.down))
            {
                // Lower wall (one tile high)
                var lowerTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                if (wallType == "Basic")
                {
                    collisionTilemap.SetTile(lowerTilePos, bottomWallBasic);
                }
                else
                {
                    collisionTilemap.SetTile(lowerTilePos, bottomWallAdvanced);
                }
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left)
                  && !mapData.floorPositions.Contains(position + 2 * Vector2Int.down)
                  && !mapData.floorPositions.Contains(position + Vector2Int.left + Vector2Int.down))
            {
                // Left wall
                if (!mapData.wallPositions.Contains(position + Vector2Int.up)
                 && !mapData.floorPositions.Contains(position + Vector2Int.up))
                {
                    // Top left corner
                    var cornerTilePos = collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                    if (wallType == "Basic")
                    {
                        collisionTilemap.SetTile(cornerTilePos, leftCornerBasic);
                    }
                    else
                    {
                        collisionTilemap.SetTile(cornerTilePos, leftCornerAdvanced);
                    }
                }

                var wallTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                if (wallType == "Basic")
                {
                    collisionTilemap.SetTile(wallTilePos, leftWallBasic);
                }
                else
                {
                    collisionTilemap.SetTile(wallTilePos, leftWallAdvanced);
                }
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right)
                  && !mapData.floorPositions.Contains(position + 2 * Vector2Int.down)
                  && !mapData.floorPositions.Contains(position + Vector2Int.right + Vector2Int.down))
            {
                // Right wall
                if (!mapData.wallPositions.Contains(position + Vector2Int.up)
                 && !mapData.floorPositions.Contains(position + Vector2Int.up))
                {
                    // Top right corner
                    var cornerTilePos = collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                    if (wallType == "Basic")
                    {
                        collisionTilemap.SetTile(cornerTilePos, rightCornerBasic);
                    }
                    else
                    {
                        collisionTilemap.SetTile(cornerTilePos, rightCornerAdvanced);
                    }
                }

                var wallTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                if (wallType == "Basic")
                {
                    collisionTilemap.SetTile(wallTilePos, rightWallBasic);
                }
                else
                {
                    collisionTilemap.SetTile(wallTilePos, rightWallAdvanced);
                }
            }
            else
            {
                // Tile handled by upper tile of top wall
            }
        }
    }

    // Leave the dungeon undecorated for now
    protected void DecorateDungeon(MapData mapData)
    {
        // TODO Add decorations
    }

    protected void PaintDoors(MapData mapData)
    {
        Debug.Log("Painting doors for positions: " + new List<Vector2Int>(mapData.doorPositions));
        foreach (Room room in mapData.rooms)
        {
            foreach (Vector2Int door in room.doorPositions)
            {
                // First find out whether door is top, bottom, left or right
                Vector2Int doorDirection; // right for top, down for right, left for bottom, up for left
                if (room.floorPositions.Contains(door + Vector2Int.down))
                {
                    // Top door
                    doorDirection = Vector2Int.right;
                }
                else if (room.floorPositions.Contains(door + Vector2Int.left))
                {
                    // Right door
                    doorDirection = Vector2Int.down;
                }
                else if (room.floorPositions.Contains(door + Vector2Int.up))
                {
                    // Bottom door
                    doorDirection = Vector2Int.left;
                }
                else
                {
                    // Left door
                    doorDirection = Vector2Int.up;
                }
                if (room.doorPositions.Contains(door - doorDirection))
                {
                    continue;
                }
                // Paint the door
                PaintDoor(door, doorDirection, room);
            }
        }
    }

    private Vector2Int CalculateRoomCenter(HashSet<Vector2Int> floorPositions)
    {
        int xSum = 0, ySum = 0;
        foreach (var pos in floorPositions)
        {
            xSum += pos.x;
            ySum += pos.y;
        }
        return new Vector2Int(xSum / floorPositions.Count, ySum / floorPositions.Count);
    }

    private Vector2Int FindClosestRoomCenter(Vector2Int position, IEnumerable<Vector2Int> roomCenters)
    {
        Vector2Int closestCenter = Vector2Int.zero;
        float minDistance = float.MaxValue;

        foreach (var center in roomCenters)
        {
            float distance = Vector2.Distance(position, center);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCenter = center;
            }
        }

        return closestCenter;
    }

    private void PaintDoor(Vector2Int position, Vector2Int doorDirection, Room room)
    {
        // Paint the door based on the door direction
        if (doorDirection == Vector2Int.right)
        {
            // Top door
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)position),
                topDoorBottomLeft
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up)),
                topDoorUpperLeft
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.right)),
                topDoorBottomMiddle
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.right + Vector2Int.up)),
                topDoorUpperMiddle
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.right)),
                topDoorBottomRight
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.right + Vector2Int.up)),
                topDoorUpperRight
            );
        }
        else if (doorDirection == Vector2Int.down)
        {
            // Door on the right

            // Top starts two tiles up in the wall
            position += 2 * Vector2Int.up;

            decorationTilemap.SetTile(
                decorationTilemap.WorldToCell((Vector3Int)position),
                rightDoorOne
            );
            decorationTilemap.SetTile(
                decorationTilemap.WorldToCell((Vector3Int)(position + Vector2Int.down)),
                rightDoorTwo
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.down)),
                rightDoorThree
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 3 * Vector2Int.down)),
                rightDoorFour
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 4 * Vector2Int.down)),
                rightDoorFive
            );
        }
        else if (doorDirection == Vector2Int.left)
        {
            // Bottom door
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)position),
                bottomDoorBottomRight
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up)),
                bottomDoorUpperRight
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.left)),
                bottomDoorBottomMiddle
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.left + Vector2Int.up)),
                bottomDoorUpperMiddle
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.left)),
                bottomDoorBottomLeft
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.left + Vector2Int.up)),
                bottomDoorUpperLeft
            );
        }
        else if (doorDirection == Vector2Int.up)
        {
            // Door on the left
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)position),
                leftDoorFive
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up)),
                leftDoorFour
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.up)),
                leftDoorThree
            );
            decorationTilemap.SetTile(
                decorationTilemap.WorldToCell((Vector3Int)(position + 3 * Vector2Int.up)),
                leftDoorTwo
            );
            decorationTilemap.SetTile(
                decorationTilemap.WorldToCell((Vector3Int)(position + 4 * Vector2Int.up)),
                leftDoorOne
            );
        }
    }
}
