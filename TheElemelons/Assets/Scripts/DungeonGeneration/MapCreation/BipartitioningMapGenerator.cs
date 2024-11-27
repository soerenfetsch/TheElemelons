using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Collections;
using UnityEngine;

public class BipartitioningMapGenerator : AbstractMapGenerator
{
    [SerializeField] private Vector2Int startPosition = new Vector2Int(0, 0);
    [SerializeField] private int minRoomWidth = 10;
    [SerializeField] private int minRoomHeight = 10;
    [SerializeField] private int dungeonWidth = 100;
    [SerializeField] private int dungeonHeight = 100;
    [SerializeField] private int offset = 3; // How many tiles offset for walls, etc.

    // Define how wide the corridors are
    [SerializeField] private int corridorWidth = 2;

    public override MapData GenerateMap()
    {
        MapData mapData = new MapData();
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
        foreach (Room room in mapData.rooms)
        {
            Debug.Log("Room type: " + room.roomType);
        }
        CreateWalls(mapData);
        CreateDecorationPositions(mapData);
        CreateMonsterSpawnPoints(mapData);
        CreateDoorPositions(mapData);
        createShrinePositions(mapData);
        createTrapPositions(mapData);

        return mapData;
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

    private HashSet<Vector2Int> CreateCorridorsOrdering(List<BoundsInt> spaceBounds, List<Room> rooms, MapData mapData)
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

        HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();
        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosesPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            mapData.AddRoom(rooms[centerToRoomMapping[closest]]);
            HashSet<Vector2Int> newCorridor = ConnectRooms(currentRoomCenter, closest);
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

    private HashSet<Vector2Int> ConnectRooms(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
                position += Vector2Int.up;
            else if (destination.y < position.y)
                position += Vector2Int.down;
            // Add a corridor of the given corridorWidth
            for (int i = 0; i < corridorWidth; ++i)
            {   
                int added = (i + 1) / 2;
                if (i % 2 == 0)
                    added = (-1) * added;
                corridor.Add(position + new Vector2Int(added, 0));
            }
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
                position += Vector2Int.right;
            else if (destination.x < position.x)
                position += Vector2Int.left;
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

    private Vector2Int FindClosesPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
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

    protected void CreateWalls(MapData mapData)
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

    protected void CreateDecorationPositions(MapData mapData)
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

    protected void CreateMonsterSpawnPoints(MapData mapData)
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

    protected void CreateDoorPositions(MapData mapData)
    {
        // Determine the positions of doors by finding the corridors
        foreach (Room room in mapData.rooms)
        {
            foreach (var position in room.floorPositions)
            {
                // Check if any surrounding floor is in corridor but not in room
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int neighborPos = new Vector2Int(position.x + x, position.y + y);
                        if (!room.floorPositions.Contains(neighborPos) && mapData.floorPositions.Contains(neighborPos))
                        {
                            room.AddDoorPosition(neighborPos);
                            mapData.AddDoorPosition(neighborPos);
                        }
                    }
                }
            }
        }
    }

    protected void createShrinePositions(MapData mapData)
    {
        // TODO: Implement this
    }

    protected void createTrapPositions(MapData mapData)
    {
        //TODO: Implement this
    }

    // Recursive bipartitioning to split space into rooms
    private List<BoundsInt> BipartitionSpace(BoundsInt spaceToSplit, int minWidth, int minHeight)
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

    private void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x); // (1, room.size.x - 1)
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                                        new Vector3Int(room.size.x -  xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y); // (1, room.size.y - 1)
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                                        new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // Create a random room shape in a partitioned space
    private HashSet<Vector2Int> CreateRandomRoomInSpace(BoundsInt space)
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

