using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EarthDungeonTileVizualizer : AbstractTilemapVisualizer
{
    public Tilemap floorTilemap, collisionTilemap, decorationTilemap; // Tilemaps for floors and walls
    public WallTileSet basicWalls, advancedWalls; // Tiles for the walls

    public TilemapData floorAtlasTilemapData;                         // Atlas tilemap for floor patterns
    public RimTileSet basicRims, advancedRims; // Rim tiles
    // For the top and bottom doors the tiles are named after their respective position
    public DoorTileSet doorTileSet; // Door tileset for all four doors

    protected enum RimPosition
    {
        UpperTopLeft, UpperTopRight, UpperBottomLeft, UpperBottomRight,
        UpperTop, UpperLeft, UpperRight, UpperBottom,
        LowerTopLeft, LowerTopRight, LowerBottomLeft, LowerBottomRight
    };
    protected Tilemap atlasTilemap;

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

            // We need to compress the bounds of the tilemap to get accurate BoundsInt
            atlasTilemap.CompressBounds();
        }
    }

    private TileBase SelectRimTile(Room.RoomType roomType, RimPosition rimPosition)
    {
        RimTileSet rimTileSet;
        if (roomType == Room.RoomType.START || roomType == Room.RoomType.NORMAL)
        {
            rimTileSet = basicRims;
        }
        else if (roomType == Room.RoomType.ADVANCED || roomType == Room.RoomType.END)
        {
            rimTileSet = advancedRims;
        }
        else
        {
            return null;
        }
        switch (rimPosition)
        {
            case RimPosition.UpperTopLeft:
                return rimTileSet.rimTopLeft;
            case RimPosition.UpperTopRight:
                return rimTileSet.rimTopRight;
            case RimPosition.UpperBottomLeft:
                return rimTileSet.rimBottomLeft;
            case RimPosition.UpperBottomRight:
                return rimTileSet.rimBottomRight;
            case RimPosition.UpperTop:
                return rimTileSet.rimTop;
            case RimPosition.UpperLeft:
                return rimTileSet.rimLeft;
            case RimPosition.UpperRight:
                return rimTileSet.rimRight;
            case RimPosition.UpperBottom:
                return rimTileSet.rimBottom;
            case RimPosition.LowerTopLeft:
                return rimTileSet.rimTopLeftOther;
            case RimPosition.LowerTopRight:
                return rimTileSet.rimTopRightOther;
            case RimPosition.LowerBottomLeft:
                return rimTileSet.rimBottomLeftOther;
            case RimPosition.LowerBottomRight:
                return rimTileSet.rimBottomRightOther;
        }
        return null;
    }

    public override void PaintTiles(MapData mapData)
    {
        PaintFloor(mapData);                               // Paint the floor tiles
        PaintWalls(mapData);                               // Paint the wall tiles
        PaintDoors(mapData);                               // Paint the door tiles
        DecorateDungeon(mapData);                          // Optionally add decorations
    }

    public override void Clear()
    {
        floorTilemap.ClearAllTiles();
        collisionTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();
    }

    /// <summary>
    /// Paints the floor tiles at the given positions in the map data.
    /// This method initializes the atlas tilemap, calculates the center of each room, and determines the possible area of the source tilemap.
    /// It then uses a random seed to select a starting point and paints the floor tiles based on the positions and room types.
    /// If a floor position is at the rim of the floor, it uses the border tiles; otherwise, it maps between the floor positions and the positions on the atlas tilemap.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void PaintFloor(MapData mapData)
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
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperTopLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.up)
                  && !mapData.floorPositions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperTopRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left)
                  && !mapData.floorPositions.Contains(position + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperBottomLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.down)
                  && !mapData.floorPositions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperBottomRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperTop));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.UpperBottom));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.LowerTopRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right + Vector2Int.down))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.LowerTopLeft));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.left + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.LowerBottomRight));
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.right + Vector2Int.up))
            {
                floorTilemap.SetTile(floorPos, SelectRimTile(closestRoomType, RimPosition.LowerBottomLeft));
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
    public void PaintWalls(MapData mapData)
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
            WallTileSet wallTileSet;
            if (closestRoomType == Room.RoomType.NORMAL || closestRoomType == Room.RoomType.START)
            {
                wallTileSet = basicWalls;
            }
            else
            {
                wallTileSet = advancedWalls;
            }

            // Handle the different walls
            if (mapData.floorPositions.Contains(position + Vector2Int.down))
            {
                // Upper wall (two tiles high)
                var upperTilePos = collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                var lowerTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                collisionTilemap.SetTile(upperTilePos, wallTileSet.topWallUpper);
                collisionTilemap.SetTile(lowerTilePos, wallTileSet.topWallBottom);
            }
            else if (!mapData.floorPositions.Contains(position + Vector2Int.down)
                  && !mapData.wallPositions.Contains(position + Vector2Int.down))
            {
                // Lower wall (one tile high)
                var lowerTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                collisionTilemap.SetTile(lowerTilePos, wallTileSet.bottomWall);
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
                    collisionTilemap.SetTile(cornerTilePos, wallTileSet.leftCorner);
                }

                var wallTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                collisionTilemap.SetTile(wallTilePos, wallTileSet.leftWall);
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
                    collisionTilemap.SetTile(cornerTilePos, wallTileSet.rightCorner);
                }

                var wallTilePos = collisionTilemap.WorldToCell((Vector3Int)position);
                collisionTilemap.SetTile(wallTilePos, wallTileSet.rightWall);
            }
            else
            {
                // Tile handled by upper tile of top wall but one above might be corner
                if (!mapData.wallPositions.Contains(position + Vector2Int.up)
                 && mapData.wallPositions.Contains(position + Vector2Int.right))
                {
                    // Top left corner
                    var cornerTilePos = collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                    collisionTilemap.SetTile(cornerTilePos, wallTileSet.leftCorner);
                }
                else if (!mapData.wallPositions.Contains(position + Vector2Int.up)
                 && mapData.wallPositions.Contains(position + Vector2Int.left))
                {
                    // Top right corner
                    var cornerTilePos = collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up));
                    collisionTilemap.SetTile(cornerTilePos, wallTileSet.rightCorner);
                }
            }
        }
    }

    // Leave the dungeon undecorated for now
    public void DecorateDungeon(MapData mapData)
    {
        // TODO Add decorations
    }

    /// <summary>
    /// Paints the doors at the specified positions in the map data.
    /// This method iterates through the door positions of each room and determines the direction of each door
    /// (top, bottom, left, or right) based on the surrounding floor positions. It then paints the door in the
    /// appropriate direction.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions, rooms, and door positions.</param>
    public void PaintDoors(MapData mapData)
    {
        foreach (Room room in mapData.rooms)
        {
            foreach (Vector2Int door in room.doorPositions)
            {
                // First find out whether door is top, bottom, left or right
                Vector2Int doorDirection; // right for top, down for right, left for bottom, up for left
                if (room.floorPositions.Contains(door + Vector2Int.down))
                {
                    // Top door
                    doorDirection = Vector2Int.up;
                }
                else if (room.floorPositions.Contains(door + Vector2Int.left))
                {
                    // Right door
                    doorDirection = Vector2Int.right;
                }
                else if (room.floorPositions.Contains(door + Vector2Int.up))
                {
                    // Bottom door
                    doorDirection = Vector2Int.down;
                }
                else
                {
                    // Left door
                    doorDirection = Vector2Int.left;
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
        // Paint the door based on the door direction, initial position is in the middle of the door
        if (doorDirection == Vector2Int.up || doorDirection == Vector2Int.down)
        {
            DoorTileSet.HorizontalDoor horizontalDoor = null;
            if (doorDirection == Vector2Int.up)
            {
                // Top door
                horizontalDoor = doorTileSet.topDoor;
            }
            else if (doorDirection == Vector2Int.down)
            {
                // Bottom door
                horizontalDoor = doorTileSet.bottomDoor;
            }
            if (horizontalDoor == null)
            {
                return;
            }
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.left)),
                horizontalDoor.doorBottomLeft
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.left + Vector2Int.up)),
                horizontalDoor.doorUpperLeft
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)position),
                horizontalDoor.doorBottomMiddle
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.up)),
                horizontalDoor.doorUpperMiddle
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.right)),
                horizontalDoor.doorBottomRight
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + Vector2Int.right + Vector2Int.up)),
                horizontalDoor.doorUpperRight
            );
        }
        else if (doorDirection == Vector2Int.left || doorDirection == Vector2Int.right)
        {
            DoorTileSet.VerticalDoor verticalDoor = null;
            if (doorDirection == Vector2Int.left)
            {
                // Left door
                verticalDoor = doorTileSet.leftDoor;
            }
            else if (doorDirection == Vector2Int.right)
            {
                // Right door
                verticalDoor = doorTileSet.rightDoor;
            }
            if (verticalDoor == null)
            {
                return;
            }
            // Top starts two tiles up in the wall
            position += 3 * Vector2Int.up;

            decorationTilemap.SetTile(
                decorationTilemap.WorldToCell((Vector3Int)position),
                verticalDoor.doorOne
            );
            decorationTilemap.SetTile(
                decorationTilemap.WorldToCell((Vector3Int)(position + Vector2Int.down)),
                verticalDoor.doorTwo
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 2 * Vector2Int.down)),
                verticalDoor.doorThree
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 3 * Vector2Int.down)),
                verticalDoor.doorFour
            );
            collisionTilemap.SetTile(
                collisionTilemap.WorldToCell((Vector3Int)(position + 4 * Vector2Int.down)),
                verticalDoor.doorFive
            );
        }
    }
}
