using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MonsterManagerTest
{
    private BipartitioningMapGenerator generator;
    private EarthDungeonTileVizualizer visualizer;
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
        for (int x = 1; x <= 3; x++)
        {
            for (int y = 1; y <= 3; y++)
            {
                room.AddFloorPosition(new Vector2Int(x, y));
            }
        }
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
        Assert.AreEqual(monsterManager.allMonsters.Count, 0, "enemylist not empty");
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
    public void TestSelectRandomSpawnPositions()
    {
        int count = 3;
        List<Vector2Int> spawnPositions = monsterManager.SelectSpawnPositions(count, room);
        for (int i = 0; i < count; i++)
        {
            Assert.True(monsterManager.IsInRoom(spawnPositions[i], room), "Spawnposition out of bounds");
        }
    }
}
