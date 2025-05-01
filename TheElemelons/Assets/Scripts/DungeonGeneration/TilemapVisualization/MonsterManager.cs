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
    public List<GameObject> allMonsters;
    public EarthDungeonTileVizualizer earthDungeonTileVizualizer;

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
        if (allMonsters == null)
        {
            allMonsters = new List<GameObject>();
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
    void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            foreach (Room room in mapData.rooms)
            {
                openCloseDoor(room);
            }
        }
    }
    /// <summary>
    /// yet unused method to open and close doors depending on whether the player is close to enemies
    /// </summary>
    /// <param name="room"></param>
    public void openCloseDoor(Room room)
    {
        bool playerIsInRoom = IsInRoom(playerTransform.position, room);
        if (!AreMonstersInRoom(room) || !playerIsInRoom)
        {
            if (!room.areDoorsOpen)
            {
                //open doors
                foreach (Vector2Int door in room.doorPositions)
                {
                    //paint open door
                    earthDungeonTileVizualizer.PaintOpenDoor(door, GetDoorDir(room, door), room);
                }
            }
            room.areDoorsOpen = true;
        }
        else
        {
            if (room.areDoorsOpen)
            {
                //close doors
                StartCoroutine(CloseDoors(room));
            }
            room.areDoorsOpen = false;
        }
    }
    public IEnumerator CloseDoors(Room room)
    {
        while (IsInDoor(playerTransform.position, room))
        {
            yield return new WaitForSeconds(0.3f);
        }
        foreach (Vector2Int door in room.doorPositions)
        {
            // Paint the door
            earthDungeonTileVizualizer.PaintDoor(door, GetDoorDir(room, door), room);
        }
    }
    public bool IsInDoor(Vector3 position, Room room)
    {
        foreach (Vector2Int door in room.doorPositions)
        {
            Debug.DrawRay(new Vector3(door.x, door.y), new Vector3(door.x, door.y), Color.cyan, 100f);
            Vector2Int doorDirection = GetDoorDir(room, door);
            if (doorDirection == Vector2Int.up || doorDirection == Vector2Int.down)
            {
                if (IsInBounds(playerTransform.position, door + Vector2Int.left * 2, door + Vector2Int.right * 2 + Vector2Int.up * 2))
                {
                    return true;
                }
            }
            else if (doorDirection == Vector2Int.left || doorDirection == Vector2Int.right)
            {
                if (IsInBounds(playerTransform.position, door + +4 * Vector2Int.down, door))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsInBounds(Vector2 position, Vector2 minBound, Vector2 maxBound)
    {
        if (position.x > minBound.x && position.y > minBound.y
        && position.x < maxBound.x && position.y < maxBound.y)
        {
            return true;
        }
        return false;
    }
    public bool AreMonstersInRoom(Room room)
    {
        List<Vector2> bounds = GetRoomBounds(room);
        Vector2 size = bounds[1] - bounds[0];
        Vector2 center = bounds[0] + size / 2;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Enemy>() != null)
            {
                return true;
            }
        }
        return false;
    }
    public Vector2Int GetDoorDir(Room room, Vector2Int door)
    {
        Vector2Int doorDirection; // right for top, down for right, left for bottom, up for left
        if (room.floorPositions.Contains(door + Vector2Int.down))
        {
            // Top door
            doorDirection = Vector2Int.up;
        }
        else if (room.floorPositions.Contains(door + Vector2Int.left))
        {
            // Right door
            doorDirection = Vector2Int.right;
        }
        else if (room.floorPositions.Contains(door + Vector2Int.up))
        {
            // Bottom door
            doorDirection = Vector2Int.down;
        }
        else
        {
            // Left door
            doorDirection = Vector2Int.left;
        }
        return doorDirection;
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
