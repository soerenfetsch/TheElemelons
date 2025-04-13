using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data transfer object that represents the entire map layout in a JSON-friendly format.
/// It converts runtime collections (e.g., HashSet) into lists for serialization and vice-versa.
/// </summary>
[System.Serializable]
public class SerializableMapData
{
    public List<Vector2Int> floorPositions;
    public List<Vector2Int> wallPositions;
    public List<Vector2Int> destructableDecorationPositions;
    public List<Vector2Int> indestructableDecorationPositions;
    public List<Vector2Int> monsterSpawnPositions;
    public List<Vector2Int> shrinePositions;
    public List<Vector2Int> trapPositions;
    public List<Vector2Int> doorPositions;
    public List<SerializableRoom> rooms;

    /// <summary>
    /// Creates a SerializableMapData from a runtime MapData object,
    /// converting runtime collections (HashSet) into JSON-friendly lists.
    /// </summary>
    /// <param name="mapData">The runtime MapData object. Must not be null.</param>
    /// <returns>The SerializableMapData representation or null if mapData is null.</returns>
    public static SerializableMapData FromMapData(MapData mapData)
    {
        if (mapData == null)
        {
            Debug.LogError("MapData is null. Cannot create SerializableMapData.");
            return null;
        }

        SerializableMapData serializable = new SerializableMapData();

        // Ensure each collection is not null to avoid errors.
        serializable.floorPositions = mapData.floorPositions != null ? new List<Vector2Int>(mapData.floorPositions) : new List<Vector2Int>();
        serializable.wallPositions = mapData.wallPositions != null ? new List<Vector2Int>(mapData.wallPositions) : new List<Vector2Int>();
        serializable.destructableDecorationPositions = mapData.destructableDecorationPositions != null ? new List<Vector2Int>(mapData.destructableDecorationPositions) : new List<Vector2Int>();
        serializable.indestructableDecorationPositions = mapData.indestructableDecorationPositions != null ? new List<Vector2Int>(mapData.indestructableDecorationPositions) : new List<Vector2Int>();
        serializable.monsterSpawnPositions = mapData.monsterSpawnPositions != null ? new List<Vector2Int>(mapData.monsterSpawnPositions) : new List<Vector2Int>();
        serializable.shrinePositions = mapData.shrinePositions != null ? new List<Vector2Int>(mapData.shrinePositions) : new List<Vector2Int>();
        serializable.trapPositions = mapData.trapPositions != null ? new List<Vector2Int>(mapData.trapPositions) : new List<Vector2Int>();
        serializable.doorPositions = mapData.doorPositions != null ? new List<Vector2Int>(mapData.doorPositions) : new List<Vector2Int>();

        serializable.rooms = new List<SerializableRoom>();
        if (mapData.rooms != null)
        {
            foreach (Room room in mapData.rooms)
            {
                // Convert each runtime room to SerializableRoom.
                SerializableRoom sRoom = SerializableRoom.FromRoom(room);
                if (sRoom != null)
                {
                    serializable.rooms.Add(sRoom);
                }
            }
        }

        // Note: HashSet to List conversion does not preserve any particular order.
        // If a specific order is required, sort the lists here.

        return serializable;
    }

    /// <summary>
    /// Converts the SerializableMapData back into a runtime MapData object.
    /// </summary>
    /// <returns>The reconstructed MapData object.</returns>
    public MapData ToMapData()
    {
        MapData mapData = new MapData();

        // Convert lists back to HashSets. Depending on your design, order does not matter.
        mapData.AddFloorPositions(new HashSet<Vector2Int>(floorPositions));
        mapData.AddWallPositions(new HashSet<Vector2Int>(wallPositions));
        mapData.AddDestructableDecorationPositions(new HashSet<Vector2Int>(destructableDecorationPositions));
        mapData.AddIndestructableDecorationPositions(new HashSet<Vector2Int>(indestructableDecorationPositions));
        mapData.AddMonsterSpawnPositions(new HashSet<Vector2Int>(monsterSpawnPositions));
        mapData.AddShrinePositions(new HashSet<Vector2Int>(shrinePositions));
        mapData.AddTrapPositions(new HashSet<Vector2Int>(trapPositions));
        mapData.AddDoorPositions(new HashSet<Vector2Int>(doorPositions));

        if (rooms != null)
        {
            foreach (var serializedRoom in rooms)
            {
                Room room = serializedRoom.ToRoom();
                mapData.AddRoom(room);
            }
        }

        return mapData;
    }
}
