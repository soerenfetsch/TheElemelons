using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Generates dungeon maps using a bipartitioning algorithm. Handles room creation, corridors, walls, decorations,
/// monster spawns, shrines, and darkness effects. Integrates with MonsterManager for monster placement.
=======
/// Generates dungeon maps using a recursive bipartitioning algorithm.
/// Handles room creation, corridor connection, wall placement, decorations, monster spawns, doors, shrines, and darkness effects.
>>>>>>> Stashed changes
/// </summary>
public class BipartitioningMapGenerator : AbstractMapGenerator
{
    // The map data being generated.
    MapData mapData;

    /// <summary>
    /// Reference to the MonsterManager for spawning monsters.
    /// </summary>
    public MonsterManager monsterManager;

    /// <summary>
    /// The starting position for the dungeon (bottom-left corner).
    /// </summary>
    public Vector2Int startPosition = new Vector2Int(0, 0);

    /// <summary>
    /// Minimum width for generated rooms.
    /// </summary>
    public int minRoomWidth = 10;

    /// <summary>
    /// Minimum height for generated rooms.
    /// </summary>
    public int minRoomHeight = 10;

    /// <summary>
    /// Total width of the dungeon.
    /// </summary>
    public int dungeonWidth = 100;

    /// <summary>
    /// Total height of the dungeon.
    /// </summary>
    public int dungeonHeight = 100;

    /// <summary>
    /// Offset for walls and room boundaries.
    /// </summary>
    public int offset = 3;

    /// <summary>
    /// Width of corridors connecting rooms.
    /// </summary>
    public int corridorWidth = 2;

    /// <summary>
    /// Probability that a room will contain a shrine.
    /// </summary>
    public float roomShrineProbability = 0.2f;

    /// <summary>
    /// Prefab for shrine objects.
    /// </summary>
    public GameObject shrinePrefab;

    /// <summary>
    /// Probability that a room will be dark.
    /// </summary>
    public float darkRoomProbability = 0.1f;

    /// <summary>
    /// Prefab for darkness effect in rooms.
    /// </summary>
    public GameObject darknessEffectPrefab;

    /// <summary>
<<<<<<< Updated upstream
    /// Generates the dungeon map and spawns monsters, shrines, and darkness effects.
    /// </summary>
    /// <returns>The generated MapData object containing all map information.</returns>
=======
    /// Generates the entire dungeon map, including rooms, corridors, walls, decorations, monsters, doors, shrines, and darkness.
    /// </summary>
    /// <returns>The generated <see cref="MapData"/> object.</returns>
>>>>>>> Stashed changes
    public override MapData GenerateMap()
    {
        mapData = new MapData();
        CreateFloor(mapData);
        CreateWalls(mapData);
        CreateDecorationPositions(mapData);
        CreateMonsterSpawnPoints(mapData);
        CreateDoorPositions(mapData);
        CreateShrinePositions(mapData);
        CreateDarkRooms(mapData);
        createTrapPositions(mapData);
        monsterManager.mapData = mapData;
        if (monsterManager.allMonsters == null)
        {
            monsterManager.allMonsters = new List<GameObject>();
        }
        monsterManager.SpawnMonsters();
        SpawnShrines(mapData);
        SpawnDarknessInRooms(mapData);
        return mapData;
    }

