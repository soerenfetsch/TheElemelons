using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BipartitioningMapGenerator : AbstractMapGenerator
{
    MapData mapData;
    public MonsterManager monsterManager;
    public Vector2Int startPosition = new Vector2Int(0, 0);
    public int minRoomWidth = 10;
    public int minRoomHeight = 10;
    public int dungeonWidth = 100;
    public int dungeonHeight = 100;
    public int offset = 3; // How many tiles offset for walls, etc.

    // Define how wide the corridors are
    public int corridorWidth = 2;

    public override MapData GenerateMap()
    {
        mapData = new MapData();
        CreateFloor(mapData);
        CreateWalls(mapData);
        CreateDecorationPositions(mapData);
        CreateMonsterSpawnPoints(mapData);
        CreateDoorPositions(mapData);
        createShrinePositions(mapData);
        createTrapPositions(mapData);
        monsterManager.mapData = mapData;
        monsterManager.SpawnMonsters();
        return mapData;
    }

    /// <summary>
    /// Generates the floor layout for the dungeon by creating rooms and corridors, 
    /// and updates the map data accordingly.
    /// </summary>
    /// <param name="mapData">The map data object that holds information about the dungeon layout.</param>
    public void CreateFloor(MapData mapData)
    {
        List<Room> rooms = new List<Room>();
        List<BoundsInt> roomSpaces = CreateRooms(rooms);
        HashSet<Vector2Int> corridorPositions = CreateCorridorsOrdering(roomSpaces, rooms, mapData);
        // Add the corridor positions to the map data
        mapData.AddFloorPositions(corridorPositions);
        // Edit the type of the last half of the rooms to be advanced rooms
        for (int i = mapData.rooms.Count / 2; i < rooms.Count; i++)
        {
            if (mapData.rooms[i].roomType == Room.RoomType.NORMAL)
            {
                mapData.rooms[i].SetRoomType(Room.RoomType.ADVANCED);
            }
        }
    }

    protected List<BoundsInt> CreateRooms(List<Room> roomList)
    {
        BoundsInt initialSpace = new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0));
        List<BoundsInt> roomSpaces = BipartitionSpace(initialSpace, minRoomWidth, minRoomHeight);
        // Create random rooms within the room spaces
        foreach (BoundsInt roomSpace in roomSpaces)
        {
            Room room = new Room();
            room.AddFloorPositions(CreateRandomRoomInSpace(roomSpace));
            roomList.Add(room);
        }

        return roomSpaces;
    }

    protected HashSet<Vector2Int> CreateCorridorsOrdering(List<BoundsInt> spaceBounds, List<Room> rooms, MapData mapData)
    {
        // Get the centers of the bounding boxes and map them to the corresponding rooms
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        Dictionary<Vector2Int, int> centerToRoomMapping = new Dictionary<Vector2Int, int>();

        int roomIndex = 0;
        foreach (var space in spaceBounds)
        {
            Vector2Int spaceCenter = (Vector2Int)Vector3Int.RoundToInt(space.center);
            roomCenters.Add(spaceCenter);
            centerToRoomMapping[spaceCenter] = roomIndex++;
        }

        // Connect the rooms by finding the closest room
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);
        mapData.AddRoom(rooms[centerToRoomMapping[currentRoomCenter]]);
        rooms[centerToRoomMapping[currentRoomCenter]].SetRoomType(Room.RoomType.START);
        //spawn in player in start room
        Transform playerTransform = GameObject.Find("Player").transform;
        playerTransform.position = new Vector3(currentRoomCenter.x, currentRoomCenter.y, 0);
        mapData.SetPlayerSpawnPosition(currentRoomCenter);
        HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();
        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosesPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            Room closestRoom = rooms[centerToRoomMapping[closest]];
            int xClosestMin = closestRoom.floorPositions.Min(pos => pos.x);
            int xClosestMax = closestRoom.floorPositions.Max(pos => pos.x);
            mapData.AddRoom(closestRoom);
            HashSet<Vector2Int> newCorridor = ConnectRooms(currentRoomCenter, closest,
                                                           xClosestMin, xClosestMax);
            currentRoomCenter = closest;
            corridorPositions.UnionWith(newCorridor);

            // Denote the last dungeon room as the end room
            if (roomCenters.Count == 0)
            {
                rooms[centerToRoomMapping[currentRoomCenter]].SetRoomType(Room.RoomType.END);
            }
        }

        return corridorPositions;
    }

    protected HashSet<Vector2Int> ConnectRooms(Vector2Int currentRoomCenter, Vector2Int destination,
                                             int xDestMin, int xDestMax)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

        // TODO: Evaluate if this is necessary since we restrict door placement now
        // // First fix corridors that cross room borders by offsetting the starting point

        // // Corridor overlaps with left border of the room
        // if (Mathf.Abs(xDestMin - position.x) <= (corridorWidth / 2 + 1))
        // {
        //     // Corridor overlaps more inside the room
        //     if (xDestMin <= position.x)
        //     {
        //         position += new Vector2Int(corridorWidth / 2 + 1 - xDestMin - position.x, 0);
        //     }
        //     // Corridor overlaps more outside the room
        //     else
        //     {
        //         position += new Vector2Int(xDestMin - position.x - corridorWidth / 2 - 2, 0);
        //     }
        // }
        // else if (Mathf.Abs(xDestMax - position.x) <= (corridorWidth / 2 + 1))
        // {
        //     // Corridor overlaps more inside the room
        //     if (xDestMax >= position.x)
        //     {
        //         position += new Vector2Int(xDestMax - position.x - corridorWidth / 2 - 1, 0);
        //     }
        //     // Corridor overlaps more outside the room
        //     else
        //     {
        //         position += new Vector2Int(corridorWidth / 2 + 2 - xDestMax - position.x, 0);
        //     }
        // }

        // Start a corridor of the given corridorWidth
        for (int i = 0; i < corridorWidth; ++i)
        {
            int added = (i + 1) / 2;
            if (i % 2 == 0)
                added = (-1) * added;
            corridor.Add(position + new Vector2Int(added, 0));
        }

        Vector2Int direction = Vector2Int.zero;
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
                direction = Vector2Int.up;
            else if (destination.y < position.y)
                direction = Vector2Int.down;
            position += direction;

            // Add a corridor of the given corridorWidth
            for (int i = 0; i < corridorWidth; ++i)
            {
                int added = (i + 1) / 2;
                if (i % 2 == 0)
                    added = (-1) * added;
                corridor.Add(position + new Vector2Int(added, 0));
            }
        }
        // Fix the corners of the corridor by going one step further at the end
        if (position.y == destination.y)
        {
            Vector2Int positionCopy = position;
            positionCopy += direction;
            for (int i = 0; i < corridorWidth; ++i)
            {
                int added = (i + 1) / 2;
                if (i % 2 == 0)
                    added = (-1) * added;
                corridor.Add(positionCopy + new Vector2Int(added, 0));
            }
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
                direction = Vector2Int.right;
            else if (destination.x < position.x)
                direction = Vector2Int.left;
            position += direction;

            // Add a corridor of the given corridorWidth
            for (int i = 0; i < corridorWidth; ++i)
            {
                int added = (i + 1) / 2;
                if (i % 2 == 0)
                    added = (-1) * added;
                corridor.Add(position + new Vector2Int(0, added));
            }
        }
        return corridor;
    }

    protected Vector2Int FindClosesPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = UnityEngine.Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    /// <summary>
    /// Creates walls around the floor positions in the map data.
    /// This method first adds walls around the floor positions of each room,
    /// and then adds walls around the floor positions of the corridors.
    /// Walls are placed around each floor tile if they are not contained in the map floor.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void CreateWalls(MapData mapData)
    {
        // First add walls to the rooms
        foreach (var room in mapData.rooms)
        {
            foreach (var position in room.floorPositions)
            {
                // Place walls around each floor tile if they are not contained in the map floor
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int neighborPos = new Vector2Int(position.x + x, position.y + y);
                        if (!mapData.floorPositions.Contains(neighborPos))
                        {
                            room.AddWallPosition(neighborPos);
                        }
                    }
                }
            }
        }

        // Then add walls to the corridors as well
        foreach (var position in mapData.floorPositions)
        {
            // Place walls around each floor tile if they are not contained in the map floor
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int neighborPos = new Vector2Int(position.x + x, position.y + y);
                    if (!mapData.floorPositions.Contains(neighborPos))
                    {
                        mapData.AddWallPosition(neighborPos);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places decorations randomly on some floor positions in the map data.
    /// This method iterates through the floor positions of each room and places decorations
    /// with a 2% chance. Decorations can be either destructible or indestructible, with a 50/50 chance.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void CreateDecorationPositions(MapData mapData)
    {
        // Place decorations randomly on some floor positions
        foreach (Room room in mapData.rooms)
        {
            foreach (var position in room.floorPositions)
            {
                if (Random.value > 0.98f) // 2% chance to place a decoration
                {
                    if (Random.value > 0.50f) // 50/50 if destructable or not
                    {
                        room.AddDestructableDecorationPosition(position);
                        mapData.AddDestructableDecorationPosition(position);
                    }
                    else
                    {
                        room.AddIndestructableDecorationPosition(position);
                        mapData.AddIndestructableDecorationPosition(position);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places monster spawn points randomly on some floor positions in the map data.
    /// This method iterates through the floor positions of each room and places monster spawn points
    /// with a 10% chance.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void CreateMonsterSpawnPoints(MapData mapData)
    {
        // Place monsters randomly on floor positions
        foreach (Room room in mapData.rooms)
        {
            foreach (var position in room.floorPositions)
            {
                if (Random.value > 0.9f) // 10% chance to spawn a monster
                {
                    room.AddMonsterSpawnPosition(position);
                    mapData.AddMonsterSpawnPosition(position);
                }
            }
        }
    }

    /// <summary>
    /// Determines the positions of doors by finding the corridors.
    /// This method iterates through the floor positions of each room and places doors
    /// at positions where the room borders intersect with corridors.
    /// Doors are placed exactly in the middle of the corridor if the gap is less than the corridor width wide.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void CreateDoorPositions(MapData mapData)
    {
        // Determine the positions of doors by finding the corridors
        foreach (Room room in mapData.rooms)
        {
            foreach (var position in room.floorPositions)
            {
                // Check if the any neighbors are not in the room but are in the map floor to place a door
                foreach (Vector2Int direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    Vector2Int neighborPos = position + direction;
                    if (!room.floorPositions.Contains(neighborPos) && mapData.floorPositions.Contains(neighborPos))
                    {
                        // Fix corridors intersecting with room borders by only placing doors when the gap is less than the corridor width wide
                        // This places doors exactly in the middle of the corridor
                        int halfCorridorWidth = corridorWidth / 2;
                        if (direction == Vector2Int.up || direction == Vector2Int.down)
                        {
                            if (mapData.floorPositions.Contains(neighborPos + (halfCorridorWidth + 1) * Vector2Int.left)
                                || mapData.floorPositions.Contains(neighborPos + (halfCorridorWidth + 1) * Vector2Int.right))
                                continue;
                        }
                        else if (direction == Vector2Int.left || direction == Vector2Int.right)
                        {
                            if (mapData.floorPositions.Contains(neighborPos + (halfCorridorWidth + 1) * Vector2Int.up)
                                || mapData.floorPositions.Contains(neighborPos + (halfCorridorWidth + 1) * Vector2Int.down))
                                continue;
                        }
                        // Else add a door position right in the middle of the corridor
                        room.AddDoorPosition(neighborPos);
                        mapData.AddDoorPosition(neighborPos);
                    }
                }
            }
        }
    }

    public void createShrinePositions(MapData mapData)
    {
        // TODO: Implement this
    }

    public void createTrapPositions(MapData mapData)
    {
        //TODO: Implement this
    }

    // Recursive bipartitioning to split space into rooms
    protected List<BoundsInt> BipartitionSpace(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                // Randomly attempt one type of split first
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }

            }
        }
        return roomsList;
    }

    protected void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x); // (1, room.size.x - 1)
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                                        new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    protected void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y); // (1, room.size.y - 1)
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                                        new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // Create a random room shape in a partitioned space
    protected HashSet<Vector2Int> CreateRandomRoomInSpace(BoundsInt space)
    {
        // For now just create a rectangle exactly within the space minus the offset for walls
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        for (int col = offset; col < space.size.x - offset; col++)
        {
            for (int row = offset; row < space.size.y - offset; row++)
            {
                Vector2Int position = (Vector2Int)space.min + new Vector2Int(col, row);
                roomPositions.Add(position);
            }
        }

        return roomPositions;
    }
}

