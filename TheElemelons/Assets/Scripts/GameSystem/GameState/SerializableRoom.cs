using UnityEngine;
using System.Collections.Generic;
using static Room;

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
    public RoomType roomType;

    public SerializableRoom() { }

    public SerializableRoom(Room room)
    {
        this.floorPositions = new List<Vector2Int>(room.floorPositions);
        this.wallPositions = new List<Vector2Int>(room.wallPositions);
        this.destructableDecorationPositions = new List<Vector2Int>(room.destructableDecorationPositions);
        this.indestructableDecorationPositions = new List<Vector2Int>(room.indestructableDecorationPositions);
        this.monsterSpawnPositions = new List<Vector2Int>(room.monsterSpawnPositions);
        this.doorPositions = new List<Vector2Int>(room.doorPositions);
        this.shrinePositions = new List<Vector2Int>(room.shrinePositions);
        this.trapPositions = new List<Vector2Int>(room.trapPositions);
        this.roomType = room.roomType;
    }

    public Room ToRoom()
    {
        Room room = new Room();
        foreach (var pos in floorPositions)
        {
            room.AddFloorPosition(pos);
        }
        foreach (var pos in wallPositions)
        {
            room.AddWallPosition(pos);
        }
        foreach (var pos in destructableDecorationPositions)
        {
            room.AddDestructableDecorationPosition(pos);
        }
        foreach (var pos in indestructableDecorationPositions)
        {
            room.AddIndestructableDecorationPosition(pos);
        }
        foreach (var pos in monsterSpawnPositions)
        {
            room.AddMonsterSpawnPosition(pos);
        }
        foreach (var pos in doorPositions)
        {
            room.AddDoorPosition(pos);
        }
        foreach (var pos in shrinePositions)
        {
            room.AddShrinePosition(pos);
        }
        foreach (var pos in trapPositions)
        {
            room.AddTrapPosition(pos);
        }
        room.SetRoomType(roomType);
        return room;
    }
}