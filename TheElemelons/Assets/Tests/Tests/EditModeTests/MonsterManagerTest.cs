using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MonsterManagerTest
{
    private BipartitioningMapGenerator generator;
    private MapData mapData;
    private Room room;
    private MonsterManager monsterManager;

    [SetUp]
    public void SetUp()
    {
        generator = new BipartitioningMapGenerator();
        monsterManager = new MonsterManager();
        generator.monsterManager = monsterManager;
        //mapdata
        mapData = new MapData();
        generator.startPosition = new Vector2Int(0, 0);
        generator.dungeonWidth = 100;
        generator.dungeonHeight = 100;
        room = new Room();
        // Make room big enough to fit at least 3 monsters
        for (int x = 1; x <= 7; x++)
        {
            for (int y = 1; y <= 7; y++)
            {
                room.AddFloorPosition(new Vector2Int(x, y));
            }
        }
        room.roomType = Room.RoomType.NORMAL;
        mapData.AddRoom(room);
        generator.CreateWalls(mapData);
        generator.CreateDoorPositions(mapData);
        //mapdata
        monsterManager.mapData = mapData;
        monsterManager.allMonsters = new List<GameObject>();
        monsterManager.playerTransform = new GameObject().transform;
        monsterManager.playerTransform.position = new Vector2(0, 0);
        GameObject monster1 = new GameObject();
        monster1.AddComponent<TreeLog>();
        GameObject monster2 = new GameObject();
        monster2.AddComponent<TreeLog>();
        monsterManager.startEnemySelection = new List<GameObject> { monster1, monster2 };
        monsterManager.normalEnemySelection = new List<GameObject> { monster1, monster2 };
        monsterManager.InitializeRoomTypeSettings();
        Assert.AreEqual(0, monsterManager.allMonsters.Count, "enemylist not empty");
    }

    [Test]
    public void TestSpawnEnemies()
    {
        Assert.AreEqual(0, monsterManager.allMonsters.Count, "no 0 enemies");
        monsterManager.SpawnMonsters();
        Assert.True(monsterManager.allMonsters.Count > 0, "didnt spawn monsters");
    }
    [Test]
    public void TestClearAllEnemies()
    {
        monsterManager.SpawnMonsters();
        monsterManager.ClearAllEnemies();
        Assert.AreEqual(0, monsterManager.allMonsters.Count, "didnt clear all monsters");
    }

    [Test]
    public void TestSelectRandomSpawnPositions_SelectsPositionsInFloorPositions()
    {
        int count = 3;
        List<Vector2Int> spawnPositions = MonsterManager.SelectSpawnPositions(count, room);
        Assert.AreEqual(count, spawnPositions.Count,
                        $"Wrong number of spawn positions selected: {spawnPositions.Count} instead of {count}");
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Spawn Position {i}: {spawnPositions[i]}");
            Assert.True(room.floorPositions.Contains(spawnPositions[i]),
                        "Spawnposition not contained in floor");
        }
    }

    [Test]
    public void TestSelectRandomSpawnPositions_SelectsUniquePositions()
    {
        int count = 3;
        List<Vector2Int> spawnPositions = MonsterManager.SelectSpawnPositions(count, room);
        Assert.AreEqual(count, spawnPositions.Count,
                        $"Wrong number of spawn positions selected: {spawnPositions.Count} instead of {count}");
        HashSet<Vector2Int> uniquePositions = new HashSet<Vector2Int>(spawnPositions);
        Assert.AreEqual(count, uniquePositions.Count,
                        $"Selected spawn positions are not unique: "
                        + "{ spawnPositions[0]}, {spawnPositions[1]}, {spawnPositions[2]}");
    }

    [Test]
    public void TestSelectRandomSpawnPositions_AdjustsCountWhenAvailableSpaceTooLow()
    {
        // Construct a room where only 1-2 positions are available
        Room smallRoom = new Room();
        for (int x = 0; x <= 3; x++)
        {
            for (int y = 0; y <= 3; y++)
            {
                smallRoom.AddFloorPosition(new Vector2Int(x, y));
            }
        }
        smallRoom.roomType = Room.RoomType.NORMAL;
        mapData.AddRoom(smallRoom);
        generator.CreateWalls(mapData);
        generator.CreateDoorPositions(mapData);

        int count = 10; // Request more positions than available
        List<Vector2Int> spawnPositions = MonsterManager.SelectSpawnPositions(count, smallRoom);
        Assert.LessOrEqual(spawnPositions.Count, 2,
                           "Too many spawn positions picked!");
    }
}
