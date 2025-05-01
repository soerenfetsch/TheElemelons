using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Represents a room in the dungeon, storing positions of various elements and their types.
/// </summary>
public class Room
{
    public bool areDoorsOpen = false;
    /// <summary>
    /// Floor positions within the room.
    /// </summary>
    public HashSet<Vector2Int> floorPositions { get; private set; }

    /// <summary>
    /// Wall positions within the room.
    /// </summary>
    public HashSet<Vector2Int> wallPositions { get; private set; }

    /// <summary>
    /// Positions of destructible decorations.
    /// </summary>
    public HashSet<Vector2Int> destructableDecorationPositions { get; private set; }

    /// <summary>
    /// Positions of indestructible decorations.
    /// </summary>
    public HashSet<Vector2Int> indestructableDecorationPositions { get; private set; }

    /// <summary>
    /// Positions where monsters can spawn.
    /// </summary>
    public HashSet<Vector2Int> monsterSpawnPositions { get; private set; }
    /// <summary>
    /// Monsters in the room
    /// </summary>
    public HashSet<GameObject> monsters { get; private set; }

    /// <summary>
    /// Positions of doors in the room.
    /// </summary>
    public HashSet<Vector2Int> doorPositions { get; private set; }

    /// <summary>
    /// Positions of shrines in the room.
    /// </summary>
    public HashSet<Vector2Int> shrinePositions { get; private set; }

    /// <summary>
    /// Positions of traps in the room.
    /// </summary>
    public HashSet<Vector2Int> trapPositions { get; private set; }

    /// <summary>
    /// Enum defining types of rooms.
    /// </summary>
    public enum RoomType { START, NORMAL, ADVANCED, END, BOSS };

    /// <summary>
    /// The type of the room.
    /// </summary>
    public RoomType roomType;



    /// <summary>
    /// Initializes a new room with empty positions and a default type.
    /// </summary>
    public Room()
    {
        floorPositions = new HashSet<Vector2Int>();
        wallPositions = new HashSet<Vector2Int>();
        destructableDecorationPositions = new HashSet<Vector2Int>();
        indestructableDecorationPositions = new HashSet<Vector2Int>();
        monsterSpawnPositions = new HashSet<Vector2Int>();
        doorPositions = new HashSet<Vector2Int>();
        shrinePositions = new HashSet<Vector2Int>();
        trapPositions = new HashSet<Vector2Int>();
        monsters = new HashSet<GameObject>();
        roomType = RoomType.NORMAL;
    }

    /// <summary>
    /// Adds floor positions to the room.
    /// </summary>
    public void AddFloorPosition(Vector2Int floorPosition)
    {
        floorPositions.Add(floorPosition);
    }

    public void AddFloorPositions(HashSet<Vector2Int> floorPositions)
    {
        this.floorPositions.UnionWith(floorPositions);
    }

    /// <summary>
    /// Adds wall positions to the room.
    /// </summary>
    public void AddWallPosition(Vector2Int wallPosition)
    {
        wallPositions.Add(wallPosition);
    }

    public void AddWallPositions(HashSet<Vector2Int> wallPositions)
    {
        this.wallPositions.UnionWith(wallPositions);
    }

    /// <summary>
    /// Adds positions of destructible decorations to the room.
    /// </summary>
    public void AddDestructableDecorationPosition(Vector2Int decoPosition)
    {
        destructableDecorationPositions.Add(decoPosition);
    }

    public void AddDestructableDecorationPositions(HashSet<Vector2Int> decorationPositions)
    {
        this.destructableDecorationPositions.UnionWith(decorationPositions);
    }

    /// <summary>
    /// Adds positions of indestructible decorations to the room.
    /// </summary>
    public void AddIndestructableDecorationPosition(Vector2Int decoPosition)
    {
        indestructableDecorationPositions.Add(decoPosition);
    }

    public void AddIndestructableDecorationPositions(HashSet<Vector2Int> decorationPositions)
    {
        this.indestructableDecorationPositions.UnionWith(decorationPositions);
    }

    /// <summary>
    /// Adds positions for monster spawns in the room.
    /// </summary>
    public void AddMonsterSpawnPosition(Vector2Int monsterSpawnPosition)
    {
        monsterSpawnPositions.Add(monsterSpawnPosition);
    }
    public void AddMonsters(GameObject monster)
    {
        monsters.Add(monster);
    }
    public void AddMonsters(HashSet<GameObject> monster)
    {
        monsters.UnionWith(monster);
    }

    public void AddMonsterSpawnPositions(HashSet<Vector2Int> monsterSpawnPositions)
    {
        this.monsterSpawnPositions.UnionWith(monsterSpawnPositions);
    }

    /// <summary>
    /// Adds positions for doors in the room.
    /// </summary>
    public void AddDoorPosition(Vector2Int doorPosition)
    {
        doorPositions.Add(doorPosition);
    }

    public void AddDoorPositions(HashSet<Vector2Int> doorPositions)
    {
        this.doorPositions.UnionWith(doorPositions);
    }

    /// <summary>
    /// Adds positions for shrines in the room.
    /// </summary>
    public void AddShrinePosition(Vector2Int shrinePosition)
    {
        shrinePositions.Add(shrinePosition);
    }

    public void AddShrinePositions(HashSet<Vector2Int> shrinePositions)
    {
        this.shrinePositions.UnionWith(shrinePositions);
    }

    /// <summary>
    /// Adds positions for traps in the room.
    /// </summary>
    public void AddTrapPosition(Vector2Int trapPosition)
    {
        trapPositions.Add(trapPosition);
    }

