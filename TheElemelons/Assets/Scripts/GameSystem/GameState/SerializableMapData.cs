using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SerializableMapData
{
    public List<Vector2Int> floorPositions;
    public List<Vector2Int> wallPositions;
    public List<SerializableRoom> rooms;
    public List<Vector2Int> destructableDecorationPositions;
    public List<Vector2Int> indestructableDecorationPositions;
    public List<Vector2Int> monsterSpawnPositions;
    public List<Vector2Int> shrinePositions;
    public List<Vector2Int> trapPositions;
    public List<Vector2Int> doorPositions;

    public SerializableMapData() { }

    public SerializableMapData(MapData mapData)
    {
        this.floorPositions = new List<Vector2Int>(mapData.floorPositions);
        this.wallPositions = new List<Vector2Int>(mapData.wallPositions);
        this.rooms = mapData.rooms.Select(room => new SerializableRoom(room)).ToList();
        this.destructableDecorationPositions = new List<Vector2Int>(mapData.destructableDecorationPositions);
        this.indestructableDecorationPositions = new List<Vector2Int>(mapData.indestructableDecorationPositions);
        this.monsterSpawnPositions = new List<Vector2Int>(mapData.monsterSpawnPositions);
        this.shrinePositions = new List<Vector2Int>(mapData.shrinePositions);
        this.trapPositions = new List<Vector2Int>(mapData.trapPositions);
        this.doorPositions = new List<Vector2Int>(mapData.doorPositions);
    }

    public MapData ToMapData()
    {
        MapData mapData = new MapData();
        foreach (var pos in floorPositions)
        {
            mapData.AddFloorPosition(pos);
        }
        foreach (var pos in wallPositions)
        {
            mapData.AddWallPosition(pos);
        }
        foreach (var sRoom in rooms)
        {
            mapData.AddRoom(sRoom.ToRoom());
        }
        foreach (var pos in destructableDecorationPositions)
        {
            mapData.AddDestructableDecorationPosition(pos);
        }
        foreach (var pos in indestructableDecorationPositions)
        {
            mapData.AddIndestructableDecorationPosition(pos);
        }
        foreach (var pos in monsterSpawnPositions)
        {
            mapData.AddMonsterSpawnPosition(pos);
        }
        foreach (var pos in shrinePositions)
        {
            mapData.AddShrinePosition(pos);
        }
        foreach (var pos in trapPositions)
        {
            mapData.AddTrapPosition(pos);
        }
        foreach (var pos in doorPositions)
        {
            mapData.AddDoorPosition(pos);
        }
        return mapData;
    }
}