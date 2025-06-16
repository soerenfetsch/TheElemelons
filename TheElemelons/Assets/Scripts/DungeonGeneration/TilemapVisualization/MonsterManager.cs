using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages spawning, tracking, and door logic for dungeon monsters.
/// Handles enemy selection, spawn positions, door opening/closing, and end portal activation.
/// </summary>
public class MonsterManager : MonoBehaviour
{
    [Header("Enemy Selection")]
    /// <summary>
    /// List of enemy prefabs for start rooms.
    /// </summary>
    public List<GameObject> startEnemySelection;
    /// <summary>
    /// List of enemy prefabs for normal rooms.
    /// </summary>
    public List<GameObject> normalEnemySelection;
    /// <summary>
    /// List of enemy prefabs for advanced rooms.
    /// </summary>
    public List<GameObject> advancedEnemySelection;
    /// <summary>
    /// List of enemy prefabs for end rooms.
    /// </summary>
    public List<GameObject> endEnemySelection;
    /// <summary>
    /// List of enemy prefabs for boss rooms.
    /// </summary>
    public List<GameObject> bossEnemySelection;

    [Header("Difficulty Settings")]
    /// <summary>
    /// Number of enemies in start rooms.
    /// </summary>
    public int startEnemyCount = 1;
    /// <summary>
    /// Variability in enemy count for start rooms.
    /// </summary>
    public int startEnemyCountVariability = 0;
    /// <summary>
    /// Number of enemies in normal rooms.
    /// </summary>
    public int normalEnemyCount = 2;
    /// <summary>
    /// Variability in enemy count for normal rooms.
    /// </summary>
    public int normalEnemyCountVariability = 1;
    /// <summary>
    /// Number of enemies in advanced rooms.
    /// </summary>
    public int advancedEnemyCount = 3;
    /// <summary>
    /// Variability in enemy count for advanced rooms.
    /// </summary>
    public int advancedEnemyCountVariability = 1;
    /// <summary>
    /// Number of enemies in end rooms.
    /// </summary>
    public int endEnemyCount = 5;
    /// <summary>
    /// Variability in enemy count for end rooms.
    /// </summary>
    public int endEnemyCountVariability = 0;
    /// <summary>
    /// Number of enemies in boss rooms.
    /// </summary>
    public int bossEnemyCount = 6;
    /// <summary>
    /// Variability in enemy count for boss rooms.
    /// </summary>
    public int bossEnemyCountVariability = 1;

    [Header("Bound Components")]
    /// <summary>
    /// Reference to the current dungeon map data.
    /// </summary>
    public MapData mapData;
    /// <summary>
    /// Reference to the player transform.
    /// </summary>
    public Transform playerTransform;
    // Dictionary mapping room types to enemy count and variability.
    Dictionary<Room.RoomType, List<int>> roomTypeStats;
    // Dictionary mapping room types to enemy prefab selections.
    Dictionary<Room.RoomType, List<GameObject>> roomTypeSelections;
    /// <summary>
    /// List of all spawned monster GameObjects.
    /// </summary>
    public List<GameObject> allMonsters;
    /// <summary>
    /// Reference to the dungeon tile visualizer for painting doors.
    /// </summary>
    public EarthDungeonTileVizualizer earthDungeonTileVizualizer;

    [Header("Optimize Performance")]
    // Whether to deactivate enemies out of reach for performance.
    bool deactivateEnemiesOutOfReach = false;
    // Distance threshold for deactivating enemies.
    readonly float reachSetActive = 80f;
    // How often to check for deactivating enemies (frames).
    readonly int intervalDeactivateEnemiesOutOfReach = 120;
    // How often to check for door logic (frames).
    readonly int intervalOpenCloseDoors = 60;

    [Header("Dungeon Progression")]
    /// <summary>
    /// True if all enemies in the end room are defeated.
    /// </summary>
    public bool endRoomEnemiesDefeated = false;

