using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data transfer object that represents a single room's layout for serialization purposes.
/// It converts the runtime Room data (which may be stored in non-serializable collections like HashSet)
/// into JSON-friendly Lists, and stores the room type as a string.
/// </summary>
[System.Serializable]
public class SerializableRoom
{
    public List<Vector2Int> floorPositions;
    public List<Vector2Int> wallPositions;
    public List<Vector2Int> destructableDecorationPositions;
    public List<Vector2Int> indestructableDecorationPositions;
    public List<Vector2Int> monsterSpawnPositions;
    public List<Vector2Int> doorPositions;
    public List<Vector2Int> shrinePositions;
    public List<Vector2Int> trapPositions;
    public string roomType;

    /// <summary>
    /// Creates a SerializableRoom from a runtime Room object.
    /// </summary>
    /// <param name="room">The Room object to convert. Must not be null.</param>
    /// <returns>A SerializableRoom representing the given Room, or null if the room parameter is null.</returns>
    public static SerializableRoom FromRoom(Room room)
    {
        if (room == null)
        {
            Debug.LogError("Room is null. Cannot create SerializableRoom.");
            return null;
        }

        return new SerializableRoom
        {
            floorPositions = new List<Vector2Int>(room.floorPositions ?? new HashSet<Vector2Int>()),
            wallPositions = new List<Vector2Int>(room.wallPositions ?? new HashSet<Vector2Int>()),
            destructableDecorationPositions = new List<Vector2Int>(room.destructableDecorationPositions ?? new HashSet<Vector2Int>()),
            indestructableDecorationPositions = new List<Vector2Int>(room.indestructableDecorationPositions ?? new HashSet<Vector2Int>()),
            monsterSpawnPositions = new List<Vector2Int>(room.monsterSpawnPositions ?? new HashSet<Vector2Int>()),
            doorPositions = new List<Vector2Int>(room.doorPositions ?? new HashSet<Vector2Int>()),
            shrinePositions = new List<Vector2Int>(room.shrinePositions ?? new HashSet<Vector2Int>()),
            trapPositions = new List<Vector2Int>(room.trapPositions ?? new HashSet<Vector2Int>()),
            // Directly convert the enum to a string (an enum is never null)
            roomType = room.roomType.ToString()
        };
    }

    /// <summary>
    /// Converts the SerializableRoom back into a runtime Room object.
    /// </summary>
    /// <returns>The reconstructed Room object based on the serialized data.</returns>
    public Room ToRoom()
    {
        Room room = new Room();

        // Convert lists back to HashSets. If any list is null, use an empty HashSet.
        room.AddFloorPositions(new HashSet<Vector2Int>(floorPositions ?? new List<Vector2Int>()));
        room.AddWallPositions(new HashSet<Vector2Int>(wallPositions ?? new List<Vector2Int>()));
        room.AddDestructableDecorationPositions(new HashSet<Vector2Int>(destructableDecorationPositions ?? new List<Vector2Int>()));
        room.AddIndestructableDecorationPositions(new HashSet<Vector2Int>(indestructableDecorationPositions ?? new List<Vector2Int>()));
        room.AddMonsterSpawnPositions(new HashSet<Vector2Int>(monsterSpawnPositions ?? new List<Vector2Int>()));
        room.AddDoorPositions(new HashSet<Vector2Int>(doorPositions ?? new List<Vector2Int>()));
        room.AddShrinePositions(new HashSet<Vector2Int>(shrinePositions ?? new List<Vector2Int>()));
        room.AddTrapPositions(new HashSet<Vector2Int>(trapPositions ?? new List<Vector2Int>()));

        // Try parsing the roomType string into the enum. If parsing fails, default to NORMAL.
        if (System.Enum.TryParse(roomType, out Room.RoomType parsedType))
        {
            room.SetRoomType(parsedType);
        }
        else
        {
            room.SetRoomType(Room.RoomType.NORMAL);
        }

        return room;
    }
}
