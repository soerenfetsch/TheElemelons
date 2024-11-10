using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    protected override List<Vector2Int> CreateFloor()
    {
        List<Vector2Int> floorPositions = new List<Vector2Int>();


        BoundsInt initialSpace = new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0));
        List<BoundsInt> roomSpaces = BipartitionSpace(initialSpace, minRoomWidth, minRoomHeight);
        // Create random rooms within the room spaces
        foreach (BoundsInt roomSpace in roomSpaces)
        {
            floorPositions.AddRange(CreateRandomRoomInSpace(roomSpace));
        }
        // Connect the rooms via corridor
        List<Vector2Int> corridorList = CreateCorridors(roomSpaces);
        // Floor is given by rooms and corridors
        floorPositions.AddRange(corridorList);
        return floorPositions;
    }

    private List<Vector2Int> CreateCorridors(List<BoundsInt> spaceBounds)
    {
        // First get the centers of the bounding boxes
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var space in spaceBounds)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(space.center));
        }

        // Connect the rooms using corridors
        HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosesPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = ConnectRooms(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridorPositions.UnionWith(newCorridor);
        }

        return new List<Vector2Int>(corridorPositions);
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

    protected override List<Vector2Int> CreateWalls(List<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            // Place walls around each floor tile (in a 1-tile radius)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int neighborPos = new Vector2Int(position.x + x, position.y + y);
                    if (!floorPositions.Contains(neighborPos))
                    {
                        wallPositions.Add(neighborPos);
                    }
                }
            }
        }

        return new List<Vector2Int>(wallPositions);
    }

    protected override List<Vector2Int> CreateDecorationPositions(List<Vector2Int> floorPositions)
    {
        // Example: Place decorations randomly on some floor positions
        List<Vector2Int> decorationPositions = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            if (Random.value > 0.95f) // 5% chance to place a decoration
            {
                decorationPositions.Add(position);
            }
        }
        return decorationPositions;
    }

    protected override List<Vector2Int> CreateMonsterSpawnPoints(List<Vector2Int> floorPositions)
    {
        // Example: Place monsters randomly on floor positions
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            if (Random.value > 0.9f) // 10% chance to spawn a monster
            {
                spawnPositions.Add(position);
            }
        }
        return spawnPositions;
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
    private List<Vector2Int> CreateRandomRoomInSpace(BoundsInt space)
    {
        // For now just create a rectangle exactly within the space minus the offset for walls
        List<Vector2Int> roomPositions = new List<Vector2Int>();
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