    public void AddTrapPositions(HashSet<Vector2Int> trapPositions)
    {
        this.trapPositions.UnionWith(trapPositions);
    }

    /// <summary>
    /// Sets the type of the room.
    /// </summary>
    public void SetRoomType(RoomType roomType)
    {
        this.roomType = roomType;
    }
}


// Class to hold the map data (positions for floors, walls, decorations, and monsters)
public class MapData
{
    public Vector2 playerSpawn { get; private set; }
    public HashSet<Vector2Int> floorPositions { get; private set; }
    public HashSet<Vector2Int> wallPositions { get; private set; }
    public HashSet<Vector2Int> destructableDecorationPositions { get; private set; }
    public HashSet<Vector2Int> indestructableDecorationPositions { get; private set; }
    public HashSet<GameObject> monsters { get; private set; }
    public HashSet<Vector2Int> monsterSpawnPositions { get; private set; }
    public HashSet<Vector2Int> shrinePositions { get; private set; }
    public HashSet<Vector2Int> trapPositions { get; private set; }
    public HashSet<Vector2Int> doorPositions { get; private set; }
    public List<Room> rooms { get; private set; }

    public MapData()
    {
        floorPositions = new HashSet<Vector2Int>();
        wallPositions = new HashSet<Vector2Int>();
        destructableDecorationPositions = new HashSet<Vector2Int>();
        indestructableDecorationPositions = new HashSet<Vector2Int>();
        monsterSpawnPositions = new HashSet<Vector2Int>();
        shrinePositions = new HashSet<Vector2Int>();
        trapPositions = new HashSet<Vector2Int>();
        doorPositions = new HashSet<Vector2Int>();
        rooms = new List<Room>();
    }

    public MapData(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions,
                   HashSet<Vector2Int> destructableDecorationPositions,
                   HashSet<Vector2Int> indestructableDecorationPositions,
                   HashSet<Vector2Int> shrinePositions,
                   HashSet<Vector2Int> trapPositions,
                   HashSet<Vector2Int> doorPositions)
    {
        this.floorPositions = floorPositions;
        this.wallPositions = wallPositions;
        this.destructableDecorationPositions = destructableDecorationPositions;
        this.indestructableDecorationPositions = indestructableDecorationPositions;
        this.shrinePositions = shrinePositions;
        this.trapPositions = trapPositions;
        this.doorPositions = doorPositions;
    }

    public void AddRoom(Room room)
    {
        rooms.Add(room);
        floorPositions.UnionWith(room.floorPositions);
        wallPositions.UnionWith(room.wallPositions);
        destructableDecorationPositions.UnionWith(room.destructableDecorationPositions);
        indestructableDecorationPositions.UnionWith(room.indestructableDecorationPositions);
        monsterSpawnPositions.UnionWith(room.monsterSpawnPositions);
        shrinePositions.UnionWith(room.shrinePositions);
        trapPositions.UnionWith(room.trapPositions);
        doorPositions.UnionWith(room.doorPositions);
    }

    public void AddFloorPosition(Vector2Int floorPosition)
    {
        floorPositions.Add(floorPosition);
    }
    public void AddFloorPositions(HashSet<Vector2Int> floorPositions)
    {
        this.floorPositions.UnionWith(floorPositions);
    }
    public void AddWallPosition(Vector2Int wallPosition)
    {
        wallPositions.Add(wallPosition);
    }
    public void SetPlayerSpawnPosition(Vector2Int pos)
    {
        playerSpawn = pos;
    }
    public void AddWallPositions(HashSet<Vector2Int> wallPositions)
    {
        this.wallPositions.UnionWith(wallPositions);
    }
    public void AddDestructableDecorationPosition(Vector2Int decoPosition)
    {
        destructableDecorationPositions.Add(decoPosition);
    }
    public void AddDestructableDecorationPositions(HashSet<Vector2Int> decorationPositions)
    {
        this.destructableDecorationPositions.UnionWith(decorationPositions);
    }
    public void AddIndestructableDecorationPosition(Vector2Int decoPosition)
    {
        indestructableDecorationPositions.Add(decoPosition);
    }
    public void AddIndestructableDecorationPositions(HashSet<Vector2Int> decorationPositions)
    {
        this.indestructableDecorationPositions.UnionWith(decorationPositions);
    }
    public void AddMonsterSpawnPosition(Vector2Int monsterSpawnPosition)
    {
        monsterSpawnPositions.Add(monsterSpawnPosition);
    }
    public void AddMonsterSpawnPositions(HashSet<Vector2Int> monsterSpawnPositions)
    {
        this.monsterSpawnPositions.UnionWith(monsterSpawnPositions);
    }
    public void AddMonster(HashSet<GameObject> monster)
    {
        this.monsters.UnionWith(monster);
    }
    public void AddShrinePosition(Vector2Int shrinePosition)
    {
        shrinePositions.Add(shrinePosition);
    }
    public void AddShrinePositions(HashSet<Vector2Int> shrinePositions)
    {
        this.shrinePositions.UnionWith(shrinePositions);
    }
    public void AddTrapPosition(Vector2Int trapPosition)
    {
        trapPositions.Add(trapPosition);
    }
    public void AddTrapPositions(HashSet<Vector2Int> trapPositions)
    {
        this.trapPositions.UnionWith(trapPositions);
    }
    public void AddDoorPosition(Vector2Int doorPosition)
    {
        doorPositions.Add(doorPosition);
    }
    public void AddDoorPositions(HashSet<Vector2Int> doorPositions)
    {
        this.doorPositions.UnionWith(doorPositions);
    }
}