    // Randomly marks rooms as dark based on probability.
    private void CreateDarkRooms(MapData mapData)
    {
        foreach (var room in mapData.rooms)
        {
            if (room.roomType != Room.RoomType.NORMAL && room.roomType != Room.RoomType.ADVANCED)
                continue;
            RandomlySetRoomDarkness(room);
        }
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

    /// <summary>
    /// Splits the dungeon space into rooms using recursive bipartitioning and creates random rooms within each space.
    /// </summary>
    /// <param name="roomList">The list to populate with generated rooms.</param>
    /// <returns>List of BoundsInt representing the spaces for each room.</returns>
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

    /// <summary>
    /// Connects rooms with corridors by finding the closest room centers and updating the map data.
    /// Also sets the start and end rooms, and places the player and end portal.
    /// </summary>
    /// <param name="spaceBounds">List of room bounding boxes.</param>
    /// <param name="rooms">List of generated rooms.</param>
    /// <param name="mapData">The map data to update.</param>
    /// <returns>Set of Vector2Int positions representing corridor tiles.</returns>
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
                // Create a portal to the boss room
                GameObject endPortal = GameObject.Find("EndPortal");
                if (endPortal != null)
                {
                    Transform endPortalTransform = endPortal.transform;
                    endPortalTransform.position = new Vector3(currentRoomCenter.x, currentRoomCenter.y, 0);
                }
                else
                {
                    Debug.LogWarning("EndPortal not found in scene!");
                }
            }
        }

        return corridorPositions;
    }

    /// <summary>
    /// Connects two room centers with a corridor of the specified width.
    /// </summary>
    /// <param name="currentRoomCenter">The starting room center.</param>
    /// <param name="destination">The destination room center.</param>
    /// <param name="xDestMin">Minimum x of the destination room.</param>
    /// <param name="xDestMax">Maximum x of the destination room.</param>
    /// <returns>Set of Vector2Int positions representing the corridor tiles.</returns>
    protected HashSet<Vector2Int> ConnectRooms(Vector2Int currentRoomCenter, Vector2Int destination,
                                             int xDestMin, int xDestMax)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

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

    /// <summary>
    /// Finds the closest point in a list of room centers to the current room center.
    /// </summary>
    /// <param name="currentRoomCenter">The current room center.</param>
    /// <param name="roomCenters">List of other room centers.</param>
    /// <returns>The closest room center.</returns>
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

    /// <summary>
    /// Creates shrine positions in the rooms of the map data.
    /// This method iterates through the rooms and randomly selects positions for shrines
    /// with a certain probability. It checks if the position is at least 3 tiles away from walls
    /// and if there is a clear 3x3 area centered on the position before placing a shrine.
    /// The shrine positions are added to both the room and the map data.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void CreateShrinePositions(MapData mapData)
    {
        System.Random rng = new System.Random();

        foreach (var room in mapData.rooms)
        {
            if (room.roomType != Room.RoomType.NORMAL && room.roomType != Room.RoomType.ADVANCED)
                continue;

            // Roll the dice to see if this room gets a shrine
            if (rng.NextDouble() > roomShrineProbability)
                continue;

            HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();

            foreach (var pos in room.floorPositions)
            {
                // Check if the position is at least 3 tiles from the wall
                if (!IsShrineFarFromWall(pos, room.floorPositions))
                    continue;

                // Check for a clear 3x3 area centered on this tile
                if (IsShrineSpaceAvailable(pos, room))
                    candidates.Add(pos);
            }

            // If any candidate positions were found, pick one at random and add shrine
            if (candidates.Count > 0)
            {
                Vector2Int[] candidateArray = candidates.ToArray();
                Vector2Int shrinePos = candidateArray[rng.Next(candidateArray.Length)];

                room.AddShrinePosition(shrinePos);
                mapData.AddShrinePosition(shrinePos);
            }
        }
    }

    // Checks if a position is at least 3 tiles away from any wall.
    private bool IsShrineFarFromWall(Vector2Int pos, HashSet<Vector2Int> floorPositions)
    {
        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                Vector2Int checkPos = pos + new Vector2Int(dx, dy);
                if (!floorPositions.Contains(checkPos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Checks if a 3x3 area centered on pos is clear and not occupied by monsters.
    private bool IsShrineSpaceAvailable(Vector2Int pos, Room room)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int checkPos = pos + new Vector2Int(dx, dy);
                if (!room.floorPositions.Contains(checkPos) || room.monsterSpawnPositions.Contains(checkPos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Placeholder for trap placement logic. Not yet implemented.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void createTrapPositions(MapData mapData)
    {
        //TODO: Implement this
    }

    /// <summary>
    /// Recursively splits the dungeon space into rooms using bipartitioning.
    /// </summary>
    /// <param name="spaceToSplit">The space to split.</param>
    /// <param name="minWidth">Minimum width for a room.</param>
    /// <param name="minHeight">Minimum height for a room.</param>
    /// <returns>List of BoundsInt representing the spaces for each room.</returns>
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

    // Splits a room vertically and enqueues the resulting subrooms.
    protected void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x); // (1, room.size.x - 1)
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                                        new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // Splits a room horizontally and enqueues the resulting subrooms.
    protected void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y); // (1, room.size.y - 1)
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                                        new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // Creates a random room shape in a partitioned space (currently a rectangle minus the offset for walls).
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

    /// <summary>
    /// Spawns shrines in the rooms of the map data.
    /// This method iterates through the rooms and instantiates shrine prefabs at the specified positions.
    /// The shrine prefab should be assigned in the inspector.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void SpawnShrines(MapData mapData)
    {
        foreach (var room in mapData.rooms)
        {
            foreach (var shrinePosition in room.shrinePositions)
            {
                GameObject shrine = Instantiate(shrinePrefab, new Vector3(shrinePosition.x, shrinePosition.y, 0), Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Randomly sets the darkness state of a room based on probability.
    /// </summary>
    /// <param name="room">The room to update.</param>
    public void RandomlySetRoomDarkness(Room room)
    {
        // Randomly set the room to be dark with a certain probability
        if (Random.value < darkRoomProbability)
        {
            room.isDark = true;
        }
        else
        {
            room.isDark = false;
        }
    }

    /// <summary>
    /// Spawns darkness effects in rooms marked as dark.
    /// Instantiates the darkness effect prefab and scales it to fit the room.
    /// </summary>
    /// <param name="mapData">The map data containing the floor positions and rooms.</param>
    public void SpawnDarknessInRooms(MapData mapData)
    {
        if (darknessEffectPrefab == null)
        {
            Debug.LogError("Darkness effect prefab is not assigned!");
            return;
        }

        foreach (var room in mapData.rooms)
        {
            if (!room.isDark)
                continue;

            // Instantiate darkness effect in the room
            // Position has to be in the center at the bottom of the room
            float centerX = (room.floorPositions.Min(pos => pos.x) + room.floorPositions.Max(pos => pos.x)) / 2f + 0.5f;
            float bottomY = room.floorPositions.Min(pos => pos.y) + 0.5f;
            Vector3 darknessPosition = new Vector3(centerX, bottomY, 0);
            Debug.Log($"Spawning darkness effect at position: {darknessPosition}");
            GameObject darknessEffect = Instantiate(darknessEffectPrefab, darknessPosition, Quaternion.identity);
            // Adjust the transform scale to the room size
            Vector2 roomSize = new Vector2(room.floorPositions.Max(pos => pos.x) - room.floorPositions.Min(pos => pos.x) + 1,
                                           room.floorPositions.Max(pos => pos.y) - room.floorPositions.Min(pos => pos.y) + 1);
            darknessEffect.transform.localScale = new Vector3(roomSize.x, roomSize.y, 1);
        }
    }
}