    /// <summary>
<<<<<<< Updated upstream
    /// Finds player at start and initializes monster list.
=======
    /// Finds the player and initializes monster lists at start.
>>>>>>> Stashed changes
    /// </summary>
    void Start()
    {
        InitializeParams();
    }

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Initializes references and monster lists.
    /// </summary>
>>>>>>> Stashed changes
    public void InitializeParams()
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
<<<<<<< Updated upstream
    /// Initializes dictionaries for room type settings and enemy selections.
=======
    /// Initializes dictionaries for enemy selection and stats per room type.
>>>>>>> Stashed changes
    /// </summary>
    public void InitializeRoomTypeSettings()
    {
        roomTypeStats = new Dictionary<Room.RoomType, List<int>>();
        roomTypeSelections = new Dictionary<Room.RoomType, List<GameObject>>();
        roomTypeStats.Add(Room.RoomType.START, new List<int>() { startEnemyCount, startEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.NORMAL, new List<int>() { normalEnemyCount, normalEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.ADVANCED, new List<int>() { advancedEnemyCount, advancedEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.END, new List<int>() { endEnemyCount, endEnemyCountVariability });
        roomTypeStats.Add(Room.RoomType.BOSS, new List<int>() { bossEnemyCount, bossEnemyCountVariability });
        roomTypeSelections.Add(Room.RoomType.START, startEnemySelection);
        roomTypeSelections.Add(Room.RoomType.NORMAL, normalEnemySelection);
        roomTypeSelections.Add(Room.RoomType.ADVANCED, advancedEnemySelection);
        roomTypeSelections.Add(Room.RoomType.END, endEnemySelection);
        roomTypeSelections.Add(Room.RoomType.BOSS, bossEnemySelection);
    }

<<<<<<< Updated upstream
    // Update is called once per frame
=======
    // Update is called once per frame.
>>>>>>> Stashed changes
    void Update()
    {
        // Periodically check for door logic and enemy activation.
        if (Time.frameCount % intervalOpenCloseDoors == 0)
        {
            foreach (Room room in mapData.rooms)
            {
                OpenCloseDoor(room);
                EnableDisableEndPortal(room);
            }
        }
        if (deactivateEnemiesOutOfReach && Time.frameCount % intervalDeactivateEnemiesOutOfReach == 0)
        {
            foreach (GameObject monster in allMonsters)
            {
                if (monster.activeSelf && Mathf.Abs((monster.transform.position - playerTransform.position).magnitude) > reachSetActive)
                {
                    monster.SetActive(false);
                }
                else if (!monster.activeSelf && Mathf.Abs((monster.transform.position - playerTransform.position).magnitude) <= reachSetActive)
                {
                    monster.SetActive(true);
                }
            }
        }
    }

    // Enables or disables the end portal based on player and enemy state in the room.
    private void EnableDisableEndPortal(Room room)
    {
        bool playerIsInRoom = IsInRoom(playerTransform.position, room);
        if (playerIsInRoom && !AreMonstersInRoom(room))
        {
            endRoomEnemiesDefeated = true;
        }
        else
        {
            endRoomEnemiesDefeated = false;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Opens or closes doors depending on whether the player is close to enemies.
    /// </summary>
=======
    /// Opens or closes doors in a room depending on player and enemy presence.
    /// </summary>
    /// <param name="room">The room to update doors for.</param>
>>>>>>> Stashed changes
    public void OpenCloseDoor(Room room)
    {
        bool playerIsInRoom = IsInRoom(playerTransform.position, room);
        if (!playerIsInRoom || !AreMonstersInRoom(room))
        {
            if (!room.areDoorsOpen)
            {
                // Open doors
                foreach (Vector2Int door in room.doorPositions)
                {
                    earthDungeonTileVizualizer.PaintOpenDoor(door, GetDoorDir(room, door), room);
                }
            }
            room.areDoorsOpen = true;
        }
        else
        {
            if (room.areDoorsOpen)
            {
                // Close doors
                StartCoroutine(CloseDoors(room));
            }
            room.areDoorsOpen = false;
        }
    }

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Coroutine to close doors after the player leaves the doorway.
    /// </summary>
    /// <param name="room">The room to close doors for.</param>
>>>>>>> Stashed changes
    public IEnumerator CloseDoors(Room room)
    {
        while (IsInDoor(playerTransform.position, room))
        {
            yield return new WaitForSeconds(0.3f);
        }
        foreach (Vector2Int door in room.doorPositions)
        {
            earthDungeonTileVizualizer.PaintDoor(door, GetDoorDir(room, door), room);
        }
    }

    /// <summary>
    /// Checks if a position is within any door area of the room.
    /// </summary>
    /// <param name="position">World position to check.</param>
    /// <param name="room">Room to check against.</param>
    /// <returns>True if in a door area, false otherwise.</returns>
    public static bool IsInDoor(Vector3 position, Room room)
    {
        foreach (Vector2Int door in room.doorPositions)
        {
            Vector2Int doorDirection = GetDoorDir(room, door);
            if ((doorDirection == Vector2Int.up || doorDirection == Vector2Int.down)
             && IsInBounds(position, door + Vector2Int.left * 2, door + Vector2Int.right * 2 + Vector2Int.up * 2))
            {
                return true;
            }
            else if ((doorDirection == Vector2Int.left || doorDirection == Vector2Int.right)
                  && IsInBounds(position, door + 4 * Vector2Int.down + Vector2Int.left, door + Vector2Int.right))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a position is within the given bounds.
    /// </summary>
    /// <param name="position">Position to check.</param>
    /// <param name="minBound">Minimum bound (inclusive).</param>
    /// <param name="maxBound">Maximum bound (inclusive).</param>
    /// <returns>True if within bounds, false otherwise.</returns>
    public static bool IsInBounds(Vector2 position, Vector2 minBound, Vector2 maxBound)
    {
        if (position.x > minBound.x && position.y > minBound.y
            && position.x < maxBound.x && position.y < maxBound.y)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if there are any monsters in the given room.
    /// </summary>
    /// <param name="room">Room to check.</param>
    /// <returns>True if monsters are present, false otherwise.</returns>
    public static bool AreMonstersInRoom(Room room)
    {
        List<Vector2> bounds = GetRoomBounds(room);
        Vector2 size = bounds[1] - bounds[0];
        Vector2 center = bounds[0] + size / 2;

        // Only use the very insides of the room to prevent door closing too early
        Vector2 roomSize = size - new Vector2(1f, 1f);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, roomSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Enemy>() != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the direction of a door in a room.
    /// </summary>
    /// <param name="room">Room containing the door.</param>
    /// <param name="door">Door position.</param>
    /// <returns>Direction as a Vector2Int (up, down, left, right).</returns>
    public static Vector2Int GetDoorDir(Room room, Vector2Int door)
    {
        Vector2Int doorDirection;
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
<<<<<<< Updated upstream
    /// Spawns monsters in each room based on settings and random selection.
=======
    /// Spawns monsters in each room based on room type and settings.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Returns a list of enemies containing a random batch from the selection choice.
    /// Amount is random and regulated by parameters.
=======
    /// Selects a random batch of enemies from the selection list.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="SelectionChoice">List of possible enemy prefabs.</param>
    /// <param name="count">Base number of enemies.</param>
    /// <param name="variance">Random variance in enemy count.</param>
    /// <returns>List of selected enemy prefabs.</returns>
    public static List<GameObject> SelectRandomEnemies(List<GameObject> SelectionChoice, int count, int variance)
    {
        int amount = count + Random.Range(-variance, variance + 1);
        List<GameObject> Selection = new();
        for (int i = 0; i < amount; i++)
        {
            int choice = Random.Range(0, SelectionChoice.Count);
            Selection.Add(SelectionChoice[choice]);
        }
        return Selection;
    }

    /// <summary>
    /// Returns a list of random spawn positions inside the room bounds.
    /// </summary>
    /// <param name="count">Number of positions to generate.</param>
    /// <param name="room">Room to generate positions in.</param>
    /// <returns>List of spawn positions as Vector2Int.</returns>
    public static List<Vector2Int> SelectSpawnPositions(int count, Room room)
    {
        List<Vector2Int> positions = new();
        List<Vector2> bounds = GetRoomBounds(room);
        int x = 0;
        int y = 0;
        if (room.roomType != Room.RoomType.START)
        {
            for (int i = 0; i < count; i++)
            {
                x = (int)Random.Range(bounds[0].x + 2, bounds[1].x - 2);
                y = (int)Random.Range(bounds[0].y + 2, bounds[1].y - 2);

            }
        }
        else
        {
            x = (int)Random.Range(bounds[0].x + 2, ((bounds[1].x - bounds[0].x) / 2) + bounds[0].x - 2);
            y = (int)Random.Range(bounds[0].y + 2, bounds[1].y - 2);
        }
        positions.Add(new Vector2Int(x, y));
        return positions;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Checks if a position is within room bounds.
=======
    /// Checks if a position is within the bounds of a room.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="position">Position to check.</param>
    /// <param name="room">Room to check against.</param>
    /// <returns>True if position is in room, false otherwise.</returns>
    public static bool IsInRoom(Vector2 position, Room room)
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
<<<<<<< Updated upstream
    /// Returns a list {Vector2 minBounds, Vector2 maxBounds} of room walls.
=======
    /// Returns the min and max bounds of a room as a list of two Vector2s.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="room">Room to get bounds for.</param>
    /// <returns>List with min and max bounds as Vector2.</returns>
    public static List<Vector2> GetRoomBounds(Room room)
    {
        float maxX = room.wallPositions.Aggregate((point, point1) => point.x > point1.x ? point : point1).x;
        float maxY = room.wallPositions.Aggregate((point, point1) => point.y > point1.y ? point : point1).y;
        float minX = room.wallPositions.Aggregate((point, point1) => point.x < point1.x ? point : point1).x;
        float minY = room.wallPositions.Aggregate((point, point1) => point.y < point1.y ? point : point1).y;

        // Add an offset corresponding to center of tiles for better positioning
        minX += 0.5f;
        minY += 1f;
        maxX += 0.5f;
        maxY += 1f;

        List<Vector2> bounds = new()
        {
            new Vector2(minX, minY),
            new Vector2(maxX, maxY)
        };
        return bounds;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Clears all enemies from the scene.
=======
    /// Clears all enemies from the scene and the internal list.
>>>>>>> Stashed changes
    /// </summary>
    [ContextMenu("Clear all enemies")]
    public void ClearAllEnemies()
    {
        int len = allMonsters.Count;
        for (int i = len - 1; i >= 0; i--)
        {
            GameObject current_enemy = allMonsters[i];
            allMonsters.RemoveAt(i);
            DestroyImmediate(current_enemy);
        }
    }

    /// <summary>
    /// Checks if the end portal is enabled based on the state of the end room enemies.
    /// If all enemies in the end room are defeated, the end portal is enabled.
    /// </summary>
    /// <returns>True if the end portal should be enabled, false otherwise.</returns>
    public bool IsEndPortalEnabled()
    {
        return endRoomEnemiesDefeated;
    }
}
