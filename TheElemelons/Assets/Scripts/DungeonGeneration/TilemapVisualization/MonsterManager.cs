using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        if (playerTransform == null)
        {
            Debug.Log("MonsterManager couldn't find the player at Start(), please fix");
        }
        SpawnMonsters();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Room room in mapData.rooms)
        {
            //openCloseDoor(room);
        }
    }
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
    public void SpawnMonsters()
    {
        foreach (Room room in mapData.rooms)
        {
            if (room.roomType == Room.RoomType.START || true)
            {
                List<GameObject> enemyGroup = SelectRandomEnemies(startEnemySelection, startEnemyCount, startEnemyCountVariability);
                List<Vector2Int> enemiesPositions = SelectSpawnPositions(enemyGroup.Count, room);
                for (int i = 0; i < enemiesPositions.Count; i++)
                {
                    GameObject currentMonster = Instantiate(enemyGroup[i], new Vector3(enemiesPositions[i].x, enemiesPositions[i].y, 0), Quaternion.Euler(0, 0, 0));
                    room.AddMonsterSpawnPosition(enemiesPositions[i]);
                    room.AddMonsters(currentMonster);
                }
            }
        }
    }
    public List<GameObject> SelectRandomEnemies(List<GameObject> SelectionChoice, int count, int variance)
    {
        int amount = count + (int)Random.Range(-variance, variance + 1);
        List<GameObject> Selection = new();
        for (int i = 0; i < amount; i++)
        {
            int choice = (int)Random.Range(0, SelectionChoice.Count + 1);
            Selection.Add(SelectionChoice[choice]);
        }
        return Selection;
    }
    public List<Vector2Int> SelectSpawnPositions(int count, Room room)
    {
        List<Vector2Int> positions = new();
        List<Vector2> bounds = GetRoomBounds(room);
        for (int i = 0; i < count; i++)
        {
            int x = (int)Random.Range(bounds[0].x + 1, bounds[1].x - 1);
            int y = (int)Random.Range(bounds[0].y + 1, bounds[1].y - 1);
            positions.Add(new Vector2Int(x, y));
        }
        return positions;
    }
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
}
