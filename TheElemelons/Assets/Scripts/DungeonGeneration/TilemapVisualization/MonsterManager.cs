using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterManager : MonoBehaviour
{
    [Header("Enemy Selection")]
    public List<GameObject> startEnemySelection;
    public List<GameObject> advancedEnemySelection;
    [Header("Difficulty Settings")]
    public int startEnemyCount = 2;
    public int startEnemyCountVariability = 2;
    public int advancedEnemyCount = 3;
    public int advancedEnemyCountVariability = 2;
    [Header("Bound Components")]
    public MapData mapData;
    public Transform playerTransform;
    Dictionary<Room.RoomType, List<int>> roomTypeStats;
    Dictionary<Room.RoomType, List<GameObject>> roomTypeSelections;
    List<GameObject> allMonsters;

    /// <summary>
    /// find player at start
    /// </summary>
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        if (playerTransform == null)
        {
            Debug.Log("MonsterManager couldn't find the player at Start(), please fix");
        }

    }
    /// <summary>
    /// initialize dictionary that contains all settings for each roomtype
    /// </summary>
    public void InitializeRoomTypeSettings()
    {
        roomTypeStats = new Dictionary<Room.RoomType, List<int>>();
        roomTypeSelections = new Dictionary<Room.RoomType, List<GameObject>>();
        roomTypeStats.Add(Room.RoomType.START, new List<int>() { startEnemyCount, startEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.NORMAL, new List<int>() { startEnemyCount, startEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.ADVANCED, new List<int>() { advancedEnemyCount, advancedEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.END, new List<int>() { advancedEnemyCount, advancedEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.BOSS, new List<int>() { advancedEnemyCount, advancedEnemyCountVariability });
        roomTypeSelections.Add(Room.RoomType.START, startEnemySelection);
        roomTypeSelections.Add(Room.RoomType.NORMAL, startEnemySelection);
        roomTypeSelections.Add(Room.RoomType.ADVANCED, advancedEnemySelection);
        roomTypeSelections.Add(Room.RoomType.END, advancedEnemySelection);
        roomTypeSelections.Add(Room.RoomType.BOSS, advancedEnemySelection);
    }
    // Update is called once per frame
    /*void Update()
    {
        foreach (Room room in mapData.rooms)
        {
            //openCloseDoor(room);
        }
    }*/
    /// <summary>
    /// yet unused method to open and close doors depending on whether the player is close to enemies
    /// </summary>
    /// <param name="room"></param>
    public void openCloseDoor(Room room)
    {
        bool playerIsInRoom = IsInRoom(playerTransform.position, room);
        if (!playerIsInRoom || room.monsters.Count == 0)
        {
            room.areDoorsOpen = true;
        }
        else
        {
            room.areDoorsOpen = false;
        }
    }
    /// <summary>
    /// uses mapdata to spawn monsters in each room, amount depends on rng and settings
    /// </summary>
    public void SpawnMonsters()
    {
        InitializeRoomTypeSettings();
        foreach (Room room in mapData.rooms)
        {
            List<GameObject> enemyGroup = SelectRandomEnemies(roomTypeSelections[room.roomType], roomTypeStats[room.roomType][0], roomTypeStats[room.roomType][1]);
            List<Vector2Int> enemiesPositions = SelectSpawnPositions(enemyGroup.Count, room);
            for (int i = 0; i < enemiesPositions.Count; i++)
            {
                GameObject currentMonster = Instantiate(enemyGroup[i], new Vector3(enemiesPositions[i].x, enemiesPositions[i].y, 0), Quaternion.Euler(0, 0, 0));
                allMonsters.Add(currentMonster);
                room.AddMonsterSpawnPosition(enemiesPositions[i]);
                room.AddMonsters(currentMonster);
            }
        }
    }
    /// <summary>
    /// returns a list of enemies containing a random batch of those in the selection choice. amount is random and regulated by params
    /// </summary>
    /// <param name="SelectionChoice"></param>
    /// <param name="count"></param>
    /// <param name="variance"></param>
    /// <returns></returns>
    public List<GameObject> SelectRandomEnemies(List<GameObject> SelectionChoice, int count, int variance)
    {
        int amount = count + (int)UnityEngine.Random.Range(-variance, variance + 1);
        List<GameObject> Selection = new();
        for (int i = 0; i < amount; i++)
        {
            int choice = (int)UnityEngine.Random.Range(0, SelectionChoice.Count);
            Selection.Add(SelectionChoice[choice]);
        }
        return Selection;
    }
    /// <summary>
    ///  returns a list of random spawn positions inside the room bounds
    /// </summary>
    /// <param name="count"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    public List<Vector2Int> SelectSpawnPositions(int count, Room room)
    {
        List<Vector2Int> positions = new();
        List<Vector2> bounds = GetRoomBounds(room);
        for (int i = 0; i < count; i++)
        {
            int x = (int)Random.Range(bounds[0].x + 2, bounds[1].x - 2);
            int y = (int)Random.Range(bounds[0].y + 2, bounds[1].y - 2);
            positions.Add(new Vector2Int(x, y));
        }
        return positions;
    }
    /// <summary>
    /// checks if certain position is within room bounds
    /// </summary>
    /// <param name="position"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    public bool IsInRoom(Vector2 position, Room room)
    {
        List<Vector2> bounds = GetRoomBounds(room);
        if (position.x < bounds[0].x || position.y < bounds[0].y ||
        position.x > bounds[1].x || position.y > bounds[1].y)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// returns list{Vector2 minBounds, Vector2 maxBounds} of walls
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public List<Vector2> GetRoomBounds(Room room)
    {
        float maxX = (room.wallPositions.Aggregate((point, point1) => point.x > point1.x ? point : point1)).x;
        float maxY = (room.wallPositions.Aggregate((point, point1) => point.y > point1.y ? point : point1)).y;
        float minX = (room.wallPositions.Aggregate((point, point1) => point.x < point1.x ? point : point1)).x;
        float minY = (room.wallPositions.Aggregate((point, point1) => point.y < point1.y ? point : point1)).y;
        List<Vector2> bounds = new();
        bounds.Add(new Vector2(minX, minY));
        bounds.Add(new Vector2(maxX, maxY));
        return bounds;
    }

    [ContextMenu("Clear all enemies")]
    /// <summary>
    /// clear all enemies from scene
    /// </summary>
    public void ClearAllEnemies()
    {
        int len = allMonsters.Count;
        for (int i = len - 1; i >= 0; i--)
        {
            DestroyImmediate(allMonsters[i]);
        }
    }
}
